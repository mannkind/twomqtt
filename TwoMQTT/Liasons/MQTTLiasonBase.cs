using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TwoMQTT.Liasons;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TData">The type representing the mapped data from the source system.</typeparam>
/// <typeparam name="TCommand">The type representing the command to the source system. </typeparam>
/// <typeparam name="TQuestion">The type representing the question to ask the source; typically some kind of key/slug pairing.</typeparam>
/// <typeparam name="TOpts">The type representing the shared options.</typeparam>
public abstract class MQTTLiasonBase<TData, TCommand, TQuestion, TOpts>
    where TData : class
    where TCommand : class
    where TQuestion : class
    where TOpts : class, Interfaces.ISharedOpts<TQuestion>, new()
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="generator"></param>
    /// <param name="sharedOpts"></param>
    public MQTTLiasonBase(ILogger<MQTTLiasonBase<TData, TCommand, TQuestion, TOpts>> logger, Interfaces.IMQTTGenerator generator, IOptions<TOpts> sharedOpts)
    {
        this.Logger = logger;
        this.Generator = generator;
        this.Questions = sharedOpts.Value.Resources;
    }

    /// <summary>
    /// The logger used internally.
    /// </summary>
    protected readonly ILogger<MQTTLiasonBase<TData, TCommand, TQuestion, TOpts>> Logger;

    /// <summary>
    /// The questions to ask the source (typically some kind of key/slug pairing).
    /// </summary>
    protected readonly IEnumerable<TQuestion> Questions;

    /// <summary>
    /// The MQTT generator used for things such as availability topic, state topic, command topic, etc.
    /// </summary>
    protected readonly Interfaces.IMQTTGenerator Generator;
}
