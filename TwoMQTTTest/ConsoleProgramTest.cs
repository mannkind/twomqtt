using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwoMQTT;
using TwoMQTT.Interfaces;
using TwoMQTT.Managers;

namespace TwoMQTTTest;

[TestClass]
public class ConsoleProgramTest
{
    [TestMethod]
    public void TestPrintVersion()
    {
        var env = "TOTESANENVVARIABLE";
        var tests = new[]
        {
                new { args = new string[] {}, Expected = true },
                new { args = new string[] { "test", "version" }, Expected = false }
            };

        foreach (var test in tests)
        {
            Environment.SetEnvironmentVariable(env, string.Empty);

            _ = ConsoleProgram<object, object, TestSourceLiason, TestMqttLiason>
                .ExecuteAsync(test.args,
                    envs: new Dictionary<string, string> { { env, env } }
                );
            var actual = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(env) ?? string.Empty);

            Assert.AreEqual(test.Expected, actual);
        }
    }
}

public class TestSourceLiason : ISourceLiason<object, object>
{
    public IAsyncEnumerable<object> ReceiveDataAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
public class TestMqttLiason : IMQTTLiason<object, object>
{
    public IEnumerable<(string topic, string payload)> MapData(object input)
    {
        throw new NotImplementedException();
    }
}
