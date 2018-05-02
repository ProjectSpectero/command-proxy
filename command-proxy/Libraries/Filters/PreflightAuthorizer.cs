using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Spectero.Cproxy.Libraries.Utils
{
    public class PreflightAuthorizer
    {
        private readonly RequestDelegate _next;

        public PreflightAuthorizer(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Method.Equals("OPTIONS")) // Horrible hack
            {
                httpContext.Response.StatusCode = 200;
                return;
            }
            await _next(httpContext);
        }
    }
}