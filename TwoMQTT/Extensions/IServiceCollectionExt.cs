using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using TwoMQTT.Managers;

namespace TwoMQTT.Extensions
{
    /// <summary>
    /// Extensions for classes implementing IServiceCollection
    /// </summary>
    public static class IServiceCollectionExt
    {
        /// <summary>
        /// Register options for a given configuration section.
        /// </summary>
        /// <typeparam name="TOpts"></typeparam>
        public static IServiceCollection AddOptions<TOpts>(
            this IServiceCollection services,
            string section,
            IConfiguration configuration) where TOpts : class => services
                .AddOptions<TOpts>()
                .Bind(configuration.GetSection(section))
                .ValidateDataAnnotations()
                .Services;

        /// <summary>
        /// Register bidirectional, interprocess communication.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TCmd"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddIPC<TData, TCmd>(this IServiceCollection services)
            where TData : class
            where TCmd : class
        {

            return services
                .AddSingleton<Channel<TData>>(x =>
                    Channel.CreateUnbounded<TData>(new UnboundedChannelOptions
                    {
                        SingleReader = true,
                        SingleWriter = true,
                    })
                )
                .AddSingleton<Channel<TCmd>>(x =>
                    Channel.CreateUnbounded<TCmd>(new UnboundedChannelOptions
                    {
                        SingleReader = true,
                        SingleWriter = true,
                    })
                )
                .AddSingleton<ChannelReader<TData>>(x =>
                {
                    var ch = x.GetRequiredService<Channel<TData>>();
                    return ch.Reader;
                })
                .AddSingleton<ChannelWriter<TData>>(x =>
                {
                    var ch = x.GetRequiredService<Channel<TData>>();
                    return ch.Writer;
                })
                .AddSingleton<ChannelReader<TCmd>>(x =>
                {
                    var ch = x.GetRequiredService<Channel<TCmd>>();
                    return ch.Reader;
                })
                .AddSingleton<ChannelWriter<TCmd>>(x =>
                {
                    var ch = x.GetRequiredService<Channel<TCmd>>();
                    return ch.Writer;
                })
                .AddSingleton<Interfaces.IIPC<TData, TCmd>, IPCManager<TData, TCmd>>()
                .AddSingleton<Interfaces.IIPC<TCmd, TData>, IPCManager<TCmd, TData>>();
        }

        /// <summary>
        /// Register source liasons, managers, etc.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TCmd"></typeparam>
        /// <typeparam name="TSourceLiason"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddSource<TData, TCmd, TSourceLiason>(this IServiceCollection services)
            where TData : class
            where TCmd : class
            where TSourceLiason : class, Interfaces.ISourceLiason<TData, TCmd> =>
            services
                .AddSingleton<Interfaces.ISourceLiason<TData, TCmd>, TSourceLiason>()
                .AddHostedService<Managers.SourceManager<TData, TCmd>>();

        /// <summary>
        /// Register MQTT liasons, managers, etc.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TCmd"></typeparam>
        /// <typeparam name="TMqttLiason"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddMqtt<TData, TCmd, TMqttLiason>(this IServiceCollection services)
            where TData : class
            where TCmd : class
            where TMqttLiason : class, Interfaces.IMQTTLiason<TData, TCmd> =>
            services
                .AddSingleton<IMqttNetLogger, Loggers.MQTTNetLogger>()
                .AddSingleton<IManagedMqttClient>(x =>
                {
                    var logger = x.GetRequiredService<IMqttNetLogger>();
                    return new MqttFactory().CreateManagedMqttClient(logger);
                })
                .AddSingleton<Utils.IMQTTGenerator, Utils.MQTTGenerator>(x =>
                {
                    var opts = x.GetRequiredService<IOptions<Models.MQTTManagerOptions>>();
                    return new Utils.MQTTGenerator(opts.Value.TopicPrefix, opts.Value.DiscoveryName);
                })
                .AddSingleton<Interfaces.IMQTTLiason<TData, TCmd>, TMqttLiason>()
                .AddHostedService<Managers.MQTTManager<TData, TCmd>>();
    }
}