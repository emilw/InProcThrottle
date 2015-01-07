using System;
using InProcThrottle;
using InProcThrottle.Client;
using InProcThrottle.CommunicationProviders;
using InProcThrottle.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HelpersAndMocks;

namespace UnitTests
{
    [TestClass]
    public class BasicTestBase<T> where T: IClientCommunicationProvider, IManagerCommunicationProvider, new()
    {
        [TestMethod]
        public void BasicAccurateConfiguration()
        {
            TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            Assert.IsTrue(ThrottleManager.Configs.Count > 0);
            while (ThrottleManager.GetCurrentSampleCount() < 2) { }
            Assert.IsTrue(ThrottleClient.CanIRun<T>("Test"));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StartClientWithUnInitializedScopeKey()
        {
            TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            Assert.IsTrue(ThrottleManager.Configs.Count > 0);
            ThrottleClient.CanIRun<T>("Test2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StartClientWithoutInitializingManager()
        {
            TestHelper.Reset();
            Assert.IsTrue(ThrottleManager.Configs.Count == 0);
            ThrottleClient.CanIRun<T>("Test2");
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReconfigureManagerWithNewProvider()
        {
            TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            ThrottleManager.Config<MyDummyCommunicationProvider>("Test", 80, 360);
        }

        [TestMethod]
        public void ReconfigureManagerWithSameProvider()
        {
            TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            Assert.IsTrue(ThrottleManager.Configs["Test"].Percentage == 80);
            Assert.IsTrue(ThrottleManager.Configs["Test"].TimeSpan == 360);
            ThrottleManager.Config<T>("Test", 10, 10);
            Assert.IsTrue(ThrottleManager.Configs["Test"].Percentage == 10);
            Assert.IsTrue(ThrottleManager.Configs["Test"].TimeSpan == 10);
        }

        [TestMethod]
        public void StatusRecordChangedEventWithQueuedItem()
        {
            TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 0, 360);
            while (ThrottleManager.GetCurrentSampleCount() < 2)
            {
            }
            var blnEnd = false;
            Assert.IsFalse(ThrottleClient.CanIRun<T>("Test", () => { blnEnd = true; }), "The client was allowed to run");
            Assert.IsTrue(ThrottleClient.QueueCount == 1, "Queue count is not 1");
            ThrottleManager.Config<T>("Test", 100, 360);
            while(!blnEnd)
            {}

            Assert.IsTrue(blnEnd, "Delegate action was not called");
        }
    }
}
