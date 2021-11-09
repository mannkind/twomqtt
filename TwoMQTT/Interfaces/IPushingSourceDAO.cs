using System.Collections.Generic;
using System.Threading;

namespace TwoMQTT.Interfaces;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TData">The type representing the response from the source system.</typeparam>
public interface IPushingSourceDAO<TData>
    where TData : class
{
    /// <summary>
    /// Fetch one response from the source.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<TData?> ReceiveAsync(CancellationToken cancellationToken = default);
}
