using System;
using Autofac;
using NUnit.Framework;

using FWF.Json;

namespace FWF.Test.Extensions
{
    [TestFixture]
    public class JsonExtensionsTest
    {

        private IRandom _random;

        public class MyJsonObject : IJsonConvertable
        {
            public string JsonType { get { return "Extensions.Tests.MyJsonObject"; } }

            public string Name
            {
                get;
                set;
            }

            public void ToJson(IJsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Name");
                writer.Write(this.Name);
                writer.WriteEndObject();
            }

            public void FromJson(IJsonReader reader)
            {
                reader.Read();
                this.Name = reader.Read(string.Empty);
                reader.Read();
            }

        }

        public class MyPlainObject
        {
            public Guid Id
            {
                get; set;
            }

            public string Name
            {
                get; set;
            }

            public int Value
            {
                get; set;
            }

            public DateTime? Timestamp
            {
                get; set;
            }

            public byte[] DataBlock
            {
                get; set;
            }

        }

        [SetUp]
        public void Setup()
        {
            _random = TestApplicationState.Container.Resolve<IRandom>();
        }

        [Test]
        public void JsonCloneConvertableObject()
        {
            var myJsonObject = new MyJsonObject { Name = "testing" };

            var clonedObject = myJsonObject.JsonClone();

            Assert.IsFalse(ReferenceEquals(myJsonObject, clonedObject));
            Assert.AreEqual(myJsonObject.Name, clonedObject.Name);
        }

        [Test]
        public void JsonCloneConverterObject()
        {
            var appVersion = new AppVersion(4, 5);

            var clonedObject = appVersion.JsonClone();

            Assert.IsFalse(ReferenceEquals(appVersion, clonedObject));
            Assert.AreEqual(appVersion.Major, clonedObject.Major);
            Assert.AreEqual(appVersion.Minor, clonedObject.Minor);
        }

        [Test]
        public void JsonCloneAnyObject()
        {
            var myObject = new MyPlainObject
            {
                Id = Guid.NewGuid(),
                Name = _random.AnyString(8),
                Timestamp = DateTime.UtcNow,
                Value = _random.Any(),
                DataBlock = _random.AnyBytes(128),
            };

            var clonedObject = myObject.JsonClone();

            Assert.IsFalse(ReferenceEquals(myObject, clonedObject));

            Assert.AreEqual(myObject.Id, clonedObject.Id);
            Assert.AreEqual(myObject.Name, clonedObject.Name);
            Assert.AreEqual(myObject.Timestamp, clonedObject.Timestamp);
            Assert.AreEqual(myObject.Value, clonedObject.Value);
            Assert.AreEqual(myObject.DataBlock, clonedObject.DataBlock);
        }

    }
}


