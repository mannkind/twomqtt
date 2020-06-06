using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using TwoMQTT.Core.Models;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// An abstract class representing a managed way to interact with an MQTT broker.
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class MQTTManager<TQuestion, TData, TCommand> : BackgroundService
    {
        /// <summary>
        /// Initializes a new instance of the MQTTManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="opts"></param>
        /// <param name="incomingData"></param>
        /// <param name="outgoingCommand"></param>
        /// <param name="questions"></param>
        public MQTTManager(ILogger<MQTTManager<TQuestion, TData, TCommand>> logger, IOptions<MQTTManagerOptions> opts,
            ChannelReader<TData> incomingData, ChannelWriter<TCommand> outgoingCommand, IEnumerable<TQuestion> questions,
            string internalSettings)
        {
            this.Logger = logger;
            this.Opts = opts.Value;
            this.IncomingData = incomingData;
            this.OutgoingCommand = outgoingCommand;
            this.Client = new MqttFactory().CreateManagedMqttClient();
            this.KnownMessages = new ConcurrentDictionary<string, string>();
            this.Questions = questions;
            this.Logger.LogInformation(
                $"Broker:            {opts.Value.Broker}\n" +
                $"TopicPrefix:       {opts.Value.TopicPrefix}\n" +
                $"DiscoveryEnabled:  {opts.Value.DiscoveryEnabled}\n" +
                $"DiscoveryPrefix:   {opts.Value.DiscoveryPrefix}\n" +
                $"DiscoveryName:     {opts.Value.DiscoveryName}\n" +
                $"{internalSettings}"
            );
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        protected readonly ILogger<MQTTManager<TQuestion, TData, TCommand>> Logger;

        /// <summary>
        /// The options required to communicate properly with MQTT.
        /// </summary>
        protected readonly MQTTManagerOptions Opts;

        /// <summary>
        /// The channel reader used to communicate data from the source.
        /// </summary>
        protected readonly ChannelReader<TData> IncomingData;

        /// <summary>
        /// The channel writer used to communicate commands to the source.
        /// </summary>
        protected readonly ChannelWriter<TCommand> OutgoingCommand;

        /// <summary>
        /// The MQTT client used to access the the MQTT broker.
        /// </summary>
        protected readonly IManagedMqttClient Client;

        /// <summary>
        /// The cache of known published messages; used to not continually publish duplicate messages.
        /// </summary>
        protected readonly ConcurrentDictionary<string, string> KnownMessages;

        /// <summary>
        /// The questions to ask the source (typically some kind of key/slug pairing).
        /// Used for MQTT Discovery purposes.
        /// </summary>
        protected readonly IEnumerable<TQuestion> Questions;

        /// <summary>
        /// Executed as an IHostedService as a background job.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // Listen for incoming messages
            var readChannelTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ReadIncomingDataAsync(cancellationToken);
                }
            });

            var pollTask = Task.Run(async () =>
            {
                await Task.WhenAll(
                    this.ConnectedCallback(cancellationToken),
                    this.MessageReceivedCallback(cancellationToken)
                );
                await this.ConnectAsync(cancellationToken);
                await Task.WhenAll(
                    this.HandleSubscribeAsync(cancellationToken),
                    this.HandleDiscoveryAsync(cancellationToken)
                );
            });

            await Task.WhenAll(readChannelTask, pollTask);
        }

        /// <summary>
        /// Connect to the MQTT broker
        /// </summary>
        protected Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            return this.Client.StartAsync(new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(
                    new MqttClientOptionsBuilder()
                        .WithTcpServer(Opts.Broker)
                        .WithWillMessage(new MqttApplicationMessageBuilder()
                            .WithTopic($"{Opts.TopicPrefix}/status")
                            .WithPayload(Const.OFFLINE)
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
        /// Setup the callback for connecting to the MQTT broker.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task ConnectedCallback(CancellationToken cancellationToken = default)
        {
            this.Client.UseConnectedHandler(async e =>
            {
                await this.Client.PublishAsync(
                    new MqttApplicationMessageBuilder()
                        .WithTopic($"{this.Opts.TopicPrefix}/status")
                        .WithPayload(Const.ONLINE)
                        .WithExactlyOnceQoS()
                        .WithRetainFlag()
                        .Build(),
                    cancellationToken
                );
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Setup the callback for receiving messages from the MQTT broker.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task MessageReceivedCallback(CancellationToken cancellationToken = default)
        {
            this.Client.UseApplicationMessageReceivedHandler(async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                await this.HandleIncomingMessageAsync(topic, payload, cancellationToken);
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Subscribe to topics specific to the source.
        /// Topics may include incoming commands, the ability to manually query the source, etc.
        /// </summary>
        protected virtual Task HandleSubscribeAsync(CancellationToken cancellationToken = default)
        {
            var topics = this.Subscriptions() ?? new List<string>();
            return this.SubscribeAsync(topics, cancellationToken);
        }

        /// <summary>
        /// A list of topics to subscribe.
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        protected virtual IEnumerable<string> Subscriptions() => new List<string>();

        /// <summary>
        /// Publish MQTT discovery messages specific to the source.
        /// </summary>
        protected virtual Task HandleDiscoveryAsync(CancellationToken cancellationToken = default)
        {
            if (!this.Opts.DiscoveryEnabled)
            {
                return Task.CompletedTask;
            }

            var discoveries = this.Discoveries() ?? new List<(string, string, string, MQTTDiscovery)>();
            var tasks = discoveries.Select(x =>
                this.PublishDiscoveryAsync(x.slug, x.sensor, x.type, x.discovery, cancellationToken)
            );

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// A list of discoveries to publish.
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="MQTTDiscovery"></typeparam>
        protected virtual IEnumerable<(string slug, string sensor, string type, MQTTDiscovery discovery)> Discoveries() =>
            new List<(string, string, string, MQTTDiscovery)>();

        /// <summary>
        /// Handle incoming MQTT messages from the MQTT broker.
        /// </summary>
        protected virtual Task HandleIncomingMessageAsync(string topic, string payload,
            CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation(
                $"Received message via MQTT\n" +
                $"* Topic = {topic}\n" +
                $"* Payload = {payload}\n" +
                $""
            );

            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish discovery messages that indicate a source is available.
        /// </summary>
        protected async Task PublishDiscoveryAsync(string slug, string sensor, string sensorType,
            MQTTDiscovery discoveryMsg, CancellationToken cancellationToken = default)
        {
            var sensorName = this.Stringify(string.Empty, slug, sensor, '_');

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, discoveryMsg);

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic($"{this.Opts.DiscoveryPrefix}/{sensorType}/{this.Opts.DiscoveryName}/{sensorName}/config")
                .WithPayload(sw.ToString())
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await this.Client.PublishAsync(msg, cancellationToken);
        }

        /// <summary>
        /// Build an appropriate discovery message.
        /// </summary>
        public MQTTDiscovery BuildDiscovery(string slug, string sensor, AssemblyName assembly, bool hasCommand)
        {
            return new MQTTDiscovery
            {
                Name = this.Stringify(this.Opts.DiscoveryName, slug, sensor, ' '),
                AvailabilityTopic = this.AvailabilityTopic(),
                StateTopic = this.StateTopic(slug, sensor),
                CommandTopic = hasCommand ? this.CommandTopic(slug, sensor) : string.Empty,
                UniqueId = this.Stringify(this.Opts.DiscoveryName, slug, sensor, '.'),
                Device = new MQTTDiscovery.DiscoveryDevice
                {
                    Identifiers = new List<string> { this.AvailabilityTopic() },
                    Name = $"{assembly.Name?.ToLower() ?? "unknown"}2mqtt",
                    SWVersion = $"v{assembly.Version?.ToString() ?? "0.0.0"}",
                    Manufacturer = "twomqtt"
                }
            };
        }

        /// <summary>
        /// Read incoming messages from the source and publish them appropriately.
        /// </summary>
        protected async Task ReadIncomingDataAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested &&
                await this.IncomingData.WaitToReadAsync(cancellationToken))
            {
                var item = await this.IncomingData.ReadAsync(cancellationToken);
                this.Logger.LogDebug($"Received incoming data {item}");
                this.Logger.LogDebug($"Started handling {item}");
                await this.HandleIncomingDataAsync(item, cancellationToken);
                this.Logger.LogDebug($"Finished handling {item}");
            }
        }

        /// <summary>
        /// Publish messages based on a source specific input.
        /// </summary>
        protected virtual Task HandleIncomingDataAsync(TData input, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        /// <summary>
        /// Subscribe topics
        /// </summary>
        protected async Task SubscribeAsync(IEnumerable<string> filters, CancellationToken cancellationToken = default)
        {
            this.Logger.LogDebug($"Started subscribing to topics");
            var topics = filters.Select(x =>
            {
                this.Logger.LogDebug($"Found topic {x}");
                return new TopicFilterBuilder().WithTopic(x).Build();
            });

            await this.Client.SubscribeAsync(topics);
            this.Logger.LogDebug($"Finished subscribing to topics");
        }

        /// <summary>
        /// Publish topics + payloads, retained, depduplicated.
        /// </summary>
        protected async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
        {
            if (this.KnownMessages.ContainsKey(topic) && this.KnownMessages[topic] == payload)
            {
                this.Logger.LogDebug($"Duplicate '{payload}' found on '{topic}'");
                return;
            }

            this.Logger.LogInformation($"Publishing '{payload}' on '{topic}'");
            await this.Client.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build(),
                cancellationToken
            );

            this.KnownMessages[topic] = payload;
        }

        protected Task PublishAsync(IEnumerable<(string topic, string payload)> messages, CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task>();
            foreach (var message in messages)
            {
                tasks.Add(this.PublishAsync(message.topic, message.payload, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Convert a boolean to an appropriate on/off string.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string BooleanOnOff(bool val) => val ? Const.ON : Const.OFF;

        /// <summary>
        /// Generate the availability topic.
        /// </summary>
        protected string AvailabilityTopic() => $"{this.Opts.TopicPrefix}/status";

        /// <summary>
        /// Generate the state topic.
        /// </summary>
        protected string StateTopic(string slug, string sensor = "") =>
            $"{string.Join('/', new[] { this.Stringify(this.Opts.TopicPrefix, slug, sensor, '/'), "state" })}";


        /// <summary>
        /// Generate the command topic.
        /// </summary>
        protected string CommandTopic(string slug, string sensor = "") =>
            $"{string.Join('/', new[] { this.Stringify(this.Opts.TopicPrefix, slug, sensor, '/'), "command" })}";

        /// <summary>
        /// Generate a messy string.
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