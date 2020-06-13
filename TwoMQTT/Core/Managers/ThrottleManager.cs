using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwoMQTT.Core.Utils
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

    /// <summary>
    /// An class representing a managed way to delay polling a source.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public class ThrottleManager : IThrottleManager
    {
        /// <inheritdoc />
        public TimeSpan Interval { get; }

        /// <summary>
        /// Initializes a new instance of the ThrottleManager class.
        /// </summary>
        /// <param name="interval"></param>
        public ThrottleManager(TimeSpan interval)
        {
            this.Interval = interval;
        }
    }
}