using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectero.Cproxy.Libraries.Utils;

namespace Spectero.Cproxy.Controllers
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class CommandProxyController : BaseController
    {
        private readonly HttpRequest _request;

        public CommandProxyController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger,
            IHttpContextAccessor httpContext) : base(
            appConfig, logger, httpContext)
        {
            _request = Context.Request;
        }

        public async Task<IActionResult> Handle()
        {
            var fullRequestedPath = _request.Path + _request.QueryString.Value;
            var body = await StreamUtils.ReadStream(_request.Body);
           

            return Ok($"path: {fullRequestedPath}, body: {body}");
        }
    }
}