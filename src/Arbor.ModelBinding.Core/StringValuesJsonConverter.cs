using System;
using System.Linq;

using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arbor.ModelBinding.Core
{
    internal class StringValuesJsonConverter : JsonConverter
    {
        public override bool CanRead { get; } = true;

        public override bool CanWrite { get; } = false;

        public override bool CanConvert(Type objectType) => objectType == typeof(StringValues);

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var array = JArray.Load(reader);

                var strings = array.OfType<JValue>().Select(value => value.Value as string).ToArray();

                return new StringValues(strings);
            }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-fied error message.
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotSupportedException();
    }
}