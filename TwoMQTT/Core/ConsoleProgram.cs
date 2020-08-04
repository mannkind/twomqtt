using System;
using System.Linq;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using TwoMQTT.Core.Utils;
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Managers;
using System.Collections.Generic;

namespace TwoMQTT.Core
{
    /// <summary>
    /// A class representing a console program.
    /// </summary>
    public abstract class ConsoleProgram<TData, TCommand, TSourceLiason, TMQTTLiason>
        where TData : class
        where TCommand : class
        where TSourceLiason : class, ISourceLiason<TData, TCommand>
        where TMQTTLiason : class, IMQTTLiason<TData, TCommand>
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
        /// Allow implementing classes to register environment defaults.
        /// </summary>
        protected virtual IDictionary<string, string> EnvironmentDefaults() =>
            new Dictionary<string, string>();

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
            // Setup default environment variables
            foreach (var env in this.EnvironmentDefaults())
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(env.Key)))
                {
                    Environment.SetEnvironmentVariable(env.Key, env.Value);
                }
            }

            // Build the host
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    var data = Channel.CreateUnbounded<TData>();
                    var command = Channel.CreateUnbounded<TCommand>();
                    services.AddSingleton<ChannelReader<TData>>(x => data.Reader);
                    services.AddSingleton<ChannelWriter<TData>>(x => data.Writer);
                    services.AddSingleton<ChannelReader<TCommand>>(x => command.Reader);
                    services.AddSingleton<ChannelWriter<TCommand>>(x => command.Writer);

                    services.AddSingleton<ISourceLiason<TData, TCommand>, TSourceLiason>();
                    services.AddHostedService<SourceManager<TData, TCommand>>();

                    services.AddSingleton<IManagedMqttClient>(x => new MqttFactory().CreateManagedMqttClient());
                    services.AddSingleton<IMQTTGenerator, MQTTGenerator>(x =>
                    {
                        var opts = x.GetService<IOptions<Models.MQTTManagerOptions>>();
                        return new MQTTGenerator(opts.Value.TopicPrefix, opts.Value.DiscoveryName);
                    });
                    services.AddSingleton<IMQTTLiason<TData, TCommand>, TMQTTLiason>();
                    services.AddHostedService<MQTTManager<TData, TCommand>>();

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