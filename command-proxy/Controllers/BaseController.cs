using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectero.Cproxy.Models;

namespace Spectero.Cproxy.Controllers
{
    public class BaseController : Controller, IController
    {
        protected readonly AppConfig AppConfig;
        protected readonly ILogger<BaseController> Logger;

        public BaseController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger)
        {
            AppConfig = appConfig.CurrentValue;
            Logger = logger;
        }

        private IEnumerable<Claim> GetClaims()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            return identity?.Claims;
        }

        private Claim GetClaim(string type)
        {
            return GetClaims().FirstOrDefault(x => x.Type == type);
        }

        private Payload GetDecryptedPayload(string payload)
        {
            return null; // TODO: Implement this
        }
    }
}