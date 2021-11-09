using System;

namespace TwoMQTT.Managers;

/// <summary>
/// An class representing a managed way to delay polling a source.
/// </summary>
public class ThrottleManager : Interfaces.IThrottleManager
{
    /// <inheritdoc />
    public TimeSpan Interval { get; }

    /// <summary>
    /// Initializes a new instance of the ThrottleManager class.
    /// </summary>
    /// <param name="interval"></param>
    public ThrottleManager(TimeSpan interval)
    {
        this.Interval = interval;
    }
}
