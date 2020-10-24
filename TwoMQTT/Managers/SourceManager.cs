using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwoMQTT.Managers
{
    /// <summary>
    /// An class representing a managed way to interact with a source.
    /// </summary>
    /// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
    /// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
    public class SourceManager<TData, TCmd> : BackgroundService
        where TData : class
        where TCmd : class
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
            ILogger<SourceManager<TData, TCmd>> logger,
            Interfaces.IIPC<TCmd, TData> ipc,
            Interfaces.ISourceLiason<TData, TCmd> liason,
            Interfaces.IThrottleManager throttler)
        {
            this.Logger = logger;
            this.IPC = ipc;
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
            await Task.WhenAll(
                this.ReadIncomingAsync(cancellationToken),
                this.PollingSourceAsync(cancellationToken)
            );
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        private readonly ILogger<SourceManager<TData, TCmd>> Logger;

        /// <summary>
        /// The IPC used internally.
        /// </summary>
        private readonly Interfaces.IIPC<TCmd, TData> IPC;

        /// <summary>
        /// The liason to the source system.
        /// </summary>
        private readonly Interfaces.ISourceLiason<TData, TCmd> Liason;

        /// <summary>
        /// The throttler used to delay polling the source system.
        /// </summary>
        private readonly Interfaces.IThrottleManager Throttler;

        /// <summary>
        /// Read incoming commands.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadIncomingAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Awaiting incoming commands");
            await this.IPC.ReadAsync(async item =>
            {
                this.Logger.LogDebug("Received incoming command {item}", item);
                await this.Liason.SendCommandAsync(item, cancellationToken);
            }, cancellationToken);
            this.Logger.LogInformation("Finished awaiting incoming commands");
        }

        /// <summary>
        /// Poll the source system.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task PollingSourceAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Polling source");
            while (!cancellationToken.IsCancellationRequested)
            {
                await this.PollAsync(cancellationToken);
                await this.DelayAsync(cancellationToken);
            }
            this.Logger.LogInformation("Finished polling source");
        }

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
                tasks.Add(this.IPC.WriteAsync(result, cancellationToken).AsTask());
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
