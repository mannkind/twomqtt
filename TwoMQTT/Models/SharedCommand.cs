namespace TwoMQTT.Models;

/// <summary>
/// A class representing a command, shared between source and sink.
/// </summary>
/// <typeparam name="T">The type representing the data of the command.</typeparam>
public record SharedCommand<T>
    where T : new()
{
    public int Command { get; init; } = 0;
    public T Data { get; init; } = new T();

    public override string ToString() => $"Command: {this.Command}, Data: {this.Data}";
}
