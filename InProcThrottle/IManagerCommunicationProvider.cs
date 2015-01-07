using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InProcThrottle
{
    public interface IManagerCommunicationProvider
    {
        void UpdateStatus(string tagKey, bool newStatus);
        void Clear();
    }
}
