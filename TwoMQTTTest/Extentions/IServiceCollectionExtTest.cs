using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwoMQTT.Extensions;
using TwoMQTT.Interfaces;

namespace TwoMQTTTest.Extensions;

[TestClass]
public class IServiceCollectionExtTest
{
    [TestMethod]
    public void AddOptionsTest()
    {
        var services = new ServiceCollection();
        var root = new Moq.Mock<IConfigurationRoot>();
        var cfg = new Moq.Mock<IConfiguration>();
        cfg.Setup(x => x.GetSection("test")).Returns(new ConfigurationSection(root.Object, "test"));
        services.AddOptions<TestOptions>("test", cfg.Object);

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IOptions<TestOptions>>());
    }

    [TestMethod]
    public void AddIPCTest()
    {
        var services = new ServiceCollection();
        services.AddIPC<object, object>();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IIPC<object, object>>());
    }


    [TestMethod]
    public void AddSourceTest()
    {
        var services = new ServiceCollection();
        services.AddSource<object, object, TestSourceLiason>();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<ISourceLiason<object, object>>());
    }

    [TestMethod]
    public void AddMqttTest()
    {
        var services = new ServiceCollection();
        services.AddMqtt<object, object, TestMQTTLiason>();

        var sp = services.BuildServiceProvider();
        Assert.IsNotNull(sp.GetService<IMQTTLiason<object, object>>());
    }
}


public record TestOptions
{
}

public class TestSourceLiason : TwoMQTT.Interfaces.ISourceLiason<object, object>
{
    public IAsyncEnumerable<object> ReceiveDataAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}

public class TestMQTTLiason : TwoMQTT.Interfaces.IMQTTLiason<object, object>
{
    public IEnumerable<(string topic, string payload)> MapData(object input)
    {
        throw new System.NotImplementedException();
    }
}