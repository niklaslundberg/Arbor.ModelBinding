using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Arbor.ModelBinding.Core;
using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.SystemTextJson
{
    public static class FormsExtensions
    {
        private static readonly JsonConverter[] Converters =
        {
            new BooleanJsonConverter(),
            new NullableBooleanJsonConverter(),
            new StringValuesJsonConverter()
        };

        private static JsonSerializerOptions GetOptions()
        {
            JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);
            foreach (var jsonConverter in Converters)
            {
                jsonSerializerOptions.Converters.Add(jsonConverter);
            }

            return jsonSerializerOptions;
        }

        public static object? ParseFromPairs(
            this IEnumerable<KeyValuePair<string, StringValues>> collection,
            Type targetType,
            JsonSerializerOptions? options = default) => FormsParser.ParseFromPairs(
            collection,
            targetType,
            instance => Serialize(instance, options ?? GetOptions()),
            (json, type) => Deserialize(json, type, options ?? GetOptions()));

        private static string Serialize(object? instance, JsonSerializerOptions? options) =>
            JsonSerializer.Serialize(instance, options);

        private static object? Deserialize(string json, Type targetType, JsonSerializerOptions? options) =>
            JsonSerializer.Deserialize(json, targetType, options);
    }

    public class StringValuesJsonConverter : JsonConverter<StringValues>
    {
        public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var values = JsonSerializer.Deserialize<string[]>(ref reader, options);

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
                writer.WriteStringValue(string.Join(", ", value));
            }
        }
    }

    public class NullableBooleanJsonConverter : JsonConverter<bool?>
    {
        private static readonly Dictionary<string, bool> _dictionary = new(StringComparer.OrdinalIgnoreCase)
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

        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.GetString() is { } stringValue && _dictionary.TryGetValue(stringValue, out var result))
            {
                return result;
            }

            return null;

        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteBooleanValue(value.Value);
        }
    }

    public class BooleanJsonConverter : JsonConverter<bool>
    {
        private static readonly Dictionary<string, bool> _dictionary = new(StringComparer.OrdinalIgnoreCase)
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
                _dictionary.TryGetValue(stringValue, out var result))
            {
                return result;
            }

            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}
