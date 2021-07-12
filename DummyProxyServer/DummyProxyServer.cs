using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestProxy
{
    public static class DummyProxyServer
    {
        static DummyProxyServer()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public static void Start(string listenUrl)
        {
            WebApp.Start<Startup>(listenUrl);
        }

        class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                var config = new HttpConfiguration();

                var handler = new ProxyHandler();

                config.Routes.MapHttpRoute("Default", "{*path}", new { path = RouteParameter.Optional }, null, handler);

                app.UseWebApi(config);
            }
        }

        class ProxyHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await DoRequest(request, cancellationToken);
            }

            private async Task<HttpResponseMessage> DoRequest(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpClient client;
                X509Certificate clientCert = null;

                if (!request.RequestUri.Query.Contains("SkipClientCert"))
                {
                    clientCert = request.GetClientCertificate();
                }

                if (clientCert == null)
                {
                    client = HttpClientFactory.Create();
                }
                else
                {
                    var handler = new WebRequestHandler();
                    handler.ClientCertificates.Add(clientCert);
                    client = HttpClientFactory.Create(handler);
                }

                var httpGetRequest = new HttpRequestMessage();
                httpGetRequest.Method = HttpMethod.Get;
                httpGetRequest.RequestUri = new Uri("https://treit.github.io");

                var response = await client.SendAsync(httpGetRequest, cancellationToken);

                return response;
            }
        }
    }
}
