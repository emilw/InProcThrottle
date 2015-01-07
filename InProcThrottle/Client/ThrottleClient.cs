using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.Client
{
    public static class ThrottleClient
    {
        static Dictionary<string, IClientCommunicationProvider> _statusDictionary = new Dictionary<string, IClientCommunicationProvider>();
        static IList<ActionQueueItem> _queueItems = new List<ActionQueueItem>();

        public static void Clear()
        {
            if (_statusDictionary != null)
                _statusDictionary.Clear();
            if (_queueItems != null)
                _queueItems.Clear();
        }

        private static IClientCommunicationProvider getProvider<T>(string scopeKey) where T: IClientCommunicationProvider, new()
        {
            if (!_statusDictionary.ContainsKey(scopeKey))
            {
                var provider = new T();
                provider.StatusChanged += new StatusChangedEventHandler(provider_StatusChanged);
                _statusDictionary.Add(scopeKey, provider);
                return provider;
            }
            else
            {
                return _statusDictionary[scopeKey];
            }
        }

        static void provider_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            lock ("queueItems")
            {
                foreach (var item in _queueItems.Where(x => x.ScopeKey == e.ScopeKey &&
                                                        x.Status == Status.Created))
                {
                    if (canIRun(item.ScopeKey))
                    {
                        item.Status = Status.InProgress;
                        item.ActionItem.Invoke();
                        item.Status = Status.ToBeRemoved;
                    }
                }
            }
        }

        private static bool canIRun(string scopeTag)
        {
            return _statusDictionary[scopeTag].IsOkToRun(scopeTag);
        }

        public static bool CanIRun<T>(string scopeTag) where T: IClientCommunicationProvider, new()
        {
            var provider = getProvider<T>(scopeTag);

            if (provider.DoesScopeKeyExists(scopeTag))
            {
                return provider.IsOkToRun(scopeTag);
            }
            else
            {
                throw new Exception(String.Format("The scope key {0} was not created by the corresponding manager", scopeTag));
            }
        }
        public static bool CanIRun<T>(string scopeKey, Action callBackFunction) where T: IClientCommunicationProvider, new()
        {
            if (!CanIRun<T>(scopeKey)){
                lock("queueItems"){
                    _queueItems.ToList().RemoveAll(x => x.Status == Status.ToBeRemoved);
                    _queueItems.Add(new ActionQueueItem(callBackFunction, scopeKey));
                }
                return false;
            }

            return true;
        }

        public static int QueueCount
        {
            get
            {
                return _queueItems.Count();
            }
        }
    }
}
