using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Spectero.Cproxy.Models;

namespace Spectero.Cproxy
{
    public class Startup
    {
        private static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Configuration = BuildConfiguration(env.EnvironmentName);
        }

        // Dirty hack? Absolutely.
        // Avoids this bullshit though: https://github.com/aspnet/Hosting/issues/766
        public static IConfiguration BuildConfiguration(string envName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(CurrentDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{envName}.json", true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Don't build a premature service provider from IServiceCollection, it only includes the services registered when the provider is built.
            var appConfig = Configuration.GetSection("Core");

            services.Configure<AppConfig>(appConfig);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfig["PoolSigningKey"]))
                    };
                });


            services.AddCors(options =>
            {
                // TODO: Lock down this policy in production
                options.AddPolicy("DefaultCORSPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddDistributedMemoryCache();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, IOptionsMonitor<AppConfig> configMonitor)
        {
            var appConfig = configMonitor.CurrentValue;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Redirect into the HTTPs port if a request is received over HTTP
            int? httpsPort = null;
            var httpsSection = Configuration.GetSection("HttpServer:Endpoints:Https");
            if (httpsSection.Exists())
            {
                var httpsEndpoint = new EndpointConfiguration();
                httpsSection.Bind(httpsEndpoint);
                httpsPort = httpsEndpoint.Port;
            }
            var statusCode = env.IsDevelopment() ? StatusCodes.Status302Found : StatusCodes.Status301MovedPermanently;
            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(statusCode, httpsPort));

            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                    name: "GlobalHandler",
                    defaults: new { controller = "CommandProxy", action = "Handle" } // Horrible hack, but hey, it works!
                );
            });

            loggerFactory.AddNLog();
            loggerFactory.ConfigureNLog(appConfig.LoggingConfig);

        }
    }
}
