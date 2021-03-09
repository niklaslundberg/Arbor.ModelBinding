using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public static class CustomJsonConverters
    {
        public static readonly ImmutableArray<JsonConverter> Converters =
            new List<JsonConverter>()
            {

                new MyConverter() //Generate
            }.ToImmutableArray();
    }
}