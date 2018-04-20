using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
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
        protected readonly HttpContext Context;

        public BaseController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger,
            IHttpContextAccessor ctxAccessor)
        {
            AppConfig = appConfig.CurrentValue;
            Logger = logger;
            Context = ctxAccessor.HttpContext;
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