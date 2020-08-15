using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Utils;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public class SourceManager<TData, TCommand> : BackgroundService
        where TData : class
        where TCommand : class
    {
        /// <summary>
        /// Initializes a new instance of the SourceManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="outgoingData"></param>
        /// <param name="incomingCommand"></param>
        /// <param name="liason"></param>
        /// <param name="throttler"></param>
        public SourceManager(
            ILogger<SourceManager<TData, TCommand>> logger,
            ChannelWriter<TData> outgoingData,
            ChannelReader<TCommand> incomingCommand,
            ISourceLiason<TData, TCommand> liason,
            IThrottleManager throttler)
        {
            this.Logger = logger;
            this.OutgoingData = outgoingData;
            this.IncomingCommands = incomingCommand;
            this.Liason = liason;
            this.Throttler = throttler;
        }

        /// <summary>
        /// Executed as an IHostedService as a background job.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // Listen for incoming messages
            var readChannelTask = Task.Run(async () =>
            {
                this.Logger.LogInformation("Awaiting incoming commands");
                await foreach (var item in this.IncomingCommands.ReadAllAsync(cancellationToken))
                {
                    this.Logger.LogDebug("Received incoming command {item}", item);
                    await this.Liason.SendCommandAsync(item, cancellationToken);
                }
                this.Logger.LogInformation("Finished awaiting incoming commands");
            });

            var pollTask = Task.Run(async () =>
            {
                this.Logger.LogInformation("Polling source");
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.PollAsync(cancellationToken);
                    await this.DelayAsync(cancellationToken);
                }
                this.Logger.LogInformation("Finished polling source");
            });

            await Task.WhenAll(readChannelTask, pollTask);
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        private readonly ILogger<SourceManager<TData, TCommand>> Logger;

        /// <summary>
        /// The channel writer used to communicate data from the source.
        /// </summary>
        private readonly ChannelWriter<TData> OutgoingData;

        /// <summary>
        /// The channel reader used to communicate commands to the source.
        /// </summary>
        private readonly ChannelReader<TCommand> IncomingCommands;

        /// <summary>
        /// The liason to the source system.
        /// </summary>
        private readonly ISourceLiason<TData, TCommand> Liason;
        private readonly IThrottleManager Throttler;

        /// <summary>
        /// Poll the source.
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async Task PollAsync(CancellationToken cancellationToken = default)
        {
            this.Logger.LogDebug("Started Polling");
            var tasks = new List<Task>();
            await foreach (var result in this.Liason.FetchAllAsync(cancellationToken))
            {
                if (result == null)
                {
                    continue;
                }

                this.Logger.LogDebug("Found {result}", result);
                tasks.Add(this.OutgoingData.WriteAsync(result, cancellationToken).AsTask());
            }
            await Task.WhenAll(tasks);
            this.Logger.LogDebug("Finished Polling");
        }

        /// <summary>
        /// Delay polling the source.
        /// </summary>
        private Task DelayAsync(CancellationToken cancellationToken = default) =>
            this.Throttler.DelayAsync(cancellationToken);
    }
}
