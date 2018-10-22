using Autofac;

using NUnit.Framework;
using FWF.ComponentModel;
using FWF.CQRS;
using FWF.Security;
using FWF.Test.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FWF.Json;

namespace FWF.Test.CQRS
{
    [TestFixture]
    public class CqrsLogicHandlerTest
    {

        private CqrsLogicHandler _cqrsLogicHandler;
        private SimpleCommandHandler _simpleCommandHandler;
        private SimpleEventHandler _simpleEventHandler;

        [SetUp]
        public void Setup()
        {
            _cqrsLogicHandler = TestApplicationState.Container.Resolve<CqrsLogicHandler>();
            _cqrsLogicHandler.Start();

            _simpleCommandHandler = TestApplicationState.Container.Resolve<SimpleCommandHandler>();
            _simpleEventHandler = TestApplicationState.Container.Resolve<SimpleEventHandler>();
        }

        [TearDown]
        public void TearDown()
        {
        }


        private class SimpleCommand : ICommand
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

        private class SimpleCommandHandler : ICommandHandler<SimpleCommand>
        {
            public bool DidHandle
            {
                get; set;
            }
            
            public ICommandResponse Handle(ISecurityContext securityContext, SimpleCommand command)
            {
                DidHandle = true;

                return new CommandResponse();
            }
        }

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
            public int? ErrorCode { get; set; }

            public string ErrorMessage { get; set; }

            public void ToJson(IJsonWriter writer)
            {

            }
            public void FromJson(IJsonReader reader)
            {

            }
        }

        private class SimpleQueryHandler : IQueryHandler<SimpleQuery>
        {
            public IQueryResponse Handle(ISecurityContext securityContext, SimpleQuery query)
            {
                return new SimpleQueryResponse();
            }
        }

        private class SimpleEvent : IEvent
        {
            public Guid? EventId { get; set; }

            public DateTime? EventTimestamp { get; set; }

            public OperationMask Operation
            {
                get { return OperationMask.Select; }
                set { }
            }
            
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

        private class SimpleEventHandler : IEventHandler<SimpleEvent>
        {
            public bool DidHandle
            {
                get; set;
            }

            public void Handle(ISecurityContext securityContext, SimpleEvent @event)
            {
                DidHandle = true;
            }
        }

        [Test]
        public void HandleCommand()
        {
            var securityContext = new TestSecurityContext();
            var command = new SimpleCommand();

            // Reset handle switch
            _simpleCommandHandler.DidHandle = false;

            var commandResponse = _cqrsLogicHandler.Handle(securityContext, command);

            Assert.IsTrue(_simpleCommandHandler.DidHandle);
        }

        [Test]
        public void HandleQuery()
        {
            var securityContext = new TestSecurityContext();
            var query = new SimpleQuery();

            IQueryResponse queryResponse = null;

            queryResponse = _cqrsLogicHandler.Handle(securityContext, query);

            Assert.IsNotNull(queryResponse);
        }

        [Test]
        public void HandleEvent()
        {
            var securityContext = new TestSecurityContext();
            var @event = new SimpleEvent();

            // Reset handle switch
            _simpleEventHandler.DidHandle = false;

            _cqrsLogicHandler.Handle(securityContext, @event);

            Assert.IsTrue(_simpleEventHandler.DidHandle);
        }


    }
}

