
using System;
using System.Collections.Generic;

namespace FWF.Json
{
    public class JsonGeneric : IJsonConvertable
    {

        private List<JsonTokenItem> _items = new List<JsonTokenItem>();

        private class JsonTokenItem
        {
            public JsonToken Token { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return string.Format(
                    "{0}: {1}",
                    this.Token,
                    this.Value
                    );
            }
        }

        public void ToJson(IJsonWriter writer)
        {
            foreach (var item in _items)
            {
                writer.WriteToken(item.Token, item.Value);
            }
        }

        public void FromJson(IJsonReader reader)
        {
            _items.Clear();

            if (reader.TokenType != JsonToken.StartObject)
            {
                throw new InvalidOperationException("Json reader must be at the start of an object");
            }

            _items.Add(
                new JsonTokenItem
                {
                    Token = JsonToken.StartObject,
                });

            var initialDepth = reader.Depth;

            while (true)
            {
                if (reader.TokenType == JsonToken.EndObject && reader.Depth == initialDepth)
                {
                    break;
                }

                var didRead = reader.Read();

                if (!didRead)
                {
                    break;
                }

                var item = new JsonTokenItem
                {
                    Token = reader.TokenType,
                    Value = reader.Value,
                };

                _items.Add(item);
            }

        }


    }
}
