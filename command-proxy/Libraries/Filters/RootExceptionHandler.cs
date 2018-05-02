using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Spectero.Cproxy.Libraries.Utils
{
    public class RootExceptionHandler : IExceptionFilter
    {
        private readonly ILogger<RootExceptionHandler> _logger;

        public RootExceptionHandler(ILogger<RootExceptionHandler> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(0, context.Exception, "Unhandled exception occured.");

            var response = context.HttpContext.Response;
            response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;

            // No further propagation. We're the last bastion of defense!
            context.ExceptionHandled = true;
        }
    }
}