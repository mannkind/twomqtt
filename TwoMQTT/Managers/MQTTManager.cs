using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using TwoMQTT.Extensions;

namespace TwoMQTT.Managers;

/// <summary>
/// A class representing a managed way to interact with an MQTT broker.
/// </summary>
/// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
/// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
public class MQTTManager<TData, TCmd> : BackgroundService
    where TData : class
    where TCmd : class
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
        ILogger<MQTTManager<TData, TCmd>> logger,
        Interfaces.IIPC<TData, TCmd> ipc,
        IManagedMqttClient client,
        Interfaces.IMQTTGenerator generator,
        Interfaces.IMQTTLiason<TData, TCmd> liason,
        IOptions<Models.MQTTManagerOptions> opts
    )
    {
        this.Logger = logger;
        this.IPC = ipc;
        this.Client = client;
        this.Generator = generator;
        this.Liason = liason;
        this.Opts = opts.Value;
        this.Logger.LogInformation(
            "Broker:            {broker}\n" +
            "Username:          {username}\n" +
            "Password:          {password}\n" +
            "TopicPrefix:       {topicPrefix}\n" +
            "DiscoveryEnabled:  {discoveryEnabled}\n" +
            "DiscoveryPrefix:   {discoveryPrefix}\n" +
            "DiscoveryName:     {discoveryName}\n",
            opts.Value.Broker,
            opts.Value.Username,
            !string.IsNullOrEmpty(opts.Value.Password) ? "<REDACTED>" : string.Empty,
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
        await Task.WhenAll(
            MQTTSetupAsync(cancellationToken),
            ReadIncomingAsync(cancellationToken)
        );
    }

    /// <summary>
    /// The logger used internally.
    /// </summary>
    private readonly ILogger<MQTTManager<TData, TCmd>> Logger;

    /// <summary>
    /// The IPC used internally.
    /// </summary>
    private readonly Interfaces.IIPC<TData, TCmd> IPC;

    /// <summary>
    /// The mqtt liason.
    /// </summary>
    private readonly Interfaces.IMQTTLiason<TData, TCmd> Liason;

    /// <summary>
    /// The options required to communicate properly with MQTT.
    /// </summary>
    private readonly Models.MQTTManagerOptions Opts;

    /// <summary>
    /// The MQTT client used to access the the MQTT broker.
    /// </summary>
    protected readonly IManagedMqttClient Client;

    /// <summary>
    /// The MQTT generator used for things such as availability topic, state topic, command topic, etc.
    /// </summary>
    private readonly Interfaces.IMQTTGenerator Generator;

    /// <summary>
    /// The cache of known published messages; used to not continually publish duplicate messages.
    /// </summary>
    private readonly ConcurrentDictionary<string, string> KnownMessages = new ConcurrentDictionary<string, string>();

    private readonly int StartedDelayMS = 503;

    /// <summary>
    /// Read incoming messages.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task ReadIncomingAsync(CancellationToken cancellationToken)
    {
        this.Logger.LogInformation("Awaiting incoming data");
        while (!this.Client.IsStarted)
        {
            this.Logger.LogDebug("Waiting a moment for the MQTT client to start");
            await Task.Delay(this.StartedDelayMS);
        }

        await this.IPC.ReadAsync(async item =>
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
        }, cancellationToken);
        this.Logger.LogInformation("Finished awaiting incoming data");
    }

    private async Task MQTTSetupAsync(CancellationToken cancellationToken)
    {
        this.SetupConnectedCallback(cancellationToken);
        this.SetupDisconnectedCallback(cancellationToken);
        this.SetupMessageReceivedCallback(cancellationToken);
        await this.ConnectAsync(cancellationToken);
    }

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
                    .WithConditionalCredentials(!string.IsNullOrEmpty(Opts.Username), Opts.Username, Opts.Password)
                    .WithWillTopic(this.Generator.AvailabilityTopic())
                    .WithWillPayload(Const.OFFLINE)
                    .WithWillRetain(true)
                .Build()
            )
            .Build()
        );
    }

    /// <summary>
    /// Setup the callback for connecting to the MQTT broker.
    /// </summary>
    /// <returns></returns>
    private void SetupConnectedCallback(CancellationToken cancellationToken = default)
    {
        this.Client.ConnectedAsync += async e =>
        {
            this.Logger.LogInformation("Connected to MQTT");

            await Task.WhenAll(
                this.HandleSubscribeAsync(cancellationToken),
                this.HandleDiscoveryAsync(cancellationToken),
                this.Client.EnqueueAsync(this.GenerateAvailabilityMessage(Const.ONLINE))
            );
        };
    }

    /// <summary>
    /// Setup the callback for connecting to the MQTT broker.
    /// </summary>
    /// <returns></returns>
    private void SetupDisconnectedCallback(CancellationToken cancellationToken = default)
    {
        this.Client.DisconnectedAsync += async e =>
        {
            this.Logger.LogInformation("Disconnected from MQTT: {reason}", e.ReasonString);
            await Task.CompletedTask;
        };
    }

    /// <summary>
    /// Setup the callback for receiving messages from the MQTT broker.
    /// </summary>
    /// <returns></returns>
    private void SetupMessageReceivedCallback(CancellationToken cancellationToken = default)
    {
        this.Client.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = e.ApplicationMessage.ConvertPayloadToString();

            // Another instance went offline; republish that we're still online
            if (topic == this.Generator.AvailabilityTopic() && payload == Const.OFFLINE)
            {
                await this.Client.EnqueueAsync(this.GenerateAvailabilityMessage(Const.ONLINE));
                return;
            }

            // Handle commands
            var cmds = this.Liason.MapCommand(topic, payload);
            var tasks = cmds.Select(cmd =>
                this.IPC.WriteAsync(cmd, cancellationToken).AsTask()
            );

            await Task.WhenAll(tasks);
            await this.Liason.HandleCommandAsync(this.Client, topic, payload, cancellationToken);
        };
    }

    /// <summary>
    /// Subscribe to topics specific to the source.
    /// Topics may include incoming commands, the ability to manually query the source, etc.
    /// </summary>
    private Task HandleSubscribeAsync(CancellationToken cancellationToken = default)
    {
        var topics = this.Liason.Subscriptions()
            .Concat(new[] { this.Generator.AvailabilityTopic() });
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
        Models.MQTTDiscovery discoveryMsg, CancellationToken cancellationToken = default)
    {
        var sensorName = this.Generator.Stringify(string.Empty, slug, sensor, '_');

        // @TODO(mannkind) - How to make this happen automatically?
        if (!discoveryMsg.Options.Any())
        {
            discoveryMsg = discoveryMsg with
            {
                Options = null!,
            };
        }

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

        await this.Client.EnqueueAsync(msg);
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
            return new MqttTopicFilterBuilder().WithTopic(topic).Build();
        }).ToList();

        await this.Client.SubscribeAsync(topics);
        this.Logger.LogDebug("Finished subscribing to topics");
    }

    /// <summary>
    /// Publish topics + payloads, retained, depduplicated.
    /// </summary>
    private async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken = default)
    {
        if (Opts.PublishDeduplicate && this.KnownMessages.ContainsKey(topic) && this.KnownMessages[topic] == payload)
        {
            this.Logger.LogDebug("Duplicate '{payload}' found on '{topic}'", payload, topic);
            return;
        }

        this.Logger.LogInformation("Publishing '{payload}' on '{topic}'", payload, topic);
        await this.Client.EnqueueAsync(
            new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag()
                .Build()
        );

        this.KnownMessages[topic] = payload;
    }

    private MqttApplicationMessage GenerateAvailabilityMessage(string state) =>
            new MqttApplicationMessageBuilder()
                .WithTopic(this.Generator.AvailabilityTopic())
                .WithPayload(state)
                .WithRetainFlag()
                .Build();
}
