using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Interfaces
{
    /// <summary>
    /// An interface representing a way to liason source data and commands.
    /// </summary>
    /// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
    /// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
    public interface ISourceLiason<TData, TCmd>
        where TData : class
        where TCmd : class
    {
        /// <summary>
        /// Fetch all records from the source.
        /// </summary>
        IAsyncEnumerable<TData?> FetchAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Publish commands to the source.
        /// </summary>
        /// <param name="cancellationToken"></param>
        Task<TCmd?> SendCommandAsync(TCmd item,
            CancellationToken cancellationToken = default) => Task.FromResult<TCmd?>(null);
    }
}