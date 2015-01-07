using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InProcThrottle.Manager;
using InProcThrottle.CommunicationProviders;
using InProcThrottle.Client;
using InProcThrottle;

namespace UnitTests
{
    /// <summary>
    /// Summary description for ResourceSamplingTests
    /// </summary>
    [TestClass]
    public class ResourceSamplingTestBase<T> where T: IManagerCommunicationProvider, IClientCommunicationProvider, new()
    {

        [TestMethod]
        public void WaitFor10SamplesToBeMadeAndVerifyAverage()
        {
            HelpersAndMocks.TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            while (ThrottleManager.GetCurrentSampleCount() < 10)
            {
            }
            Assert.IsTrue(ThrottleManager.CPUAverage > 0);
        }

        [TestMethod]
        public void VerifyManagerStatusWithPreparedAverageTests()
        {
            HelpersAndMocks.TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            while (ThrottleManager.CPUAverage == 0)
            {
            }
            Assert.IsTrue(ThrottleManager.CPUAverage > 0);
            ThrottleManager.Config<T>("Test", ThrottleManager.CPUAverage+1, 360);
            Assert.IsTrue(ThrottleManager.IsItOkToRun("Test"));
        }

        [TestMethod]
        public void VerifyClientStatusWithPreparedAverageTestsToBeOk()
        {
            HelpersAndMocks.TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 80, 360);
            while (ThrottleManager.CPUAverage == 0)
            {
            }
            Assert.IsTrue(ThrottleManager.CPUAverage > 0);
            ThrottleManager.Config<T>("Test", ThrottleManager.CPUAverage + 1, 360);
            Assert.IsTrue(ThrottleManager.IsItOkToRun("Test"));
            Assert.IsTrue(ThrottleClient.CanIRun<T>("Test"));
        }

        [TestMethod]
        public void VerifyClientStatusWithPreparedAverageTestsToNotBeOk()
        {
            HelpersAndMocks.TestHelper.Reset();
            ThrottleManager.Config<T>("Test", 0, 360);
            while (ThrottleManager.CPUAverage == 0)
            {
            }
            Assert.IsTrue(ThrottleManager.CPUAverage > 0);
            Assert.IsFalse(ThrottleManager.IsItOkToRun("Test"), "Manager signals ok");
            System.Threading.Thread.Sleep(3000);
            Assert.IsTrue(ThrottleManager.CPUAverage > 0);
            Assert.IsFalse(ThrottleClient.CanIRun<T>("Test"), "Client signals ok");
        }
    }
}
