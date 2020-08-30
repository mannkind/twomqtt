using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwoMQTT.Core.Interfaces;

namespace TwoMQTT.Core.Liasons
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
    /// <typeparam name="TCommand">The type representing the command to the source system. </typeparam>
    /// <typeparam name="TQuestion">The type representing the question to ask the source; typically some kind of key/slug pairing.</typeparam>
    /// <typeparam name="TSource">The type representing the access to the source system.</typeparam>
    /// <typeparam name="TOpts">The type representing the shared options.</typeparam>
    public abstract class SourceLiasonBase<TData, TCommand, TQuestion, TSource, TOpts>
        where TData : class
        where TCommand : class
        where TQuestion : class
        where TOpts : class, ISharedOpts<TQuestion>, new()
    {
        public SourceLiasonBase(ILogger<SourceLiasonBase<TData, TCommand, TQuestion, TSource, TOpts>> logger, TSource sourceDAO, IOptions<TOpts> sharedOpts)
        {
            this.Logger = logger;
            this.Questions = sharedOpts.Value.Resources;
            this.SourceDAO = sourceDAO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<TData?> FetchAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var key in this.Questions)
            {
                this.Logger.LogDebug("Looking up {key}", key);
                var resp = await this.FetchOneAsync(key, cancellationToken);
                yield return resp;
            }
        }

        protected abstract Task<TData?> FetchOneAsync(TQuestion key, CancellationToken cancellationToken);

        /// <summary>
        /// The logger used internally.
        /// </summary>
        protected readonly ILogger<SourceLiasonBase<TData, TCommand, TQuestion, TSource, TOpts>> Logger;

        /// <summary>
        /// The questions to ask the source (typically some kind of key/slug pairing).
        /// </summary>
        protected readonly IEnumerable<TQuestion> Questions;

        /// <summary>
        /// The dao used to interact with the source.
        /// </summary>
        protected readonly TSource SourceDAO;
    }
}