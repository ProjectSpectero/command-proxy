using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Spectero.Cproxy.Models;

namespace Spectero.Cproxy.Libraries.Utils
{
    // Not used, usage requires distinct HttpClient instances.
    public class DaemonHttpRequestHandler : HttpClientHandler
    {
        private readonly Credential _credentials;

        public DaemonHttpRequestHandler(Credential credentials)
        {
            _credentials = credentials;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // TODO: Before doing this, check if the token is expired or not.
            request.Headers.Add("Authorization", $"Bearer {_credentials.access.token}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}