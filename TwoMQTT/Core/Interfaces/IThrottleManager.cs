using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Core.Interfaces
{
    /// <summary>
    /// An interface to throttle access to the source.
    /// </summary>
    public interface IThrottleManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        TimeSpan Interval { get; }

        /// <summary>
        /// Delay polling the source.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DelayAsync(CancellationToken cancellationToken = default) =>
            this.DelayAsync(this.Interval, cancellationToken);

        /// <summary>
        /// Delay polling the source.
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DelayAsync(TimeSpan interval, CancellationToken cancellationToken = default) =>
            Task.Delay(this.Interval, cancellationToken);
    }
}