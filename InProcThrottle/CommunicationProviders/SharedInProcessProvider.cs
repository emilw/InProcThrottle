using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle.CommunicationProviders
{
    public class SharedInProcessProvider : ICommunicationProvider
    {
        public SharedInProcessProvider()
        {

        }
        
        public bool IsOkToRun(string tagKey)
        {
            return InMemmoryStorage.StatusDictionary[tagKey];
        }

        void ICommunicationProvider.UpdateStatus(string tagKey, bool newStatus)
        {
            if (InMemmoryStorage.StatusDictionary.ContainsKey(tagKey))
                InMemmoryStorage.StatusDictionary[tagKey] = newStatus;
            else
                InMemmoryStorage.StatusDictionary.Add(tagKey, newStatus);
        }

        public bool DoesScopeKeyExists(string tagKey)
        {
            return InMemmoryStorage.StatusDictionary.ContainsKey(tagKey);
        }


        public void Clear()
        {
            InMemmoryStorage.StatusDictionary.Clear();
        }
    }

    public static class InMemmoryStorage
    {
        public static Dictionary<string, bool> StatusDictionary = new Dictionary<string, bool>();
    }
}
