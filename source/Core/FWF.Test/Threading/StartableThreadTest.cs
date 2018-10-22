using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using FWF.Threading;
using Autofac;

namespace FWF.Test.Threading
{
    [TestFixture]
    public class StartableThreadTest
    {

        private int _loopMethodCalled;

        [SetUp]
        public void Setup()
        {
            var container = TestApplicationState.Container;
        }

        [TearDown]
        public void TearDown()
        {

        }


        [Test]
        public void Test()
        {
            var thread = TestApplicationState.Container.Resolve<StartableThread>();

            thread.Name = "test";
            thread.ThreadLatency = TimeSpan.FromMilliseconds(10);
            thread.ThreadLoop = LoopMethod;
            
            //
            _loopMethodCalled = 0;

            thread.Start();

            Thread.Sleep(150);

            Assert.Greater(_loopMethodCalled, 1);

            thread.Stop();
        }


        private Task LoopMethod(IThreadLoopEvent loopEvent)
        {
            _loopMethodCalled++;

            return Task.CompletedTask;
        }

        [Test]
        public void TestWithFailure()
        {
            var thread = TestApplicationState.Container.Resolve<StartableThread>();

            thread.Name = "test";
            thread.ThreadLatency = TimeSpan.FromMilliseconds(10);
            thread.ThreadLoop = LoopMethodFailure;

            //
            _loopMethodCalled = 0;
            
            thread.Start();

            Thread.Sleep(150);
            
            thread.Stop();

            Assert.Greater(_loopMethodCalled, 1);
        }

        private Task LoopMethodFailure(IThreadLoopEvent loopEvent)
        {
            _loopMethodCalled++;

            throw new Exception("Something bad happened");
        }

        [Test]
        public void TestWithFailureAsync()
        {
            var thread = TestApplicationState.Container.Resolve<StartableThread>();

            thread.Name = "test";
            thread.ThreadLatency = TimeSpan.FromMilliseconds(10);
            thread.ThreadLoop = LoopMethodFailure2;

            //
            _loopMethodCalled = 0;

            thread.Start();

            Thread.Sleep(150);

            thread.Stop();

            Assert.Greater(_loopMethodCalled, 1);
        }

        private async Task LoopMethodFailure2(IThreadLoopEvent loopEvent)
        {
            _loopMethodCalled++;

            await Task.Run(() => 
            {
                throw new Exception("Something bad happened");
            }
            );
        }






    }
}



