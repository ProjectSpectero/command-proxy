using System;
using System.Net;
using System.Threading.Tasks;
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
            // This gets thrown on a generic timeout error. Logging it is pointless.
            if (! (context.Exception is TaskCanceledException) && ! (context.Exception is OperationCanceledException))
                _logger.LogError(0, context.Exception, "Unhandled exception occured.");
            
            var response = context.HttpContext.Response;
            response.StatusCode = (int) HttpStatusCode.ServiceUnavailable;

            // No further propagation. We're the last bastion of defense!
            context.ExceptionHandled = true;
        }
    }
}