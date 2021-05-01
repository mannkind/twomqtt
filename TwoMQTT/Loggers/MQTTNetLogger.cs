using System;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics;

namespace TwoMQTT.Loggers
{
    public class MQTTNetLogger : IMqttNetLogger
    {
        public MQTTNetLogger(ILogger<MQTTNetLogger> logger)
        {
            this.Logger = logger;
        }

        public IMqttNetScopedLogger CreateScopedLogger(string source)
        {
            return new MqttNetScopedLogger(this, source);
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

        private readonly ILogger<MQTTNetLogger> Logger;
    }
}
