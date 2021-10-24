namespace PollingExample.Models.Source;

/// <summary>
/// The response from the source
/// </summary>
public record Response
{
    /// <summary>
    /// 
    /// </summary>
    /// <value></value>
    public string Key { get; init; } = string.Empty;
}
