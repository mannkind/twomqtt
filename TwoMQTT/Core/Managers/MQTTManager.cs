using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using TwoMQTT.Core.Models;

namespace TwoMQTT.Core.Managers
{
    public abstract class MQTTManager<TData, TCommand> : BackgroundService
    {
        public MQTTManager(ILogger<MQTTManager<TData, TCommand>> logger, IOptions<MQTTManagerOptions> opts, ChannelReader<TData> incoming, ChannelWriter<TCommand> outgoing)
        {
            this.logger = logger;
            this.opts = opts.Value;
            this.incoming = incoming;
            this.outgoing = outgoing;
            this.client = new MqttFactory().CreateManagedMqttClient();
            this.knownMessages = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Executed as an IHostedService as a background job.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            this.LogSettings();

            // Listen for incoming messages
            var readChannelTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ReadIncomingAsync(cancellationToken);
                }
            });

            var pollTask = Task.Run(async () =>
            {
                await this.ConnectAsync(cancellationToken);
                await this.HandleSubscribeAsync(cancellationToken);
                await this.HandleDiscoveryAsync(cancellationToken);
                await this.PublishOnlineStatus(cancellationToken);
            });

            await Task.WhenAll(readChannelTask, pollTask);
        }
        protected readonly ILogger<MQTTManager<TData, TCommand>> logger;
        protected readonly MQTTManagerOptions opts;
        protected readonly ChannelReader<TData> incoming;
        protected readonly ChannelWriter<TCommand> outgoing;
        protected readonly IManagedMqttClient client;
        protected readonly ConcurrentDictionary<string, string> knownMessages;

        /// <summary>
        /// Log settings specific to the MQTT client
        /// </summary>
        protected virtual void LogSettings()
        {
            this.logger.LogInformation(
                $"Broker:            {this.opts.Broker}\n" +
                $"TopicPrefix:       {this.opts.TopicPrefix}\n" +
                $"DiscoveryEnabled:  {this.opts.DiscoveryEnabled}\n" +
                $"DiscoveryPrefix:   {this.opts.DiscoveryPrefix}\n" +
                $"DiscoveryName:     {this.opts.DiscoveryName}\n" +
                $""
            );
        }

        /// <summary>
        /// Connect to the MQTT broker
        /// </summary>
        protected Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            return this.client.StartAsync(new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(
                    new MqttClientOptionsBuilder()
                        .WithTcpServer(opts.Broker)
                        .WithWillMessage(new MqttApplicationMessageBuilder()
                            .WithTopic($"{opts.TopicPrefix}/status")
                            .WithPayload("offline")
                            .WithExactlyOnceQoS()
                            .WithRetainFlag()
                            .Build()
                        )
                    .Build()
                )
                .Build()
            );
        }

        /// <summary>
        /// Subscribe to topics specific to the source.
        /// Topics may include incoming commands, the ability to manually query the source, etc.
        /// </summary>
        protected virtual Task HandleSubscribeAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish MQTT discovery messages specific to the source
        /// </summary>
        protected virtual Task HandleDiscoveryAsync(CancellationToken cancellationToken = default) 
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish LWT messages that indicate a source is online
        /// </summary>
        protected Task PublishOnlineStatus(CancellationToken cancellationToken = default)
        {
            return client.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic($"{opts.TopicPrefix}/status")
                    .WithPayload("online")
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                cancellationToken
            );
        }

        /// <summary>
        /// Publish discovery messages that indicate a source is available
        /// </summary>
        protected async Task PublishDiscoveryAsync(string slug, string sensor, string sensorType, MQTTDiscovery discoveryMsg, CancellationToken cancellationToken = default)
        {
            var sensorName = this.Stringify(string.Empty, slug, sensor, '_');

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, discoveryMsg);

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic($"{this.opts.DiscoveryPrefix}/{sensorType}/{this.opts.DiscoveryName}/{sensorName}/config")
                .WithPayload(sw.ToString())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await this.client.PublishAsync(msg, cancellationToken);
        }

        /// <summary>
        /// Build an appropriate discovery message
        /// </summary>
        public MQTTDiscovery BuildDiscovery(string slug, string sensor, AssemblyName assembly, bool hasCommand)
        {
            return new MQTTDiscovery
            {
                Name = this.Stringify(this.opts.DiscoveryName, slug, sensor, ' '),
                AvailabilityTopic = this.AvailabilityTopic(),
                StateTopic = this.StateTopic(slug, sensor),
                CommandTopic = hasCommand ? this.CommandTopic(slug, sensor) : string.Empty,
                UniqueId = this.Stringify(this.opts.DiscoveryName, slug, sensor, '.'),
                Device = new MQTTDiscoveryDevice
                {
                    Identifiers = new List<string> { this.AvailabilityTopic() },
                    Name = $"{assembly.Name?.ToLower() ?? "unknown"}2mqtt",
                    SWVersion = $"v{assembly.Version?.ToString() ?? "0.0.0"}",
                    Manufacturer = "twomqtt"
                }
            };
        }

        /// <summary>
        /// Read incoming messages from the source and publish them appropriately
        /// </summary>
        protected async Task ReadIncomingAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested && await this.incoming.WaitToReadAsync(cancellationToken))
            {
                var item = await this.incoming.ReadAsync(cancellationToken);
                await this.HandleIncomingAsync(item, cancellationToken);
            }
        }

        /// <summary>
        /// Publish messages based on a source specific input
        /// </summary>
        protected virtual Task HandleIncomingAsync(TData input, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Generate the availability topic
        /// </summary>
        protected string AvailabilityTopic() => $"{this.opts.TopicPrefix}/status";

        /// <summary>
        /// Generate the state topic
        /// </summary>
        protected string StateTopic(string slug, string sensor = "") => $"{string.Join('/', new[] { this.Stringify(this.opts.TopicPrefix, slug, sensor, '/'), "state"})}";


        /// <summary>
        /// Generate the command topic
        /// </summary>
        protected string CommandTopic(string slug, string sensor = "") => $"{string.Join('/', new[] { this.Stringify(this.opts.TopicPrefix, slug, sensor, '/'), "command"})}";

        /// <summary>
        /// Generate a messy string
        /// </summary>
        protected string Stringify(string prefix, string slug, string sensor, char seperator)
        {
            var pieces = new List<string>();

            if (!string.IsNullOrEmpty(prefix)) 
            {
                pieces.Add(prefix);
            }

            pieces.Add(slug);

            if (!string.IsNullOrEmpty(sensor)) 
            {
                pieces.Add(sensor);
            }

            return string.Join(seperator, pieces).ToLower();
        }
    }
}