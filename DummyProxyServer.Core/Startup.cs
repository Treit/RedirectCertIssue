namespace CampaignServiceWcfHost
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("redirectClient", config =>
            {
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                HttpClientHandler handler = new()
                {
                    AutomaticDecompression = DecompressionMethods.All,
                };

                return handler;
            });
        }

        public void Configure(IApplicationBuilder app, IHttpClientFactory clientFactory)
        {
            app.Run(async (context) =>
            {
                context.Response.Headers.Add("Content-Type", "text/xml; charset=utf-8");

                await DoRequest(context, clientFactory);
            });
        }

        static async Task DoRequest(HttpContext context, IHttpClientFactory clientFactory)
        {
            HttpClient client;
            X509Certificate? clientCert = null;

            var request = context.Request;

            if (!request.Query.ContainsKey("SkipClientCert"))
            {
                clientCert = await context.Connection.GetClientCertificateAsync();
            }

            if (clientCert == null)
            {
                client = clientFactory.CreateClient("redirectClient");
            }
            else
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCert);
                client = new HttpClient(handler);
            }

            var httpGetRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://treit.github.io")
            };

            var response = await client.SendAsync(httpGetRequest, CancellationToken.None);
            await context.Response.WriteAsync(await response.Content.ReadAsStringAsync());
        }
    }
}