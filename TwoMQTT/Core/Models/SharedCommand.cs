namespace TwoMQTT.Core.Models
{
    /// <summary>
    /// A class representing a command, shared between source and sink.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SharedCommand<T>
        where T : new()
    {
        public int Command { get; set; } = 0;
        public T Data { get; set; } = new T();

        public override string ToString()
        {
            return $"Command: {this.Command}, Data: {this.Data}";
        }
    }
}
