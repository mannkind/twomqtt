using System;

namespace TwoMQTT.Interfaces;

/// <summary>
/// Backwards compatibility; will be removed soon.
/// </summary>
[Obsolete("Use IPollingSourceDAO instead.")]
public interface ISourceDAO<TQuestion, TQuestionResponse, TCmd, TCmdResponse> : IPollingSourceDAO<TQuestion, TQuestionResponse, TCmd, TCmdResponse>
    where TQuestion : class
    where TQuestionResponse : class
    where TCmd : class
    where TCmdResponse : class
{
}
