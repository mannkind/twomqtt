using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTnet.Client;
using TwoMQTT.Extensions;

namespace TwoMQTTTest.Extensions;

[TestClass]
public class MqttClientOptionsBuilderExtTest
{
    [TestMethod]
    public void WithCredentialsTest()
    {
        var tests = new[]
        {
                new { HasCredentials = true, Username = "testUsername", Password = "testPassword", },
                new { HasCredentials = true, Username = "testUsername", Password = (string)null, },
                new { HasCredentials = false, Username = "", Password = ""},
        };

        foreach (var test in tests)
        {
            var builder = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost")
                .WithConditionalCredentials(test.HasCredentials, test.Username, test.Password);
            var obj = builder.Build();

            Assert.AreEqual(test.HasCredentials, obj.Credentials != null);

            if (!test.HasCredentials)
            {
                continue;
            }

            var actualPassword = obj.Credentials.GetPassword(obj) == null ? null : System.Text.Encoding.Default.GetString(obj.Credentials.GetPassword(obj));
            Assert.AreEqual(test.Username, obj.Credentials.GetUserName(obj), "Username mismatch");
            Assert.AreEqual(test.Password, actualPassword, "Password mismatch");
        }
    }
}
