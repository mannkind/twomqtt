using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Core.Interfaces
{
    /// <summary>
    /// An interface representing a way to liason source data and commands.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public interface ISourceLiason<TData, TCommand>
        where TData : class
        where TCommand : class
    {
        /// <summary>
        /// Fetch all records from the source.
        /// </summary>
        IAsyncEnumerable<TData?> FetchAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Publish commands to the source.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<TCommand?> SendCommandAsync(TCommand item,
            CancellationToken cancellationToken = default) => Task.FromResult<TCommand?>(null);
    }
}