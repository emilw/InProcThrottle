using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle
{
    public interface ICommunicationProvider
    {
        bool DoesScopeKeyExists(string tagKey);
        bool IsOkToRun(string tagKey);
        void UpdateStatus(string tagKey, bool newStatus);
        void Clear();
    }
}
