using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Spectero.Cproxy.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommandProxyController : BaseController
    {
        public CommandProxyController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger) : base(
            appConfig, logger)
        {
        }

        public async Task<IActionResult> Handle()
        {
            return null;
        }
    }
}