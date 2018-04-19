using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using Spectero.Cproxy.Libraries.Extensions;

namespace Spectero.Cproxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return WebHost
                .CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(Startup.BuildConfiguration(environment))
                .UseStartup<Startup>()
                .UseNLog()
                /*
                 * Be sure to set the PFX secret (for SSL/TLS) using one of the following:
                 *  1. dotnet user-secrets set HttpServer:Endpoints:Https:Password
                 *  2. Set env var -> HttpServer:Endpoints:Https:Password or HttpServer__Endpoints__Https__Password
                 */
                .UseKestrel(options => options.ConfigureEndpoints())
                .Build();
        }
    }
}