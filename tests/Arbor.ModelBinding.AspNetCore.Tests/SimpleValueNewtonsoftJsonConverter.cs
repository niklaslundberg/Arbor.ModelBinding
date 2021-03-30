using System;
using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class SimpleValueNewtonsoftJsonConverter : JsonConverter<SimpleValue>
    {
        public override void WriteJson(JsonWriter writer, SimpleValue? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.Value);
            }
        }

        public override SimpleValue? ReadJson(JsonReader reader, Type objectType, SimpleValue? existingValue, bool hasExistingValue,
            JsonSerializer serializer) =>
            existingValue ?? (reader.Value is string stringValue ? new SimpleValue(stringValue) : null);
    }
}