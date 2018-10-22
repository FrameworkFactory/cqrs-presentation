using System;
using System.Collections.Generic;
using NUnit.Framework;
using Autofac;
using FWF.Logging;

namespace FWF.Test
{
    [TestFixture]
    public class RngRandomTest
    {

        private IRandom _random;
        private ILog _log;

        [SetUp]
        public void Setup()
        {
            _log = TestApplicationState.Container.Resolve<ILogFactory>().CreateForType(this);

            _random = TestApplicationState.Container.Resolve<IRandom>();
        }

        [TearDown]
        public void Teardown()
        {
        }

        [Test]
        public void Any()
        {
            var listR = new List<int>();

            for (int i = 0; i < 500; i++)
            {
                listR.Add(_random.Any());
            }
            
            foreach (var i in listR)
            {
                Assert.Greater(i, 0);
            }
        }

        [Test]
        public void AnyWithMax()
        {
            var listR = new List<int>();

            const int maxValue = 10000;
            const int testRange = (int)(maxValue * 0.001);

            for (int i = 0; i < testRange; i++)
            {
                listR.Add(_random.Any(maxValue));
            }
            
            foreach (var i in listR)
            {
                Assert.LessOrEqual(i, maxValue);
            }
        }

        [Test]
        public void AnyWithMinAndMax()
        {
            var listR = new List<int>();

            const int minValue = 10;
            const int maxValue = 100000;
            const int testRange = 20;

            for (int i = 0; i < testRange; i++)
            {
                listR.Add(_random.Any(maxValue));
            }
            
            foreach (var i in listR)
            {
                Assert.GreaterOrEqual(i, minValue);
                Assert.LessOrEqual(i, maxValue);
            }
        }

        [Test]
        public void AnyWithLongMinAndMax()
        {
            var listR = new List<long>();

            const long minValue = 1000000000000000L;
            const long maxValue = 9999999999999999L;
            const int testRange = 500;

            for (int i = 0; i < testRange; i++)
            {
                listR.Add(_random.Any(minValue, maxValue));
            }

            foreach (var i in listR)
            {
                Assert.GreaterOrEqual(i, minValue);
                Assert.LessOrEqual(i, maxValue);
            }
        }

        [Test]
        public void AnyString()
        {
            const int length = 128;
            string x = _random.AnyString(length);
            Assert.AreEqual(length, x.Length);
        }

        [Test]
        public void AnySequence()
        {
            var sequence1 = _random.AnySequence(3);
            var sequence2 = _random.AnySequence(255);
        }

        [Test]
        public void AnyInList()
        {
            var numberOfUniqueItems = 5;
            var numberOfOccurrances = 2000;
            var averageOccurrances = (numberOfOccurrances / numberOfUniqueItems);

            // We expect randomization to occur where every item in the sequence
            // if not the average.  Larger result sets should move towards
            // a more consistant average.  For this test, we allow for a large amount of randomness.
            var minAcceptedOccurrances = averageOccurrances * 0.1;
            
            var listR = new List<int>();

            for (int i = 0; i < numberOfUniqueItems; i++)
            {
                listR.Add(i);
            }

            var numberOfTimesEachIndexHasBeenSeen = new int[numberOfUniqueItems];
            for(int i= 0; i < numberOfOccurrances; i++)
            {
                var randomIndex = _random.Any(listR);
                numberOfTimesEachIndexHasBeenSeen[randomIndex]++;
            }
            
            foreach (var count in numberOfTimesEachIndexHasBeenSeen)
            {
                Assert.IsTrue(count > minAcceptedOccurrances);
            }
        }
    }
}




