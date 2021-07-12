using System;

namespace TestProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            string listenUrl = @"https://localhost:1234";

            if (args.Length > 0)
            {
                listenUrl = args[0];
            }

            Console.WriteLine($"Listening on {listenUrl}.");

            DummyProxyServer.Start(listenUrl);
            Console.WriteLine("Press <Enter> to stop.");
            Console.ReadKey();
        }
    }
}
