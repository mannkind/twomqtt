using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTnet.Extensions.ManagedClient;
using TwoMQTT.Core;
using TwoMQTT.Core.Models;

namespace TwoMQTTTest
{
    [TestClass]
    public class MQTTManagerTest
    {
        [TestMethod]
        public void TestBooleanOnOff()
        {
            var tests = new[]
            {
                new { Bool = true, Expected = Const.ON },
                new { Bool = false, Expected = Const.OFF },
            };

            var opts = Options.Create(new MQTTManagerOptions { });
            var mqtt = this.ProvisionMQTTManager(opts);
            foreach (var test in tests)
            {
                Assert.AreEqual(test.Expected, mqtt.ExposedBooleanOnOff(test.Bool));
            }
        }

        [TestMethod]
        public void TestAvailabilityTopic()
        {
            var tests = new[]
            {
                new { Opts = Options.Create(new MQTTManagerOptions { }), Expected = "/status" },
                new { Opts = Options.Create(new MQTTManagerOptions { TopicPrefix = "test" }), Expected = "test/status" },
            };

            foreach (var test in tests)
            {
                var mqtt = this.ProvisionMQTTManager(test.Opts);
                Assert.AreEqual(test.Expected, mqtt.ExposedAvailabilityTopic());
            }
        }

        [TestMethod]
        public void TestStateTopic()
        {
            var tests = new[]
            {
                new { Opts = Options.Create(new MQTTManagerOptions { }), Slug = "slug", Sensor = "", Expected = "slug/state" },
                new { Opts = Options.Create(new MQTTManagerOptions { TopicPrefix = "test" }), Slug = "slug", Sensor = "", Expected = "test/slug/state" },
                new { Opts = Options.Create(new MQTTManagerOptions { TopicPrefix = "test" }), Slug = "slug", Sensor = "sensor", Expected = "test/slug/sensor/state" },
            };

            foreach (var test in tests)
            {
                var mqtt = this.ProvisionMQTTManager(test.Opts);
                Assert.AreEqual(test.Expected, mqtt.ExposedStateTopic(test.Slug, test.Sensor));
            }
        }

        [TestMethod]
        public void TestCommandTopic()
        {
            var tests = new[]
            {
                new { Opts = Options.Create(new MQTTManagerOptions { }), Slug = "slug", Sensor = "", Expected = "slug/command" },
                new { Opts = Options.Create(new MQTTManagerOptions { TopicPrefix = "test" }), Slug = "slug", Sensor = "", Expected = "test/slug/command" },
                new { Opts = Options.Create(new MQTTManagerOptions { TopicPrefix = "test" }), Slug = "slug", Sensor = "sensor", Expected = "test/slug/sensor/command" },
            };

            foreach (var test in tests)
            {
                var mqtt = this.ProvisionMQTTManager(test.Opts);
                Assert.AreEqual(test.Expected, mqtt.ExposedCommandTopic(test.Slug, test.Sensor));
            }
        }

        [TestMethod]
        public void TestStringify()
        {
            var tests = new[]
            {
                new { Prefix = "", Slug = "slug", Sensor = "", Sep = '/', Expected = "slug" },
                new { Prefix = "test", Slug = "slug", Sensor = "", Sep = '/', Expected = "test/slug" },
                new { Prefix = "test", Slug = "slug", Sensor = "sensor", Sep = '/', Expected = "test/slug/sensor" },
            };

            var opts = Options.Create(new MQTTManagerOptions { });
            var mqtt = this.ProvisionMQTTManager(opts);
            foreach (var test in tests)
            {
                Assert.AreEqual(test.Expected, mqtt.ExposedStringify(test.Prefix, test.Slug, test.Sensor, test.Sep));
            }
        }

        private StubMQTTManager ProvisionMQTTManager(IOptions<MQTTManagerOptions> opts)
        {
            var logger = new Moq.Mock<ILogger<StubMQTTManager>>();
            var client = new Moq.Mock<IManagedMqttClient>();
            var c = Channel.CreateUnbounded<object>();

            var mqtt = new StubMQTTManager(logger.Object, opts, client.Object, c.Reader, c.Writer);
            return mqtt;
        }
    }
}
