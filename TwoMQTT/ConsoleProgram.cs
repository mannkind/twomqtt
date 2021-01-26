using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwoMQTT.Extensions;

namespace TwoMQTT
{
    /// <summary>
    /// A class representing a console program.
    /// </summary>
    /// <typeparam name="TData">The type representing the data from the source system.</typeparam>
    /// <typeparam name="TCmd">The type representing the command to the source system. </typeparam>
    /// <typeparam name="TSourceLiason">The type representing the liason to the source system.</typeparam>
    /// <typeparam name="TMqttLiason">The type representing the liason to the mqtt broker.</typeparam>
    public class ConsoleProgram<TData, TCmd, TSourceLiason, TMqttLiason>
        where TData : class
        where TCmd : class
        where TSourceLiason : class, Interfaces.ISourceLiason<TData, TCmd>
        where TMqttLiason : class, Interfaces.IMQTTLiason<TData, TCmd>
    {
        /// <summary>
        /// Run the program.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task ExecuteAsync(string[] args,
            IDictionary<string, string>? envs = null,
            Action<HostBuilderContext, IConfigurationBuilder>? configureAppConfiguration = null,
            Action<HostBuilderContext, IServiceCollection>? configureServices = null,
            Action<HostBuilderContext, ILoggingBuilder>? configureLogging = null,
            CancellationToken cancellationToken = default)
        {
            if (PrintVersion(args))
            {
                return Task.CompletedTask;
            }

            ConfigureEnvironmentDefaults(envs);
            return RunAsync(args, configureAppConfiguration, configureServices, configureLogging, cancellationToken);
        }

        /// <summary>
        /// Print the version of the current assembly.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool PrintVersion(string[] args)
        {
            var param = args?.ElementAtOrDefault(1);
            if (param != VERSION)
            {
                return false;
            }

            var version = Assembly.GetExecutingAssembly()?.GetName().Version?.ToString() ?? UNKNOWVERSION;
            Console.WriteLine($"v{version}");
            return true;
        }

        /// <summary>
        /// Configure environment defaults used to configure options.
        /// </summary>
        /// <param name="envs"></param>
        private static void ConfigureEnvironmentDefaults(IDictionary<string, string>? envs)
        {
            // Setup default environment variables
            envs ??= new Dictionary<string, string>();
            foreach (var env in envs)
            {
                var key = env.Key.Replace(":", "__");
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    continue;
                }

                Environment.SetEnvironmentVariable(key, env.Value);
            }
        }

        /// <summary>
        /// Run the console program.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="configureAppConfiguration"></param>
        /// <param name="configureServices"></param>
        /// <param name="configureLogging"></param>
        /// <returns></returns>
        private static Task RunAsync(string[] args, Action<HostBuilderContext, IConfigurationBuilder>? configureAppConfiguration, Action<HostBuilderContext, IServiceCollection>? configureServices, Action<HostBuilderContext, ILoggingBuilder>? configureLogging, CancellationToken cancellationToken = default)
        {
            // Build the host
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);

                    configureAppConfiguration?.Invoke(context, config);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions()
                        .AddIPC<TData, TCmd>()
                        .AddSource<TData, TCmd, TSourceLiason>()
                        .AddMqtt<TData, TCmd, TMqttLiason>();

                    configureServices?.Invoke(context, services);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"))
                        .AddSimpleConsole(c =>
                        {
                            c.TimestampFormat = "[HH:mm:ss] ";
                        });

                    configureLogging?.Invoke(context, logging);
                });

            return builder.RunConsoleAsync();
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