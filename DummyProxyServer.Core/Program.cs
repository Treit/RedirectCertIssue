namespace CampaignServiceWcfHost
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using System;

    class Program
    {
        static int Main(string[] args)
        {
            string hostName = "localhost";
            int port = 1236;

            if (args.Length > 1)
            {
                hostName = args[0];
                if (!int.TryParse(args[1], out int temp))
                {
                    throw new ArgumentException("The port to use must be a valid integer.");
                }

                port = temp;
            }

            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging
                        .AddEventSourceLogger()
                        .SetMinimumLevel(LogLevel.Warning);
                })
                .UseKestrel(options =>
                {
                    options.ListenAnyIP(port, (options) =>
                    {
                        options.UseHttps();
                        Console.WriteLine($"Listening on port {port}.");
                    });
                })
                .UseStartup<Startup>();

            IWebHost host = builder.Build();

            host.Run();
            return 0;
        }
    }
}
