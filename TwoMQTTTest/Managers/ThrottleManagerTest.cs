using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwoMQTT.Interfaces;
using TwoMQTT.Managers;

namespace TwoMQTTTest.Managers
{
    [TestClass]
    public class ThrottleManagerTest
    {
        [TestMethod]
        public async Task TestDelay()
        {
            var tests = new[]
            {
                new { Interval = new TimeSpan(0, 0, 8), Seconds = 5 },
            };

            foreach (var test in tests)
            {
                var obj = this.ProvisionThrottleManager(test.Interval);
                var sw = new Stopwatch();
                sw.Start();
                await obj.DelayAsync();
                sw.Stop();

                Assert.IsTrue(sw.Elapsed.Seconds >= test.Seconds, $"Is {sw.Elapsed.Seconds}s > {test.Seconds}s");
            }
        }

        private IThrottleManager ProvisionThrottleManager(TimeSpan t)
        {
            var tm = new ThrottleManager(t);

            return tm;
        }
    }
}
