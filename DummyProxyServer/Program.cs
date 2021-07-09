using System;

namespace TestProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            DummyProxyServer.Start();
            Console.WriteLine("Press <Enter> to stop.");
            Console.ReadKey();
        }
    }
}
