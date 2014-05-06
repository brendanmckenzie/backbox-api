using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackBox.Api.Tests
{
    [TestClass]
    public class ApiTests
    {
        Guid sessionId = Guid.NewGuid();

        [TestInitialize]
        public void Initialise()
        {
            BackBox.Api.Controllers.ApiController.TestOverrideSessionId(sessionId);
        }

        [TestMethod]
        public void Connect()
        {
            var controller = new BackBox.Api.Controllers.ApiController();

            var ret = controller.Connect();

            Assert.AreEqual(ret, sessionId);
        }

        [TestMethod]
        public void SetName()
        {
            const string TestName = "TEST_USER";

            var controller = new BackBox.Api.Controllers.ApiController();

            var id = controller.Connect();

            var ret = controller.SetName(TestName);

            Assert.AreEqual(ret, "ok, " + TestName);

        }

        [TestMethod]
        public void SetBounds()
        {
            const double TestLat = 10.0;
            const double TestLng = 10.0;
            const int TestRadius = 5;

            var controller = new BackBox.Api.Controllers.ApiController();

            var ret = controller.SetBounds(TestLat, TestLng, TestRadius);

            Assert.AreEqual(ret, string.Format("ok, i'll let you know about messages {2}km around ({0}, {1})", TestLat, TestLng, TestRadius));
        }

        [TestMethod]
        public void Send()
        {
            // needs to test that sending works and message appears in correct location.
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GetLatest()
        {
            Assert.Inconclusive();
        }
    }
}
