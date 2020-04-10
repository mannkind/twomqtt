using System.Net.Http;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwoMQTT.Core.Communication
{
    public abstract class HTTPManager<TSrc, TData, TCommand> : BackgroundService
    {
        public HTTPManager(ILogger<HTTPManager<TSrc, TData, TCommand>> logger, ChannelWriter<TData> outgoing, ChannelReader<TCommand> incoming, HttpClient client)
        {
            this.logger = logger;
            this.outgoing = outgoing;
            this.incoming = incoming;
            this.client = client;
        }

        /// <summary>
        /// Executed as an IHostedService as a background job.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            this.LogSettings();

            // Listen for incoming messages
            var readChannelTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ReadIncomingAsync(cancellationToken);
                }
            });

            var pollTask = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.PollAsync(cancellationToken);
                    await this.DelayAsync(cancellationToken);
                }
            });

            await Task.WhenAll(readChannelTask, pollTask);
        }

        protected readonly ILogger<HTTPManager<TSrc, TData, TCommand>> logger;
        protected readonly HttpClient client;
        protected readonly ChannelWriter<TData> outgoing;
        protected readonly ChannelReader<TCommand> incoming;

        /// <summary>
        /// Log settings specific to the source client
        /// </summary>
        protected virtual void LogSettings()
        {
        }

        /// <summary>
        /// Poll the source
        /// </summary>
        protected abstract Task PollAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Delay polling the source
        /// </summary>
        protected abstract Task DelayAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Read incoming commands and send them to the source appropriately
        /// </summary>
        protected async Task ReadIncomingAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested && await this.incoming.WaitToReadAsync(cancellationToken))
            {
                var item = await this.incoming.ReadAsync(cancellationToken);
                await this.HandleIncomingAsync(item, cancellationToken);
            }
        }

        /// <summary>
        /// Handle incoming commands
        /// </summary>
        protected virtual Task HandleIncomingAsync(TCommand item, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
