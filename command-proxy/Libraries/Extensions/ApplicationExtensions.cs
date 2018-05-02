using Microsoft.AspNetCore.Builder;
using Spectero.Cproxy.Libraries.Utils;

namespace Spectero.Cproxy.Libraries.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UsePreflightAuthorizer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PreflightAuthorizer>();
        }
    }
}