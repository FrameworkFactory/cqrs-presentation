using Autofac;

using NUnit.Framework;
using FWF.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FWF.Test.Json
{
    [TestFixture]
    public class JsonGenericTest
    {

        private IRandom _random;

        private class SimpleJsonObject : IJsonConvertable
        {

            public SimpleJsonObject()
            {
                this.Children = new List<SimpleChildJsonObject>();
            }

            public string Name { get; set; }
            public int? Number { get; set; }

            public List<SimpleChildJsonObject> Children { get; set; }

            public void ToJson(IJsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.Write(this.Name);
                writer.WritePropertyName("number");
                writer.Write(this.Number);

                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (var item in Children)
                {
                    item.ToJson(writer);
                }
                writer.WriteEndArray();

                writer.WriteEndObject();
            }
            public void FromJson(IJsonReader reader)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    reader.Read();
                }

                while (true)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        break;
                    }

                    var propertyName = reader.Value as string;

                    switch (propertyName)
                    {
                        case "name":
                            this.Name = reader.Read(string.Empty);
                            break;
                        case "number":
                            this.Number = reader.Read<int?>(null);
                            break;
                        case "children":
                            reader.Read();
                            reader.Read();

                            while(reader.TokenType == JsonToken.StartObject)
                            {
                                var child = new SimpleChildJsonObject();
                                child.FromJson(reader);

                                this.Children.Add(child);
                            }
                            break;
                    }

                    var didRead = reader.Read();

                    if (!didRead)
                    {
                        break;
                    }
                }

                reader.Read(); // EndObject
            }
        }

        private class SimpleChildJsonObject : IJsonConvertable
        {

            public string Name { get; set; }
            public int? Number { get; set; }

            public void ToJson(IJsonWriter writer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.Write(this.Name);
                writer.WritePropertyName("number");
                writer.Write(this.Number);
                writer.WriteEndObject();
            }
            public void FromJson(IJsonReader reader)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    reader.Read();
                }

                while (true)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                    {
                        break;
                    }

                    var propertyName = reader.Value as string;

                    switch (propertyName)
                    {
                        case "name":
                            this.Name = reader.Read(string.Empty);
                            break;
                        case "number":
                            this.Number = reader.Read<int?>(null);
                            break;
                    }

                    var didRead = reader.Read();

                    if (!didRead)
                    {
                        break;
                    }
                }

                reader.Read(); // EndObject
            }
        }

        [SetUp]
        public void Setup()
        {
            _random = TestApplicationState.Container.Resolve<IRandom>();
        }

        [Test]
        public void Test()
        {
            var testObject = new SimpleJsonObject
            {
                Name = _random.AnyString(6),
                Number = _random.Any(),
            };

            for (int i = 0; i < _random.Any(2, 7); i++)
            {
                var childObject = new SimpleChildJsonObject
                {
                    Name = _random.AnyString(6),
                    Number = _random.Any(),
                };

                testObject.Children.Add(childObject);
            }


            var memoryStream = new MemoryStream();

            // Serialize the object
            using (var writer = JSON.GetWriter(memoryStream))
            {
                testObject.ToJson(writer);
            }

            Assert.Greater(memoryStream.Position, 0);

            memoryStream.Position = 0;

            
            // Deserialize back into the original object

            var simpleJsonObjectResponse = new SimpleJsonObject();

            using (var reader = JSON.GetReader(memoryStream))
            {
                reader.Read();

                simpleJsonObjectResponse.FromJson(reader);
            }


        }


        [Test]
        public void TestGeneric()
        {

            // Test that: Object => Generic => Object #2 are identical

            var testObject = new SimpleJsonObject
            {
                Name = _random.AnyString(6),
                Number = _random.Any(),
            };

            for (int i = 0; i < _random.Any(2, 7); i++)
            {
                var childObject = new SimpleChildJsonObject
                {
                    Name = _random.AnyString(6),
                    Number = _random.Any(),
                };

                testObject.Children.Add(childObject);
            }


            var memoryStream = new MemoryStream();

            // Serialize the object
            using (var writer = JSON.GetWriter(memoryStream))
            {
                testObject.ToJson(writer);
            }

            Assert.Greater(memoryStream.Position, 0);

            memoryStream.Position = 0;


            // Deserialize the object into a JsonGeneric

            var jsonGeneric = new JsonGeneric();

            using (var reader = JSON.GetReader(memoryStream))
            {
                reader.Read();

                jsonGeneric.FromJson(reader);
            }


            // Serialize the JsonGeneric to another stream

            var memoryStream2 = new MemoryStream();

            using (var writer2 = JSON.GetWriter(memoryStream2))
            {
                jsonGeneric.ToJson(writer2);
            }

            Assert.Greater(memoryStream2.Position, 0);

            memoryStream2.Position = 0;


            // Deserialize back into the original object

            var simpleJsonObjectResponse = new SimpleJsonObject();

            using (var reader2 = JSON.GetReader(memoryStream2))
            {
                reader2.Read();

                simpleJsonObjectResponse.FromJson(reader2);
            }

        }

    }
}
