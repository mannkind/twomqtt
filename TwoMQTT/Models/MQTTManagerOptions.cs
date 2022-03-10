using System;

namespace TwoMQTT.Models;

/// <summary>
/// An class representing options required to communicate properly with MQTT.
/// </summary>
public record MQTTManagerOptions
{
    public string Broker { get; init; } = DEFAULTBROKER;
    public string Username { get; init; } = string.Empty;
    public string? Password { get; init; } = null;
    public string TopicPrefix { get; init; } = string.Empty;
    public bool DiscoveryEnabled { get; init; } = true;
    public string DiscoveryPrefix { get; init; } = DEFAULTDISCOVERYPREFIX;
    public string DiscoveryName { get; init; } = string.Empty;
    public bool PublishDeduplicate { get; init; } = true;
    private const string DEFAULTBROKER = "test.mosquitto.org";
    private const string DEFAULTDISCOVERYPREFIX = "homeassistant";
}
