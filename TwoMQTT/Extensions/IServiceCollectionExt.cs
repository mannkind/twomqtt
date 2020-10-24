using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TwoMQTT.Extensions
{
    /// <summary>
    /// Extensions for classes implementing IServiceCollection
    /// </summary>
    public static class IServiceCollectionExt
    {
        public static IServiceCollection ConfigureOpts<TOpts>(this IServiceCollection services,
            HostBuilderContext hostContext, string section) where TOpts : class =>
            services.Configure<TOpts>(hostContext.Configuration.GetSection(section));
    }
}