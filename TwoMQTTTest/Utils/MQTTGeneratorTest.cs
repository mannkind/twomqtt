using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwoMQTT.Core;
using TwoMQTT.Core.Utils;

namespace TwoMQTTTest.Utils
{
    [TestClass]
    public class MQTTGeneratorTest
    {
        [TestMethod]
        public void TestBooleanOnOff()
        {
            var tests = new[]
            {
                new { Bool = true, Expected = Const.ON },
                new { Bool = false, Expected = Const.OFF },
            };

            var obj = this.ProvisionMQTTGenerator(string.Empty);
            foreach (var test in tests)
            {
                Assert.AreEqual(test.Expected, obj.BooleanOnOff(test.Bool));
            }
        }

        [TestMethod]
        public void TestAvailabilityTopic()
        {
            var tests = new[]
            {
                new { TopicPrefix = string.Empty, Expected = "/status" },
                new { TopicPrefix = "test" , Expected = "test/status" },
            };

            foreach (var test in tests)
            {
                var obj = this.ProvisionMQTTGenerator(test.TopicPrefix);
                Assert.AreEqual(test.Expected, obj.AvailabilityTopic());
            }
        }

        [TestMethod]
        public void TestStateTopic()
        {
            var tests = new[]
            {
                new { TopicPrefix = string.Empty, Slug = "slug", Sensor = "", Expected = "slug/state" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "", Expected = "test/slug/state" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "sensor", Expected = "test/slug/sensor/state" },
            };

            foreach (var test in tests)
            {
                var obj = this.ProvisionMQTTGenerator(test.TopicPrefix);
                Assert.AreEqual(test.Expected, obj.StateTopic(test.Slug, test.Sensor));
            }
        }

        [TestMethod]
        public void TestCommandTopic()
        {
            var tests = new[]
            {
                new { TopicPrefix = string.Empty, Slug = "slug", Sensor = "", Expected = "slug/command" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "", Expected = "test/slug/command" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "sensor", Expected = "test/slug/sensor/command" },
            };

            foreach (var test in tests)
            {
                var obj = this.ProvisionMQTTGenerator(test.TopicPrefix);
                Assert.AreEqual(test.Expected, obj.CommandTopic(test.Slug, test.Sensor));
            }
        }

        [TestMethod]
        public void TestStringify()
        {
            var tests = new[]
            {
                new { TopicPrefix = "", Slug = "slug", Sensor = "", Sep = '/', Expected = "slug" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "", Sep = '/', Expected = "test/slug" },
                new { TopicPrefix = "test", Slug = "slug", Sensor = "sensor", Sep = '/', Expected = "test/slug/sensor" },
            };

            foreach (var test in tests)
            {
                var obj = this.ProvisionMQTTGenerator(test.TopicPrefix);
                Assert.AreEqual(test.Expected, obj.Stringify(test.TopicPrefix, test.Slug, test.Sensor, test.Sep));
            }
        }

        private MQTTGenerator ProvisionMQTTGenerator(string topicPrefix, string discoveryName = "")
        {
            var msm = new MQTTGenerator(topicPrefix, discoveryName);

            return msm;
        }
    }
}
