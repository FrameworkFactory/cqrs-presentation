using Autofac;

using NUnit.Framework;
using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Test.Security;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FWF.Json;

namespace FWF.Test.CQRS
{
    [TestFixture]
    public class CQRSRequestorTest
    {

        private ICQRSRequestor _cqrsRequestor;


        private class SimpleQuery : IQuery
        {
            public IEnumerable<ValidationError> Validate()
            {
                yield break;
            }

            public void ToJson(IJsonWriter writer)
            {

            }
            public void FromJson(IJsonReader reader)
            {

            }
        }


        private class SimpleQueryResponse : IQueryResponse
        {
            public int? ErrorCode
            {
                get; set;
            }

            public string ErrorMessage
            {
                get; set;
            }

            public void ToJson(IJsonWriter writer)
            {

            }
            public void FromJson(IJsonReader reader)
            {

            }
        }


        [SetUp]
        public void Setup()
        {
            _cqrsRequestor = TestApplicationState.Container.Resolve<ICQRSRequestor>();
        }

        [TearDown]
        public void TearDown()
        {
        }


        [Test]
        public void Request()
        {
            var simpleQuery = new SimpleQuery();
            SimpleQueryResponse simpleQueryResponse = null;

            Task.Run(async () =>
                simpleQueryResponse = await _cqrsRequestor.RequestAsync<SimpleQuery, SimpleQueryResponse>(
                    Url.Empty,
                    simpleQuery,
                    new TestSecuritySession()
                    )
            ).Wait();

            Assert.IsNotNull(simpleQueryResponse);
        }

        [Test]
        public void RequestBulk()
        {

            var simpleQuery = new SimpleQuery();
            CQRSRequestorBuilderResponse bulkResponse = null;

            Task.Run(async () =>
                bulkResponse = await _cqrsRequestor.Build()
                    .With<SimpleQuery, SimpleQueryResponse>(simpleQuery)
                    .With<SimpleQuery, SimpleQueryResponse>(simpleQuery)
                    .With<SimpleQuery, SimpleQueryResponse>(simpleQuery)
                    .Go(Url.Empty, new TestSecuritySession())

            ).Wait();

            Assert.IsNotNull(bulkResponse);
            Assert.AreEqual(3, bulkResponse.Count);
        }

    }
}

