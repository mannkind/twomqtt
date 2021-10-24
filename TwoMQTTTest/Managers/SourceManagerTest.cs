using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwoMQTT.Interfaces;
using TwoMQTT.Managers;


namespace TwoMQTTTest.Managers;

[TestClass]
public class SourceManagerTest
{
    [TestMethod]
    public async Task IntegationTest()
    {
        var logger = new Moq.Mock<ILogger<SourceManager<object, object>>>().Object;
        var outgoingData = Channel.CreateUnbounded<object>();
        var incomingCommand = Channel.CreateUnbounded<object>();
        var ipc = new IPCManager<object, object>(incomingCommand, outgoingData);
        var liason = new Moq.Mock<ISourceLiason<object, object>>();
        var throttleManager = new Moq.Mock<IThrottleManager>();
        var obj = new SourceManager<object, object>(logger, ipc, liason.Object, throttleManager.Object);
        var ct = new CancellationToken();

        await obj.StartAsync(ct);
        await incomingCommand.Writer.WriteAsync(new object { });
        await Task.Delay(5000);
        await obj.StopAsync(ct);

        liason.Verify(x => x.ReceiveDataAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        liason.Verify(x => x.ReceiveDataAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        throttleManager.Verify(x => x.DelayAsync(It.IsAny<CancellationToken>()), Times.AtMostOnce());
    }
}
