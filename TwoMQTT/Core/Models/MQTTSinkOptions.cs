namespace TwoMQTT.Core.Models
{
    public class MQTTManagerOptions
    {
        public string Broker { get; set; } = "test.mosquitto.org";
        public string TopicPrefix { get; set; } = string.Empty;
        public bool DiscoveryEnabled { get; set; } = true;
        public string DiscoveryPrefix { get; set; } = "homeassistant";
        public string DiscoveryName { get; set; } = string.Empty;
    }
}
