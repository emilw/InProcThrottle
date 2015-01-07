using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using InProcThrottle.CommunicationProviders;
using InProcThrottle.Manager;

namespace ExampleProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing");
            var timer = new System.Timers.Timer(2000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            ThrottleManager.Config<InterProcessProvider>("Test", 10, 360);
            while (ThrottleManager.GetCurrentSampleCount() < 2) { };
            var clients = new List<Client>();

            for (int i = 0; i < 50; i++)
            {
                clients.Add(startNewDomain(i));
            }

                Console.WriteLine("Hit any key to exist");
            Console.ReadLine();
        }

        static Client startNewDomain(int domainId)
        {
            var domain = AppDomain.CreateDomain("ChildDomain");
            var test = domain.CreateInstance(Assembly.GetExecutingAssembly().FullName, typeof(Client).FullName);
            var client = (Client)test.Unwrap();
            client.Start("Test", domainId, 10);
            return client;
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Average CPU right now: {0}, Limit was set to: {1}, Samples {2}, Latest sample: {3}",
                                            ThrottleManager.CPUAverage,
                                            ThrottleManager.Configs.First().Value.Percentage,
                                            ThrottleManager.GetCurrentSampleCount(), ThrottleManager.CPULatest);
        }
    }
}
