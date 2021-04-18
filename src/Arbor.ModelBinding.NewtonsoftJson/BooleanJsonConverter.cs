using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.NewtonsoftJson
{
    // Taken from https://gist.github.com/randyburden/5924981

    /// <summary>
    /// Handles converting JSON string values into a C# boolean data type.
    /// </summary>
    internal class BooleanJsonConverter : JsonConverter
    {
        private readonly Dictionary<string, bool> _dictionary;

        public BooleanJsonConverter() =>
            _dictionary = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
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

        /// <summary>
        /// Specifies that this converter will not participate in writing results.
        /// </summary>
        public override bool CanWrite => false;

        public override bool CanRead => true;

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) => objectType == typeof(bool) || objectType == typeof(bool?);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            object? readerValue = reader.Value;

            if (reader.Value is bool value)
            {
                return value;
            }

            if (readerValue is string stringValue)
            {
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    if (objectType == typeof(bool?))
                    {
                        return null;
                    }

                    return false;
                }

                if (_dictionary.TryGetValue(stringValue, out bool result))
                {
                    return result;
                }

                throw new FormatException($"Cannot parse {objectType.FullName} from value '{stringValue}'");
            }

            if (objectType == typeof(bool?) && reader.Value is null && readerValue is null)
            {
                return null;
            }

            // If we reach here, we're pretty much going to throw an error so let's let Json.NET throw it's pretty-field error message.
            return serializer.Deserialize(reader, objectType);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
