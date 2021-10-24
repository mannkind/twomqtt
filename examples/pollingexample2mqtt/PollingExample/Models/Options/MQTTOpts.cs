using TwoMQTT.Models;

namespace PollingExample.Models.Options;

/// <summary>
/// The sink options
/// </summary>
public record MQTTOpts : MQTTManagerOptions
{
    public const string Section = "Polling:MQTT";
    public const string TopicPrefixDefault = "home/polling";
    public const string DiscoveryNameDefault = "polling";
}
