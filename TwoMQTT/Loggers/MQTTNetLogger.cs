using System;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics.Logger;

namespace TwoMQTT.Loggers
{
    public class MQTTNetLogger : IMqttNetLogger
    {
        public MQTTNetLogger(ILogger<MQTTNetLogger> logger)
        {
            this.Logger = logger;
        }

        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
        {
            switch (logLevel)
            {
                case MqttNetLogLevel.Info:
                    this.Logger.LogInformation(message, parameters);
                    break;
                case MqttNetLogLevel.Warning:
                    this.Logger.LogWarning(message, parameters);
                    break;
                case MqttNetLogLevel.Error:
                    this.Logger.LogError(message, parameters);
                    break;
                case MqttNetLogLevel.Verbose:
                    this.Logger.LogDebug(message, parameters);
                    break;
                default:
                    this.Logger.LogTrace(message, parameters);
                    break;
            };
        }

        public bool IsEnabled => true;

        private readonly ILogger<MQTTNetLogger> Logger;
    }
}
