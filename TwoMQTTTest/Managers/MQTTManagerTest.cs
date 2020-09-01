using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading;
using Moq;
using TwoMQTT.Core.Utils;
using MQTTnet.Extensions.ManagedClient;
using Microsoft.Extensions.Options;
using TwoMQTT.Core.Models;
using System.Collections.Generic;

namespace TwoMQTTTest.Managers
{
    [TestClass]
    public class MQTTManagerTest
    {
        [TestMethod]
        public async Task IntegrationTest()
        {
            var logger = new Moq.Mock<ILogger<MQTTManager<object, object>>>().Object;
            var incomingData = Channel.CreateUnbounded<object>();
            var outgoingCommand = Channel.CreateUnbounded<object>();
            var client = new Moq.Mock<IManagedMqttClient>();
            var generator = new Moq.Mock<IMQTTGenerator>();
            var liason = new Moq.Mock<IMQTTLiason<object, object>>();
            var opts = Options.Create(new MQTTManagerOptions());
            var obj = new MQTTManager<object, object>(logger, incomingData, outgoingCommand, client.Object, generator.Object, liason.Object, opts);

            var ct = new CancellationToken();

            await obj.StartAsync(ct);
            await incomingData.Writer.WriteAsync(new object { });
            await Task.Delay(5000);
            await obj.StopAsync(ct);

            client.Verify(x => x.StartAsync(It.IsAny<IManagedMqttClientOptions>()), Times.AtMostOnce());
            client.Verify(x => x.SubscribeAsync(It.IsAny<IEnumerable<MQTTnet.TopicFilter>>()), Times.AtMostOnce());
            client.Verify(x => x.PublishAsync(It.IsAny<MQTTnet.MqttApplicationMessage>(), It.IsAny<CancellationToken>()), Times.AtMostOnce());
            liason.Verify(x => x.Discoveries(), Times.AtMostOnce());
            liason.Verify(x => x.Subscriptions(), Times.AtMostOnce());
            generator.Verify(x => x.AvailabilityTopic(), Times.AtMostOnce());
        }
    }
}
