using System;
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

namespace Spectero.Cproxy.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommandProxyController : BaseController
    {
        private readonly IDistributedCache _cache;
        private readonly HttpClient _client;

        public CommandProxyController(IOptionsMonitor<AppConfig> appConfig, ILogger<BaseController> logger,
            IHttpContextAccessor httpContext, IDistributedCache distributedCache,
            HttpClient httpClient) : base(
            appConfig, logger, httpContext)
        {
            _cache = distributedCache;
            _client = httpClient;
        }

        public async Task<IActionResult> Handle()
        {
            var request = Context.Request;
            var response = Context.Response;
            
            var targetNode = GetPayload();

            // Method to proxy to the daemon over
            var method = new HttpMethod(request.Method);

            // Combination of the path and any querystrings given
            var fullRequestedPath = targetNode.GetAccessor() + request.Path + request.QueryString.Value;

            // Request body, if one was given
            var body = await StreamUtils.ReadStream(request.Body);

            // Create a new request object, and dress it up
            var daemonRequest = new HttpRequestMessage(method, fullRequestedPath);

            if (!body.IsNullOrEmpty())
                daemonRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Daemon actions typically require authorization, dress it with the token
            daemonRequest.Headers.Add("Authorization", "Bearer " + targetNode.credentials?.access?.token);

            var responseMessage = await _client.SendAsync(daemonRequest);
            
            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            foreach (var upstreamHeader in responseMessage.Headers)
            {
                var key = upstreamHeader.Key;
                var values = upstreamHeader.Value;

                foreach (var value in values)
                {
                    response.Headers.Add("E-" + key, value);
                }
            }

            var byteCount = 0;
            if (! responseBody.IsNullOrEmpty())
            {
                byteCount = Encoding.Unicode.GetByteCount(responseBody);
            }
            
            response.Headers.Add("E-Response-Size", byteCount.ToString());
            response.Headers.Add("E-Uri-Requested", fullRequestedPath);
            response.Headers.Add("E-Request-Method", request.Method);

            // Setting this unfortunately leads to the string being double escaped, see https://puu.sh/APSGg/9e0e8b140c.png
            //response.ContentType = "application/json";
            
            return StatusCode((int) responseMessage.StatusCode, responseBody);
        }

    }
}