using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.CommunicationProviders
{
    public class SharedInProcessProvider : IClientCommunicationProvider, IManagerCommunicationProvider
    {
        public SharedInProcessProvider()
        {
            InMemoryStorage.StatusUpdatedEvent += new StatusChangedEventHandler(MemoryStorageStatusChanged);
        }
        
        public bool IsOkToRun(string tagKey)
        {
            return InMemoryStorage.GetStatus(tagKey);//InMemmoryStorage.StatusDictionary[tagKey];
        }

        void IManagerCommunicationProvider.UpdateStatus(string tagKey, bool newStatus)
        {
            if (InMemoryStorage.ContainsKey(tagKey))//.StatusDictionary.ContainsKey(tagKey))
            {
                InMemoryStorage.UpdateStatus(tagKey, newStatus);//.StatusDictionary[tagKey] = newStatus;
            }
            else
            {
                InMemoryStorage.Add(tagKey, newStatus);
            }
        }

        public bool DoesScopeKeyExists(string tagKey)
        {
            return InMemoryStorage.ContainsKey(tagKey);
        }
        public void Clear()
        {
            InMemoryStorage.Clear();
        }


        public event StatusChangedEventHandler StatusChanged;
        void MemoryStorageStatusChanged(object sender, StatusChangedEventArgs e)
        {
            OnStatusChanged(e);
        }

        protected void OnStatusChanged(StatusChangedEventArgs args)
        {
            if(StatusChanged != null)
                StatusChanged(this, args);
        }
    }

    public static class InMemoryStorage
    {
        public static StatusChangedEventHandler StatusUpdatedEvent;
        
        public static void UpdateStatus(string scopeKey, bool value){
            StatusDictionary[scopeKey] = value;
            StatusUpdatedEvent(scopeKey, new StatusChangedEventArgs(scopeKey));
        }
        public static bool GetStatus(string scopeKey){
            return StatusDictionary[scopeKey];
        }

        public static bool ContainsKey(string scopeKey)
        {
            return StatusDictionary.ContainsKey(scopeKey);
        }

        public static void Add(string scopeKey, bool value)
        {
            StatusDictionary.Add(scopeKey, value);
        }

        public static void Clear()
        {
            StatusDictionary.Clear();
        }
        private static Dictionary<string, bool> StatusDictionary = new Dictionary<string, bool>();
    }
}
