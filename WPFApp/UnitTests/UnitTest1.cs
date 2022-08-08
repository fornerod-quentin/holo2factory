using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFApp;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        ControllerClient ControllerClient { get;} = new ControllerClient();
        [TestMethod]
        public void TestDetectNetwork()
        {
            try
            {
                ControllerClient.DetectNetwork("127.0.0.1");
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
            try
            {
                ControllerClient.DetectNetwork("Misoune");
                Assert.IsFalse(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void TestPutRequest()
        {
            SingleMeasure singleMeasure = new SingleMeasure();
            try
            {
                ControllerClient.PutMeasure(singleMeasure, "127.0.0.1", 5114);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsFalse(false);
            }
            try
            {
                ControllerClient.PutMeasure(singleMeasure, "127.0.2.1", 5114);
                Assert.IsFalse(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
            try
            {
                ControllerClient.PutMeasure(singleMeasure, "127.0.1.1", 35);
                Assert.IsFalse(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }
    }
}