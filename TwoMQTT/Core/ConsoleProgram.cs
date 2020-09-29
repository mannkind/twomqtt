using System;
using System.Collections.Generic;
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
using TwoMQTT.Core.Interfaces;
using TwoMQTT.Core.Managers;
using TwoMQTT.Core.Utils;

namespace TwoMQTT.Core
{
    /// <summary>
    /// A class representing a console program.
    /// </summary>
    /// <typeparam name="TData">The type representing the data from the source system.</typeparam>
    /// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
    /// <typeparam name="TSourceLiason">The type representing the liason to the source system.</typeparam>
    /// <typeparam name="TMqttLiason">The type representing the liason to the mqtt broker.</typeparam>
    public abstract class ConsoleProgram<TData, TCmd, TSourceLiason, TMqttLiason>
        where TData : class
        where TCmd : class
        where TSourceLiason : class, ISourceLiason<TData, TCmd>
        where TMqttLiason : class, IMQTTLiason<TData, TCmd>
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
        protected virtual IDictionary<string, string> EnvironmentDefaults() => new Dictionary<string, string>();

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
                    var command = Channel.CreateUnbounded<TCmd>();
                    services.AddSingleton<ChannelReader<TData>>(x => data.Reader);
                    services.AddSingleton<ChannelWriter<TData>>(x => data.Writer);
                    services.AddSingleton<ChannelReader<TCmd>>(x => command.Reader);
                    services.AddSingleton<ChannelWriter<TCmd>>(x => command.Writer);
                    services.AddSingleton<IIPC<TData, TCmd>, IPCManager<TData, TCmd>>();
                    services.AddSingleton<IIPC<TCmd, TData>, IPCManager<TCmd, TData>>();

                    services.AddSingleton<ISourceLiason<TData, TCmd>, TSourceLiason>();
                    services.AddHostedService<SourceManager<TData, TCmd>>();

                    services.AddSingleton<IManagedMqttClient>(x => new MqttFactory().CreateManagedMqttClient());
                    services.AddSingleton<IMQTTGenerator, MQTTGenerator>(x =>
                    {
                        var opts = x.GetService<IOptions<Models.MQTTManagerOptions>>();
                        if (opts == null)
                        {
                            throw new ArgumentException($"{nameof(opts.Value.TopicPrefix)} and {nameof(opts.Value.DiscoveryName)} are required for {nameof(MQTTGenerator)}.");
                        }

                        return new MQTTGenerator(opts.Value.TopicPrefix, opts.Value.DiscoveryName);
                    });
                    services.AddSingleton<IMQTTLiason<TData, TCmd>, TMqttLiason>();
                    services.AddHostedService<MQTTManager<TData, TCmd>>();

                    this.ConfigureServices(hostContext, services);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddSimpleConsole(c =>
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