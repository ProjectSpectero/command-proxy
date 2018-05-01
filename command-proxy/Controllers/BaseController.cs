using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Spectero.Cproxy.Models;

namespace Spectero.Cproxy.Controllers
{
    public class BaseController : Controller, IController
    {
        protected readonly AppConfig AppConfig;
        protected readonly ILogger<BaseController> Logger;
        protected readonly HttpContext Context;

        public BaseController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger,
            IHttpContextAccessor httpContext)
        {
            AppConfig = appConfig.CurrentValue;
            Logger = logger;
            Context = httpContext.HttpContext;
        }

        protected IEnumerable<Claim> GetClaims()
        {
            var identity = Context.User.Identity as ClaimsIdentity;
            return identity?.Claims;
        }

        protected Claim GetClaim(string type)
        {
            return GetClaims().FirstOrDefault(x => x.Type == type);
        }

        protected Node GetPayload()
        {
            var proxyPayload = GetClaim(Libraries.Constants.Claim.ProxyPayload); // This is a JSON payload containing the node object

            return JsonConvert.DeserializeObject<Node>(proxyPayload.Value);
        }
    }
}