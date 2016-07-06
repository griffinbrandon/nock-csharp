using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Nock.CSharp
{
    public class TestMessageHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var method = request.Method.ToString().ToLower();
            var uri = request.RequestUri.ToString().ToLower();

            var nocks = Nocker.Nocks;

            // make sure the method exist for the uri
            if (nocks.All(x => !(x.RequestType.ToString().ToLower() == method && $"{x.BasePath}{x.Uri}".ToLower() == uri)))
            {
                throw new NockException($"Unable to mock {method} method, {uri}");
            }

            Nocker nocker;

            try
            {
                nocker =
                    nocks.Single(x => x.RequestType.ToString().ToLower() == method && $"{x.BasePath}{x.Uri}".ToLower() == uri);
            }
            catch (InvalidOperationException ex)
            {
                // not sure what else could go wrong here
                throw new NockException($"Mulitple nocks found with method {method}, {uri}");
            }

            return await nocker.Respond();

        }
    }
}