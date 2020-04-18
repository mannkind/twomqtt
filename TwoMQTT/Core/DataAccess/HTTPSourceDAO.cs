using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TwoMQTT.Core.DataAccess
{
    /// <summary>
    /// An abstract class representing a barebones way to interact with a source via HTTP.
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TFetchResponse"></typeparam>
    /// <typeparam name="TSendResponse"></typeparam>
    public abstract class HTTPSourceDAO<TQuestion, TCommand, TFetchResponse, TSendResponse> :
        IHTTPSourceDAO<TQuestion, TCommand, TFetchResponse, TSendResponse>
        where TFetchResponse : class
        where TSendResponse : class
    {
        /// <summary>
        /// Initializes a new instance of the HTTPSourceDAO class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public HTTPSourceDAO(ILogger<HTTPSourceDAO<TQuestion, TCommand, TFetchResponse, TSendResponse>> logger,
            IHttpClientFactory httpClientFactory)
        {
            this.Logger = logger;
            this.Client = httpClientFactory.CreateClient();
        }

        /// <inheritdoc />
        public abstract Task<TFetchResponse?> FetchOneAsync(TQuestion key,
            CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public virtual Task<TSendResponse?> SendOneAsync(TCommand item,
            CancellationToken cancellationToken = default) => Task.FromResult(default(TSendResponse));

        /// <summary>
        /// The logger used internally.
        /// </summary>
        protected readonly ILogger<HTTPSourceDAO<TQuestion, TCommand, TFetchResponse, TSendResponse>> Logger;

        /// <summary>
        /// The HTTP client used to access the source.
        /// </summary>
        protected readonly HttpClient Client;

    }
}