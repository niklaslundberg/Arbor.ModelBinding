using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arbor.ModelBinding.SystemTextJson
{
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        private static readonly Dictionary<string, bool> BooleanMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { "true", true },
            { "y", true },
            { "yes", true },
            { "1", true },
            { "on", true },
            { "false", false },
            { "n", false },
            { "no", false },
            { "0", false },
            { "off", false }
        };

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.String && reader.GetString() is { } stringValue &&
                BooleanMappings.TryGetValue(stringValue, out bool result))
            {
                return result;
            }

            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
    }
}