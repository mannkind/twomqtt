using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TwoMQTT.Interfaces;

namespace TwoMQTT.Liasons
{
    /// <summary>
    /// Untested, subject to change.
    /// </summary>
    /// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
    public abstract class PushSourceLiasonBase<TData, TResponse, TSource>
        where TData : class
        where TResponse : class
        where TSource: IPushingSourceDAO<TResponse>
    {
        public PushSourceLiasonBase(ILogger<PushSourceLiasonBase<TData, TResponse, TSource>> logger, TSource sourceDAO)
        {
            this.Logger = logger;
            this.SourceDAO = sourceDAO;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async IAsyncEnumerable<TData?> ReceiveDataAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var resp in this.SourceDAO.ReceiveAsync(cancellationToken))
            {
                var data = this.Map(resp);
                yield return data;
            }
        }

        /// <summary>
        /// The logger used internally.
        /// </summary>
        protected readonly ILogger<PushSourceLiasonBase<TData, TResponse, TSource>> Logger;

        protected readonly TSource SourceDAO;

        protected abstract TData? Map(TResponse? response);
    }
}