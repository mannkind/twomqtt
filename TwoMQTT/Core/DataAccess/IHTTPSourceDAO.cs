using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Core.DataAccess
{
    /// <summary>
    /// An interface representing a barebones way to interact with a source via HTTP.
    /// </summary>
    /// <typeparam name="TQuestion"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TFetchResponse"></typeparam>
    /// <typeparam name="TSendResponse"></typeparam>
    public interface IHTTPSourceDAO<TQuestion, TCommand, TFetchResponse, TSendResponse>
        where TFetchResponse : class
        where TSendResponse : class
    {
        /// <summary>
        /// Fetch one response from the source.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TFetchResponse?> FetchOneAsync(TQuestion key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send one request to the source.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TSendResponse?> SendOneAsync(TCommand item, CancellationToken cancellationToken = default);
    }
}