using System;
using System.Linq;
using System.Reflection;

namespace TwoMQTT.Core
{
    public static class AppVersion
    {
        public static bool PrintVersion<T>(string[] args) 
        {
            var param = args?.Skip(1)?.FirstOrDefault() ?? string.Empty;
            if (param != VERSION)
            {
                return false;
            }

            var version = Assembly.GetAssembly(typeof(T))
                ?.GetName()
                ?.Version
                ?.ToString() ?? UNKNOWVERSION;

            Console.WriteLine($"v{version}");
            return true;
        }

        private const string VERSION = "version";
        private const string UNKNOWVERSION = "0.0.0.0";
    }
}