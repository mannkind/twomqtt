using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;

namespace TwoMQTT.Interfaces;

/// <summary>
/// An interface representing a way to liason subscriptions, discovery, and more.
/// </summary>
/// <typeparam name="TData">The type representing the data from the source system.</typeparam>
/// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
public interface IMQTTLiason<TData, TCmd>
    where TData : class
    where TCmd : class
{
    /// <summary>
    /// Map incoming data to an MQTT topic/payload.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IEnumerable<(string topic, string payload)> MapData(TData input);

    /// <summary>
    /// Map an MQTT topic/payload to a command for the source system.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IEnumerable<TCmd> MapCommand(string topic, string payload) => new List<TCmd>();

    /// <summary>
    /// Manually handle an MQTT topic/payload.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleCommandAsync(IManagedMqttClient client, string topic, string payload, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    /// <summary>
    /// A list of topics to subscribe.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    IEnumerable<string> Subscriptions() => new List<string>();

    /// <summary>
    /// A list of discoveries to publish.
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="string"></typeparam>
    /// <typeparam name="MQTTDiscovery"></typeparam>
    IEnumerable<(string slug, string sensor, string type, Models.MQTTDiscovery discovery)> Discoveries() =>
        new List<(string, string, string, Models.MQTTDiscovery)>();
}
