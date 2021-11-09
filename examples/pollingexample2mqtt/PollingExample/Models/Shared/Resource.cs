namespace PollingExample.Models.Shared;

/// <summary>
/// The shared resource across the application
/// </summary>
public record Resource
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string Key { get; init; } = string.Empty;
}
