using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InProcThrottle;

namespace UnitTests.HelpersAndMocks
{
    public class MyDummyCommunicationProvider : IClientCommunicationProvider, IManagerCommunicationProvider
    {
        public bool DoesScopeKeyExists(string tagKey)
        {
            throw new NotImplementedException();
        }

        public bool IsOkToRun(string tagKey)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(string tagKey, bool newStatus)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }


        public event StatusChangedEventHandler StatusChanged;
    }
}
