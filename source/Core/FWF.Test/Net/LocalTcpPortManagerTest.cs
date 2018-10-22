using Autofac;
using NUnit.Framework;
using FWF.Net;

namespace FWF.Test.Net
{
    [TestFixture]
    public class LocalTcpPortManagerTest
    {

        private ILocalTcpPortManager _localTcpPortManager;

        [SetUp]
        public void Setup()
        {
            _localTcpPortManager = TestApplicationState.Container.Resolve<ILocalTcpPortManager>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GetNextAvailablePort()
        {
            var port = _localTcpPortManager.GetNextAvailablePort();

            Assert.AreNotEqual(-1, port);
        }
    }
}

