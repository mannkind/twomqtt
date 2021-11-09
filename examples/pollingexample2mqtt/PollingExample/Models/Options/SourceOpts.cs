using System;

namespace PollingExample.Models.Options;

/// <summary>
/// The source options
/// </summary>
public record SourceOpts
{
    public const string Section = "Polling";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public TimeSpan PollingInterval { get; init; } = new(0, 3, 31);
}
