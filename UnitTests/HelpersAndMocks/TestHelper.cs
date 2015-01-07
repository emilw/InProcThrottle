using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InProcThrottle.Client;
using InProcThrottle.Manager;

namespace UnitTests.HelpersAndMocks
{
    public static class TestHelper
    {
        #region Helpers
        public static void Reset()
        {
            ThrottleManager.Clear();
            ThrottleClient.Clear();
        }
        #endregion Helpers
    }
}
