using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InProcThrottle.Client;
using InProcThrottle.CommunicationProviders;

namespace ExampleProject
{
    [Serializable]
    public class Client
    {
        string _tagKey = "";
        int _domainId = 0;
        int _numberOfJobs;
        System.Timers.Timer _timer;
        public void Start(string tagKey, int domainId, int numberOfJobs)
        {
            _numberOfJobs = numberOfJobs;
            _tagKey = tagKey;
            _domainId = domainId;
            print("Starting domain {0}, status {1}", domainId, ThrottleClient.CanIRun<InterProcessProvider>(_tagKey));
            _timer = new System.Timers.Timer(2000);
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _numberOfJobs = _numberOfJobs - 1;
            if (_numberOfJobs < 0)
            {
                _timer.Stop();
            }

            if (ThrottleClient.CanIRun<InterProcessProvider>(_tagKey, () => { Console.WriteLine("Resuming: "); RunJob(_numberOfJobs); }))
            {
                RunJob(_numberOfJobs);
            }
            else
            {
                print("Under to heavy load, job have to wait");
            }
        }

        public void RunJob(int jobNum)
        {
            var list = new List<Client>();
            for(int i = 0; i < 100000; i++){
                list.Add(new Client());
            }
            print("Done {0} - JobNum: {1}", Thread.CurrentThread.ManagedThreadId, jobNum);
        }

        private void print(string message, params object[] arg)
        {
            message = "Domain: " + _domainId + " - " + message;
            Console.WriteLine(message, arg);
        }
    }
}
