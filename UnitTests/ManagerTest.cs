using System;
using InProcThrottle.Client;
using InProcThrottle.CommunicationProviders;
using InProcThrottle.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class ManagerTest
    {
        [TestMethod]
        public void BasicAccurateConfiguration()
        {
            Reset();
            ThrottleManager.Config<SharedInProcessProvider>("Test", 80, 360);
            Assert.IsTrue(ThrottleManager.Configs.Count > 0);
            Assert.IsTrue(ThrottleClient.CanIRun<SharedInProcessProvider>("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StartClientWithUnInitializedScopeKey()
        {
            Reset();
            ThrottleManager.Config<SharedInProcessProvider>("Test", 80, 360);
            Assert.IsTrue(ThrottleManager.Configs.Count > 0);
            ThrottleClient.CanIRun<SharedInProcessProvider>("Test2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StartClientWithoutInitializingManager()
        {
            Reset();
            Assert.IsTrue(ThrottleManager.Configs.Count == 0);
            ThrottleClient.CanIRun<SharedInProcessProvider>("Test2");
        }

        #region Helpers
        private void Reset()
        {
            ThrottleManager.Clear();
            ThrottleClient.Clear();
        }
        #endregion Helpers
    }
}
