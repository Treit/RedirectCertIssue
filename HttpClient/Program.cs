using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Provide a URL to fetch.");
                return;
            }

            string url = args[0];

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

            HttpClient client = new HttpClient(handler);

            Console.WriteLine($"GET of {url} starting.");
            var resp = await client.GetAsync(url);
            var content = await resp.Content.ReadAsStringAsync();

            Console.WriteLine(content);
        }
    }
}
