using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Core.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TQuestion">The type representing the question to ask the source; typically some kind of key/slug pairing.</typeparam>
    /// <typeparam name="TQuestionResponse">The type representing the response from the source system.</typeparam>
    /// <typeparam name="TCmd">The type representing the command to the source system.</typeparam>
    /// <typeparam name="TCmdResponse">The type representing the response to commands from the source system.</typeparam>
    public interface ISourceDAO<TQuestion, TQuestionResponse, TCmd, TCmdResponse>
        where TQuestion : class
        where TQuestionResponse : class
        where TCmd : class
        where TCmdResponse : class
    {
        /// <summary>
        /// Fetch one response from the source.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TQuestionResponse?> FetchOneAsync(TQuestion key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Send one command to the source.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TCmdResponse?> SendOneAsync(TCmd item, CancellationToken cancellationToken = default) =>
            Task.FromResult<TCmdResponse?>(null);
    }
}