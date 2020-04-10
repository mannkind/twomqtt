namespace TwoMQTT.Core.Models
{
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
