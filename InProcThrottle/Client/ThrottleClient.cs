using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.Client
{
    public static class ThrottleClient
    {
        static Dictionary<string, ICommunicationProvider> _statusDictionary = new Dictionary<string, ICommunicationProvider>();
        static IList<ActionQueueItem> _queueItems = new List<ActionQueueItem>();

        public static void Clear()
        {
            if (_statusDictionary != null)
                _statusDictionary.Clear();
        }

        private static ICommunicationProvider getProvider<T>(string scopeKey) where T: ICommunicationProvider, new()
        {
            if (!_statusDictionary.ContainsKey(scopeKey))
            {
                var provider = new T();
                _statusDictionary.Add(scopeKey, provider);
                return provider;
            }
            else
            {
                return _statusDictionary[scopeKey];
            }
        }

        public static bool CanIRun<T>(string scopeTag) where T: ICommunicationProvider, new()
        {
            var provider = getProvider<T>(scopeTag);

            if (provider.DoesScopeKeyExists(scopeTag))
                return provider.IsOkToRun(scopeTag);
            else
                throw new Exception(String.Format("The scope key {0} was not created by the corresponding manager", scopeTag));
        }
        public static bool CanIRun<T>(string scopeKey, Action callBackFunction) where T: ICommunicationProvider, new()
        {
            if (CanIRun<T>(scopeKey))
            {
                callBackFunction.Invoke();
                return true;
            }
            else
            {
                _queueItems.Add(new ActionQueueItem() { ActionItem = callBackFunction, ScopeKey = scopeKey });
                return false;
            }
        }
        public static bool IsManagerReady
        {
            get
            {
                if (_statusDictionary.Keys.Count() > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
