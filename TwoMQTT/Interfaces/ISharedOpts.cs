using System.Collections.Generic;

namespace TwoMQTT.Interfaces
{
    /// <summary>
    /// An interface representing shared options.
    /// </summary>
    /// <typeparam name="TQuestion">The type representing the question to ask the source; typically some kind of key/slug pairing.</typeparam>
    public interface ISharedOpts<TQuestion>
        where TQuestion : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        List<TQuestion> Resources { get; init; }
    }
}