using System;
using System.Collections.Generic;
using System.Text.Json;
using Arbor.ModelBinding.Core;
using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.SystemTextJson
{
    public static class FormsExtensions
    {
        //private static readonly JsonConverter[] Converters = {new BooleanJsonConverter(), new StringValuesJsonConverter()};

        private static JsonSerializerOptions GetOptions() => new(JsonSerializerDefaults.Web);

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
}