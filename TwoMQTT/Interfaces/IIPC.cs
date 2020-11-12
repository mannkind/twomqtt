using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Interfaces
{
    /// <summary>
    /// An interface representing a way to handle IPC.
    /// </summary>
    /// <typeparam name="TIncoming">The type representing the incoming data.</typeparam>
    /// <typeparam name="TOutgoing">The type representing the outgoing data.</typeparam>
    public interface IIPC<TIncoming, TOutgoing>
        where TIncoming : class
        where TOutgoing : class
    {
        /// <summary>
        /// Read from the incoming IPC channel.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ReadAsync(Func<TIncoming, Task> handler, CancellationToken cancellationToken = default);

        /// <summary>
        /// Write to the outgoing IPC channel.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask WriteAsync(TOutgoing obj, CancellationToken cancellationToken = default);
    }
}