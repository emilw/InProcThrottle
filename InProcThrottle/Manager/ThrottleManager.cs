using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace InProcThrottle.Manager
{
    public static class ThrottleManager
    {
        private static Dictionary<string, Config> _configuration;
        private static int SAMPLING_INTERVALL = 2000;
        private static int _samplingPosition = 0;
        private static int[] _cpuSamples;
        private static Timer _timer;

        public static void Clear()
        {
            if (_configuration != null)
            {
                _configuration.Values.ToList().ForEach(x => x.Provider.Clear());
                _configuration.Clear();
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Close();
            }
        }

        private static void init()
        {
            _configuration = new Dictionary<string,Config>();
            _cpuSamples = new int[100];
            _timer = new Timer(SAMPLING_INTERVALL);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _cpuSamples[_samplingPosition] = getCPUCounter();
            _samplingPosition += 1;
        }

        public static void Config<T>(string configTag, int cpuPercentage, int timeSpanInSeconds) where T: ICommunicationProvider, new()
        {
            if (_configuration == null)
                init();

            var provider = new T();
            provider.UpdateStatus(configTag, true);
            _configuration.Add(configTag, new Config(){Percentage = cpuPercentage, TimeSpan = timeSpanInSeconds, Provider = provider});
        }

        public static IDictionary<string, Config> Configs
        {
            get {
                if (_configuration == null)
                    return new Dictionary<string, Config>();
                else
                    return _configuration;
            }
        }

        private static int getCPUCounter()
        {

            PerformanceCounter cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            // will always start at 0
            dynamic firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            dynamic secondValue = cpuCounter.NextValue();

            return secondValue;
        }

    }
}
