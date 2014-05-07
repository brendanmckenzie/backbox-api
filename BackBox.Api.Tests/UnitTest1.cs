using System;
using System.Linq;
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
            BackBox.Api.Controllers.ApiController.TestOverrideLastCheck(new DateTime(1970, 1, 1));
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

            var id = controller.Connect();

            var ret = controller.SetBounds(TestLat, TestLng, TestRadius);

            Assert.AreEqual(ret, string.Format("ok, i'll let you know about messages {2}km around ({0}, {1})", TestLat, TestLng, TestRadius));

            // should probably check database contents here also
        }

        [TestMethod]
        public void Send()
        {
            const double TestLat = 10.0;
            const double TestLng = 10.0;
            const string TestMessage = "wazzaaaaaaaaap.";

            var controller = new BackBox.Api.Controllers.ApiController();

            var id = controller.Connect();

            var message = controller.Send(TestLat, TestLng, TestMessage);

            Assert.AreNotEqual(Guid.Empty, message);

            // test database content
        }

        [TestMethod]
        public void GetLatest()
        {
            const double Lat1 = 10.0;
            const double Lng1 = 10.0;
            const string Message1 = "INSIDE RANGE";

            const double Lat2 = 90.0;
            const double Lng2 = 180.0;
            const string Message2 = "OUTSIDE RANGE";

            var controller = new BackBox.Api.Controllers.ApiController();

            var userId = controller.Connect();

            controller.SetBounds(Lat1, Lng1, 10);

            controller.Send(Lat1, Lng1, Message1);

            controller.Send(Lat2, Lng2, Message2);

            var messages = controller.GetLatest();

            System.Diagnostics.Debug.WriteLine(messages);

            Assert.IsTrue(messages.Contains(Message1));
            Assert.IsFalse(messages.Contains(Message2));
        }
    }
}
