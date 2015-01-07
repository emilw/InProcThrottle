using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs args);
    public interface IClientCommunicationProvider
    {
        bool DoesScopeKeyExists(string tagKey);
        bool IsOkToRun(string tagKey);
        void Clear();

        event StatusChangedEventHandler StatusChanged;
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(string scopeKey)
        {
            ScopeKey = scopeKey;
        }
        public string ScopeKey { get; set; }
    }
}
