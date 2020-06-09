using System.Collections.Generic;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Extensions.ManagedClient;
using TwoMQTT.Core.Managers;
using TwoMQTT.Core.Models;

namespace TwoMQTTTest
{
    public class StubMQTTManager : MQTTManager<object, object, object>
    {
        public StubMQTTManager(ILogger<StubMQTTManager> logger, IOptions<MQTTManagerOptions> opts,
            IManagedMqttClient client, ChannelReader<object> incomingData, ChannelWriter<object> outgoingCommand) :
            base(logger, opts, client, incomingData, outgoingCommand, new List<object>(), string.Empty)
        {
        }
        public string ExposedBooleanOnOff(bool val) => this.BooleanOnOff(val);
        public string ExposedAvailabilityTopic() => this.AvailabilityTopic();
        public string ExposedStateTopic(string slug, string sensor = "") => this.StateTopic(slug, sensor);
        public string ExposedCommandTopic(string slug, string sensor = "") => this.CommandTopic(slug, sensor);
        public string ExposedStringify(string prefix, string slug, string sensor, char seperator) =>
            this.Stringify(prefix, slug, sensor, seperator);
    }
}
