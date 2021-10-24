using System.Collections.Generic;
using PollingExample.Models.Shared;
using TwoMQTT.Interfaces;

namespace PollingExample.Models.Options;

/// <summary>
/// The shared options across the application
/// </summary>
public record SharedOpts : ISharedOpts<SlugMapping>
{
    public const string Section = "Polling";

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="SlugMapping"></typeparam>
    /// <returns></returns>
    public List<SlugMapping> Resources { get; init; } = new();
}
