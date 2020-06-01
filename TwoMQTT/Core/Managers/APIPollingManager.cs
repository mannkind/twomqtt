using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TwoMQTT.Core.DataAccess;

namespace TwoMQTT.Core.Managers
{
    /// <summary>
    /// An abstract class representing a managed way to poll an source via an API.
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TSourceFetchResponse"></typeparam>
    /// <typeparam name="TSourceSendResponse"></typeparam>
    /// <typeparam name="TSharedData"></typeparam>
    /// <typeparam name="TSharedCommand"></typeparam>
    public abstract class APIPollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand> :
        PollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand>
        where TSourceFetchResponse : class
        where TSourceSendResponse : class
    {
        /// <summary>
        /// Initializes a new instance of the HTTPPollingManager class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="outgoingData"></param>
        /// <param name="incomingCommand"></param>
        /// <param name="questions"></param>
        /// <param name="pollingInterval"></param>
        /// <param name="sourceDAO"></param>
        /// <param name="internalSettings"></param>
        /// <returns></returns>
        public APIPollingManager(ILogger<APIPollingManager<TQuestion, TSourceFetchResponse, TSourceSendResponse, TSharedData, TSharedCommand>> logger,
            ChannelWriter<TSharedData> outgoingData, ChannelReader<TSharedCommand> incomingCommand,
            IEnumerable<TQuestion> questions, TimeSpan pollingInterval,
            ISourceDAO<TQuestion, TSharedCommand, TSourceFetchResponse, TSourceSendResponse> sourceDAO, string internalSettings) :
            base(logger, outgoingData, incomingCommand, questions, pollingInterval, internalSettings)
        {
            this.SourceDAO = sourceDAO;
        }

        /// <summary>
        /// The DAO for interacting with the source.
        /// </summary>
        protected readonly ISourceDAO<TQuestion, TSharedCommand, TSourceFetchResponse, TSourceSendResponse> SourceDAO;

        /// <summary>
        /// Send commands to the source.
        /// </summary>
        protected override Task HandleIncomingCommandAsync(TSharedCommand item,
            CancellationToken cancellationToken = default) => this.SourceDAO.SendOneAsync(item, cancellationToken);

        /// <summary>
        /// Fetch one record from the source.
        /// </summary>
        protected override Task<TSourceFetchResponse?> FetchOneAsync(TQuestion key,
            CancellationToken cancellationToken = default) => this.SourceDAO.FetchOneAsync(key, cancellationToken);
    }
}
