using System;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TwoMQTT.Core
{
    /// <summary>
    /// A class representing a console program.
    /// </summary>
    public abstract class ConsoleProgram<TData, TCommand, TSource, TSink>
        where TSource : class, IHostedService
        where TSink : class, IHostedService
    {
        /// <summary>
        /// Initializes a new instance of the HTTPSourceDAO class.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task ExecuteAsync(string[] args)
        {
            if (this.PrintVersion(args))
            {
                return Task.CompletedTask;
            }

            return this.Run(args);
        }

        /// <summary>
        /// Allow implementing classes to register dependencies.
        /// </summary>
        /// <param name="hostContext"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        protected abstract IServiceCollection ConfigureServices(HostBuilderContext hostContext, IServiceCollection services);

        /// <summary>
        /// Print the version of the current assembly.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool PrintVersion(string[] args)
        {
            var param = args?.Skip(1)?.FirstOrDefault() ?? string.Empty;
            if (param != VERSION)
            {
                return false;
            }

            var version = Assembly.GetAssembly(this.GetType())
                ?.GetName()
                ?.Version
                ?.ToString() ?? UNKNOWVERSION;

            Console.WriteLine($"v{version}");
            return true;
        }

        /// <summary>
        /// Run the console program.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Task Run(string[] args) =>
            this.Configure(args)
                .RunConsoleAsync();

        /// <summary>
        /// Configure the console program.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private IHostBuilder Configure(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {

                    var data = Channel.CreateUnbounded<TData>();
                    var command = Channel.CreateUnbounded<TCommand>();

                    services.AddOptions();
                    services.AddSingleton<ChannelReader<TData>>(x => data.Reader);
                    services.AddSingleton<ChannelWriter<TData>>(x => data.Writer);
                    services.AddSingleton<ChannelReader<TCommand>>(x => command.Reader);
                    services.AddSingleton<ChannelWriter<TCommand>>(x => command.Writer);
                    services.AddHostedService<TSource>();
                    services.AddHostedService<TSink>();

                    this.ConfigureServices(hostContext, services);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole(c =>
                    {
                        c.TimestampFormat = "[HH:mm:ss] ";
                    });
                });

            return builder;
        }

        /// <summary>
        /// The string representing the 'version' command line argument.
        /// </summary>
        private const string VERSION = "version";

        /// <summary>
        /// The string representing an unknown version.
        /// </summary>
        private const string UNKNOWVERSION = "0.0.0.0";
    }
}