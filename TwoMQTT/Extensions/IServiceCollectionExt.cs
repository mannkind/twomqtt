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
        public static IServiceCollection AddOptions<TOpts>(
            this IServiceCollection services,
            string section,
            IConfiguration configuration) where TOpts : class => services
                .AddOptions<TOpts>()
                .Bind(configuration.GetSection(section))
                .ValidateDataAnnotations()
                .Services;

        public static IServiceCollection AddIPC<TData, TCmd>(this IServiceCollection services)
            where TData : class
            where TCmd : class
        {
            var chanOpts = new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true,
            };
            var data = Channel.CreateUnbounded<TData>(chanOpts);
            var command = Channel.CreateUnbounded<TCmd>(chanOpts);

            return services
                .AddSingleton<ChannelReader<TData>>(x => data.Reader)
                .AddSingleton<ChannelWriter<TData>>(x => data.Writer)
                .AddSingleton<ChannelReader<TCmd>>(x => command.Reader)
                .AddSingleton<ChannelWriter<TCmd>>(x => command.Writer)
                .AddSingleton<Interfaces.IIPC<TData, TCmd>, Managers.IPCManager<TData, TCmd>>()
                .AddSingleton<Interfaces.IIPC<TCmd, TData>, Managers.IPCManager<TCmd, TData>>();
        }

        public static IServiceCollection AddSource<TData, TCmd, TSourceLiason>(this IServiceCollection services)
            where TData : class
            where TCmd : class
            where TSourceLiason : class, Interfaces.ISourceLiason<TData, TCmd> =>
            services
                .AddSingleton<Interfaces.ISourceLiason<TData, TCmd>, TSourceLiason>()
                .AddHostedService<Managers.SourceManager<TData, TCmd>>();

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