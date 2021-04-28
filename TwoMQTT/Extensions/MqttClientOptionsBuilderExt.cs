using MQTTnet.Client.Options;

namespace TwoMQTT.Extensions
{
    /// <summary>
    /// Extensions for classes implementing IServiceCollection
    /// </summary>
    public static class MqttClientOptionsBuilderExt
    {
        /// <summary>
        /// Conditional credentials.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="condition"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static MqttClientOptionsBuilder WithConditionalCredentials(this MqttClientOptionsBuilder builder, bool condition, string username, string? password = null)
        {
            if (!condition) 
            {
                return builder;  
            }

            return builder.WithCredentials(username, password);
        }
    }
}