using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// An abstract class representing a managed way to poll a source.
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TSourceFetchResponse"></typeparam>
    /// <typeparam name="TSourceSendResponse"></typeparam>
    /// <typeparam name="TSharedData"></typeparam>
    /// <typeparam name="TSharedCommand"></typeparam>
    public abstract class PollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand> : BackgroundService
        where TSourceFetchResponse : class
        where TSourceSendResponse : class
    {
        /// <summary>
        /// Initializes a new instance of the PollingManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="outgoingData"></param>
        /// <param name="incomingCommand"></param>
        /// <param name="questions"></param>
        /// <param name="pollingInterval"></param>
        /// <param name="internalSettings"></param>
        public PollingManager(ILogger<PollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand>> logger,
            ChannelWriter<TSharedData> outgoingData, ChannelReader<TSharedCommand> incomingCommand,
            IEnumerable<TQuestion> questions, TimeSpan pollingInterval, string internalSettings)
        {
            this.Logger = logger;
            this.OutgoingData = outgoingData;
            this.IncomingCommands = incomingCommand;
            this.Questions = questions;
            this.PollingInterval = pollingInterval;
            this.Logger.LogInformation(internalSettings ?? string.Empty);
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        protected readonly ILogger<PollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand>> Logger;

        /// <summary>
        /// The channel writer used to communicate data from the source.
        /// </summary>
        protected readonly ChannelWriter<TSharedData> OutgoingData;

        /// <summary>
        /// The channel reader used to communicate commands to the source.
        /// </summary>
        protected readonly ChannelReader<TSharedCommand> IncomingCommands;

        /// <summary>
        /// The questions to ask the source (typically some kind of key/slug pairing).
        /// </summary>
        protected readonly IEnumerable<TQuestion> Questions;

        /// <summary>
        /// The interval at which to poll the source.
        /// </summary>
        protected readonly TimeSpan PollingInterval;

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
                while (!cancellationToken.IsCancellationRequested)
                {
                    await this.ReadIncomingCommandAsync(cancellationToken);
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
        /// Read incoming commands and send them to the source appropriately.
        /// </summary>
        protected async Task ReadIncomingCommandAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested &&
                await this.IncomingCommands.WaitToReadAsync(cancellationToken))
            {
                var item = await this.IncomingCommands.ReadAsync(cancellationToken);
                await this.HandleIncomingCommandAsync(item, cancellationToken);
            }
        }

        /// <summary>
        /// Publish commands to the source.
        /// </summary>
        protected abstract Task HandleIncomingCommandAsync(TSharedCommand item,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Poll the source.
        /// </summary>
        protected virtual async Task PollAsync(CancellationToken cancellationToken = default)
        {
            var tasks = new List<Task<TSourceFetchResponse?>>();
            foreach (var key in this.Questions)
            {
                this.Logger.LogInformation($"Looking up {key}");
                tasks.Add(this.FetchOneAsync(key, cancellationToken));
            }

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                if (result == null)
                {
                    continue;
                }

                this.Logger.LogInformation($"Found {result}");
                await this.OutgoingData.WriteAsync(this.MapResponse(result), cancellationToken);
            }
        }

        /// <summary>
        /// Delay polling the source.
        /// </summary>
        protected virtual Task DelayAsync(CancellationToken cancellationToken = default) =>
            Task.Delay(this.PollingInterval, cancellationToken);

        /// <summary>
        /// Fetch one record from the source.
        /// </summary>
        protected abstract Task<TSourceFetchResponse?> FetchOneAsync(TQuestion key,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Map the source response into the shared data model.
        /// </summary>
        protected abstract TSharedData MapResponse(TSourceFetchResponse src);
    }
}
