using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack;
using Spectero.Cproxy.Libraries.Utils;
using Spectero.Cproxy.Models;

namespace Spectero.Cproxy.Controllers
{
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class CommandProxyController : BaseController
    {
        private readonly HttpRequest _request;
        private readonly IDistributedCache _cache;
        private readonly HttpClient _client;

        public CommandProxyController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger,
            IHttpContextAccessor httpContext, IDistributedCache distributedCache,
            HttpClient httpClient) : base(
            appConfig, logger, httpContext)
        {
            _cache = distributedCache;
            _client = httpClient;
            _request = Context.Request;
        }

        public async Task<IActionResult> Handle()
        {
            // Method to proxy to the daemon over
            var method = new HttpMethod(_request.Method);

            // Combination of the path and any querystrings given
            var fullRequestedPath = _request.Path + _request.QueryString.Value;

            // Request body, if one was given
            var body = await StreamUtils.ReadStream(_request.Body);

            // Create a new request object, and dress it up
            var daemonRequest = new HttpRequestMessage(method, fullRequestedPath);

            if (!body.IsNullOrEmpty())
                daemonRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Daemon actions typically require authorization, dress it with the token
            daemonRequest.Headers.Add("Authorization", "Bearer w0w!");

            var response = await _client.SendAsync(daemonRequest);

            return Ok(response);
        }

    }
}