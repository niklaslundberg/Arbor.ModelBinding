using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.SystemTextJson
{
    public class StringValuesJsonConverter : JsonConverter<StringValues>
    {
        public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                string[]? values = JsonSerializer.Deserialize<string[]>(ref reader, options);

                if (values is null || values.Length == 0)
                {
                    return StringValues.Empty;
                }

                if (values.Length == 1)
                {
                    return new StringValues(values[0]);
                }

                return new StringValues(values);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();

                return new StringValues(stringValue);
            }

            return StringValues.Empty;
        }

        public override void Write(Utf8JsonWriter writer, StringValues value, JsonSerializerOptions options)
        {
            if (value.Count == 0)
            {
                writer.WriteStringValue("");
            }

            if (value.Count == 1)
            {
                writer.WriteStringValue(value[0]);
            }

            if (value.Count > 1)
            {
                writer.WriteStartArray();

                foreach (string? item in value)
                {
                    writer.WriteStringValue(item);
                }

                writer.WriteEndArray();
            }
        }
    }
}