using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwoMQTT.Liasons
{
    /// <summary>
    /// Backwards compatibility; will be removed soon.
    /// </summary>
    [Obsolete("Use PollingSourceLiasonBase instead. Changes required: Remove type declaration TCommand, rename FetchAllAsync to ReceiveDataAsync.")]
    public abstract class SourceLiasonBase<TData, TCommand, TQuestion, TSource, TOpts> : PollingSourceLiasonBase<TData, TQuestion, TSource, TOpts>
        where TData : class
        where TCommand : class
        where TQuestion : class
        where TOpts : class, Interfaces.ISharedOpts<TQuestion>, new()
    {
        public SourceLiasonBase(ILogger<PollingSourceLiasonBase<TData, TQuestion, TSource, TOpts>> logger, TSource sourceDAO, IOptions<TOpts> sharedOpts) :
            base(logger, sourceDAO, sharedOpts)
        {
        }

        public IAsyncEnumerable<object?> FetchAllAsync(CancellationToken cancellationToken = default) =>
            this.ReceiveDataAsync(cancellationToken);
    }
}