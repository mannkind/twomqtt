using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace TwoMQTT.Core
{
    /// <summary>
    /// A class representing a console program.
    /// </summary>
    public abstract class ConsoleProgram
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
                    services.AddOptions();
                    this.ConfigureServices(hostContext, services);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
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