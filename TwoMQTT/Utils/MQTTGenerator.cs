using System.Collections.Generic;
using System.Reflection;

namespace TwoMQTT.Utils;

/// <summary>
/// 
/// </summary>
public class MQTTGenerator : Interfaces.IMQTTGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="topixPrefix"></param>
    /// <param name="discoveryName"></param>
    public MQTTGenerator(string topicPrefix, string discoveryName)
    {
        this.TopicPrefix = topicPrefix;
        this.DiscoveryName = discoveryName;
    }

    /// <inheritdoc />
    public string BooleanOnOff(bool val) => val ? Const.ON : Const.OFF;

    /// <inheritdoc />
    public string BooleanHomeNotHome(bool val) => val ? Const.HOME : Const.NOTHOME;

    /// <inheritdoc />
    public string AvailabilityTopic() => $"{this.TopicPrefix}/status";

    /// <inheritdoc />
    public string StateTopic(string slug, string sensor = "") =>
        $"{string.Join('/', new[] { this.Stringify(this.TopicPrefix, slug, sensor, '/'), "state" })}";


    /// <inheritdoc />
    public string CommandTopic(string slug, string sensor = "") =>
        $"{string.Join('/', new[] { this.Stringify(this.TopicPrefix, slug, sensor, '/'), "command" })}";


    /// <inheritdoc />
    public string Stringify(string prefix, string slug, string sensor, char seperator)
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

    /// <inheritdoc />
    public Models.MQTTDiscovery BuildDiscovery(string slug, string sensor, AssemblyName assembly, bool hasCommand)
    {
        var deviceName = $"{assembly.Name?.ToLower() ?? "unknown"}2mqtt-{slug}";
        return new Models.MQTTDiscovery
        {
            Name = this.Stringify(this.DiscoveryName, slug, sensor, ' '),
            AvailabilityTopic = this.AvailabilityTopic(),
            StateTopic = this.StateTopic(slug, sensor),
            CommandTopic = hasCommand ? this.CommandTopic(slug, sensor) : string.Empty,
            UniqueId = this.Stringify(this.DiscoveryName, slug, sensor, '.'),
            Device = new Models.MQTTDiscovery.DiscoveryDevice
            {
                Identifiers = new List<string> { $"{deviceName}-{this.AvailabilityTopic()}" },
                Name = deviceName,
                SWVersion = $"v{assembly.Version?.ToString() ?? "0.0.0"}",
                Manufacturer = "twomqtt"
            }
        };
    }

    private readonly string TopicPrefix;
    private readonly string DiscoveryName;
}