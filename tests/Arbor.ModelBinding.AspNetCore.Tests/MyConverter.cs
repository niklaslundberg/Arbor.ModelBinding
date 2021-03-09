using System;
using System.Text.Json;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class MyConverter : System.Text.Json.Serialization.JsonConverter<MyValueObject>
    {
        public override MyValueObject? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.GetString() is { } value ? new MyValueObject(value) : null;

        public override void Write(Utf8JsonWriter writer, MyValueObject value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
    }
}