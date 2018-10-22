using Autofac;
using NUnit.Framework;
using FWF.Configuration;
using System;

namespace FWF.Test.Configuration
{
    [TestFixture]
    public class InMemoryAppSettingsTest
    {

        private IAppSettings _appSettings;

        [SetUp]
        public void Setup()
        {
            _appSettings = TestApplicationState.Container.Resolve<IAppSettings>();
            _appSettings.Start();
        }
        
        [Test]
        public void AppSettings()
        {
            Assert.AreNotEqual(Guid.Empty, _appSettings.EnvironmentName);
        }


    }
}

