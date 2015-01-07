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
        private static int _samplingPosition;
        private static decimal[] _cpuSamples;
        private static Timer _timer;
        private static bool _initizalized = false;

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

            _initizalized = false;
        }

        public static bool IsInitialized
        {
            get
            {
                return _initizalized;
            }
        }

        public static void Init(int samplingIntervall)
        {
            _configuration = new Dictionary<string, Config>();
            _cpuSamples = new decimal[100];
            _samplingPosition = 0;
            for (int i = 0; i < _cpuSamples.Count(); i++)
                _cpuSamples[i] = 0;

            _timer = new Timer(samplingIntervall);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

            _initizalized = true;
        }

        public static void Init()
        {
            Init(2000);//2 sec
        }

        static void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //If we are in the end of the sampling array, start over
            if ((_samplingPosition+1) == _cpuSamples.Count())
            {
                _samplingPosition = 0;
            }
            
            _cpuSamples[_samplingPosition] = getCPUCounter();
            _samplingPosition = _samplingPosition + 1;
            saveStateForConfigurations();
        }

        private static void saveStateForConfigurations()
        {
            foreach (var key in _configuration.Keys)
            {
                var config = _configuration[key];
                config.Provider.UpdateStatus(key, IsItOkToRun(key));
            }
        }

        public static void Config<T>(string configTag, decimal cpuPercentage, int timeSpanInSeconds) where T: IManagerCommunicationProvider, new()
        {
            //To allow zero configuration, set the default settings if it's not Initialized explicitly
            if (!IsInitialized)
                Init();

            //Update existing config
            if (_configuration.ContainsKey(configTag))
            {
                var configItem = _configuration[configTag];
                if (configItem.Provider.GetType() != typeof(T))
                    throw new NotSupportedException("It's not supported to change a provider type for a given config key");

                configItem.Percentage = cpuPercentage;
                configItem.TimeSpan = timeSpanInSeconds;
            }
            else
            {
                var provider = new T();
                provider.UpdateStatus(configTag, false);
                _configuration.Add(configTag, new Config() { Percentage = cpuPercentage, TimeSpan = timeSpanInSeconds, Provider = provider });
            }
        }

        public static int GetCurrentSampleCount()
        {
            return _samplingPosition;
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

        public static bool IsItOkToRun(string keyTag)
        {
            return _configuration[keyTag].Percentage > CPUAverage;
        }

        public static decimal CPUAverage
        {
            get
            {
                if (_samplingPosition == 0)
                    return 0;

                return _cpuSamples.Where(x => x != 0).Average();
            }
        }

        public static decimal CPULatest
        {
            get
            {
                int samplePos = _samplingPosition - 1;
                if (samplePos < 0)
                    samplePos = 0;

                return _cpuSamples[samplePos];
            }
        }

        private static decimal getCPUCounter()
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

            return Convert.ToDecimal(secondValue);
        }

    }
}
