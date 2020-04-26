using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TwoMQTT.Core.Extensions
{
    /// <summary>
    /// Extensions for classes implementing IServiceCollection
    /// </summary>
    public static class IServiceCollectionExt
    {
        /// <summary>
        /// Setup communication channels between a source (e.g. HTTPPollingManager) and sink (e.g. MQTTManager) 
        /// in order to allow for bidirectional communication.
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TSink"></typeparam>
        /// <returns></returns>
        public static IServiceCollection ConfigureBidirectionalSourceSink<TData, TCommand, TSource, TSink>(
            this IServiceCollection services)
            where TSource : class, IHostedService
            where TSink : class, IHostedService
        {
            var data = Channel.CreateUnbounded<TData>();
            var command = Channel.CreateUnbounded<TCommand>();
            services.AddSingleton<ChannelReader<TData>>(x => data.Reader);
            services.AddSingleton<ChannelWriter<TData>>(x => data.Writer);
            services.AddSingleton<ChannelReader<TCommand>>(x => command.Reader);
            services.AddSingleton<ChannelWriter<TCommand>>(x => command.Writer);
            services.AddHostedService<TSource>();
            services.AddHostedService<TSink>();

            return services;
        }
    }
}