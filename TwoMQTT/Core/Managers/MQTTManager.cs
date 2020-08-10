using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using TwoMQTT.Core.Utils;
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Models;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// A class representing a managed way to interact with an MQTT broker.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public class MQTTManager<TData, TCommand> : BackgroundService
        where TData : class
        where TCommand : class
    {
        /// <summary>
        /// Initializes a new instance of the MQTTManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="incomingData"></param>
        /// <param name="outgoingCommand"></param>
        /// <param name="client"></param>
        /// <param name="generator"></param>
        /// <param name="liason"></param>
        /// <param name="opts"></param>
        public MQTTManager(
            ILogger<MQTTManager<TData, TCommand>> logger,
            ChannelReader<TData> incomingData,
            ChannelWriter<TCommand> outgoingCommand,
            IManagedMqttClient client,
            IMQTTGenerator generator,
            IMQTTLiason<TData, TCommand> liason,
            IOptions<MQTTManagerOptions> opts
        )
        {
            this.Logger = logger;
            this.IncomingData = incomingData;
            this.OutgoingCommand = outgoingCommand;
            this.Client = client;
            this.Generator = generator;
            this.Liason = liason;
            this.Opts = opts.Value;
            this.Logger.LogInformation(
                "Broker:            {broker}\n" +
                "TopicPrefix:       {topicPrefix}\n" +
                "DiscoveryEnabled:  {discoveryEnabled}\n" +
                "DiscoveryPrefix:   {discoveryPrefix}\n" +
                "DiscoveryName:     {discoveryName}\n",
                opts.Value.Broker,
                opts.Value.TopicPrefix,
                opts.Value.DiscoveryEnabled,
                opts.Value.DiscoveryPrefix,
                opts.Value.DiscoveryName
            );
        }

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
                await foreach (var item in this.IncomingData.ReadAllAsync(cancellationToken))
                {
                    this.Logger.LogDebug("Started publishing data for {item}", item);
                    var pubs = this.Liason.MapData(item);
                    var tasks = new List<Task>();
                    foreach (var pub in pubs)
                    {
                        tasks.Add(this.PublishAsync(pub.topic, pub.payload, cancellationToken));
                    }

                    await Task.WhenAll(tasks);
                    this.Logger.LogDebug("Finished publishing data {item}", item);
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
        /// The logger used internally.
        /// </summary>
        private readonly ILogger<MQTTManager<TData, TCommand>> Logger;

        /// <summary>
        /// The mqtt liason.
        /// </summary>
        private readonly IMQTTLiason<TData, TCommand> Liason;

        /// <summary>
        /// The options required to communicate properly with MQTT.
        /// </summary>
        private readonly MQTTManagerOptions Opts;

        /// <summary>
        /// The channel reader used to communicate data from the source.
        /// </summary>
        private readonly ChannelReader<TData> IncomingData;

        /// <summary>
        /// The channel writer used to communicate commands to the source.
        /// </summary>
        private readonly ChannelWriter<TCommand> OutgoingCommand;

        /// <summary>
        /// The MQTT client used to access the the MQTT broker.
        /// </summary>
        protected readonly IManagedMqttClient Client;

        /// <summary>
        /// The MQTT generator used for things such as availability topic, state topic, command topic, etc.
        /// </summary>
        private readonly IMQTTGenerator Generator;

        /// <summary>
        /// The cache of known published messages; used to not continually publish duplicate messages.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> KnownMessages = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Connect to the MQTT broker
        /// </summary>
        private Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            return this.Client.StartAsync(new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(
                    new MqttClientOptionsBuilder()
                        .WithTcpServer(Opts.Broker)
                        .WithWillMessage(new MqttApplicationMessageBuilder()
                            .WithTopic(this.Generator.AvailabilityTopic())
                            .WithPayload(Const.OFFLINE)
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
        private Task ConnectedCallback(CancellationToken cancellationToken = default)
        {
            this.Client.UseConnectedHandler(async e =>
            {
                await this.Client.PublishAsync(
                    new MqttApplicationMessageBuilder()
                        .WithTopic(this.Generator.AvailabilityTopic())
                        .WithPayload(Const.ONLINE)
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
        private Task MessageReceivedCallback(CancellationToken cancellationToken = default)
        {
            this.Client.UseApplicationMessageReceivedHandler(async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = e.ApplicationMessage.ConvertPayloadToString();
                var cmds = this.Liason.MapCommand(topic, payload);
                var tasks = cmds.Select(cmd =>
                    this.OutgoingCommand.WriteAsync(cmd, cancellationToken).AsTask()
                );

                await Task.WhenAll(tasks);
                await this.Liason.HandleCommandAsync(this.Client, topic, payload, cancellationToken);
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Subscribe to topics specific to the source.
        /// Topics may include incoming commands, the ability to manually query the source, etc.
        /// </summary>
        private Task HandleSubscribeAsync(CancellationToken cancellationToken = default)
        {
            var topics = this.Liason.Subscriptions();
            return this.SubscribeAsync(topics, cancellationToken);
        }

        /// <summary>
        /// Publish MQTT discovery messages specific to the source.
        /// </summary>
        private Task HandleDiscoveryAsync(CancellationToken cancellationToken = default)
        {
            if (!this.Opts.DiscoveryEnabled)
            {
                return Task.CompletedTask;
            }

            var discoveries = this.Liason.Discoveries();
            var tasks = discoveries.Select(x =>
                this.PublishDiscoveryAsync(x.slug, x.sensor, x.type, x.discovery, cancellationToken)
            );

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Publish discovery messages that indicate a source is available.
        /// </summary>
        private async Task PublishDiscoveryAsync(string slug, string sensor, string sensorType,
            MQTTDiscovery discoveryMsg, CancellationToken cancellationToken = default)
        {
            var sensorName = this.Generator.Stringify(string.Empty, slug, sensor, '_');

            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

            var sw = new StringWriter();
            var writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, discoveryMsg);

            var msg = new MqttApplicationMessageBuilder()
                .WithTopic($"{this.Opts.DiscoveryPrefix}/{sensorType}/{this.Opts.DiscoveryName}/{sensorName}/config")
                .WithPayload(sw.ToString())
                .WithRetainFlag()
                .Build();

            await this.Client.PublishAsync(msg, cancellationToken);
        }

        /// <summary>
        /// Subscribe topics
        /// </summary>
        private async Task SubscribeAsync(IEnumerable<string> filters, CancellationToken cancellationToken = default)
        {
            this.Logger.LogDebug("Started subscribing to topics");
            var topics = filters.Select(topic =>
            {
                this.Logger.LogDebug("Found topic {topic}", topic);
                return new TopicFilterBuilder().WithTopic(topic).Build();
            });

            await this.Client.SubscribeAsync(topics);
            this.Logger.LogDebug("Finished subscribing to topics");
        }

        /// <summary>
        /// Publish topics + payloads, retained, depduplicated.
        /// </summary>
        private async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
        {
            if (this.KnownMessages.ContainsKey(topic) && this.KnownMessages[topic] == payload)
            {
                this.Logger.LogDebug("Duplicate '{payload}' found on '{topic}'", payload, topic);
                return;
            }

            this.Logger.LogInformation("Publishing '{payload}' on '{topic}'", payload, topic);
            await this.Client.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag()
                    .Build(),
                cancellationToken
            );

            this.KnownMessages[topic] = payload;
        }
    }
}