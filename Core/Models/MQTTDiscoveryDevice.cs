using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace TwoMQTT.Core.Models
{
    public class MQTTDiscoveryDevice
    {
        [JsonProperty("identifiers")]
        [DefaultValue("")]
        public List<string> Identifiers { get; set; } = new List<string>();

        [JsonProperty("connections")]
        [DefaultValue("")]
        public List<string> Connections { get; set; } = new List<string>();

        [JsonProperty("manufacturer")]
        [DefaultValue("")]
        public string Manufacturer { get; set; } = string.Empty;

        [JsonProperty("model")]
        [DefaultValue("")]
        public string Model { get; set; } = string.Empty;

        [JsonProperty("name")]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("sw_version")]
        [DefaultValue("")]
        public string SWVersion { get; set; } = string.Empty;

    }
}