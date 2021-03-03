using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Core;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.NewtonsoftJson
{
    public static class FormsExtensions
    {
        private static readonly JsonConverter[] _converters = {new BooleanJsonConverter(), new StringValuesJsonConverter()};

        public static object? ParseFromPairs(
            IEnumerable<KeyValuePair<string, StringValues>> collection,
            Type targetType) => FormsParser.ParseFromPairs(collection, targetType, Serialize, Deserialize);

        private static string Serialize(object? instance) => JsonConvert.SerializeObject(instance);

        private static object? Deserialize(string json, Type targetType) =>
            JsonConvert.DeserializeObject(json, targetType, _converters);
    }
}