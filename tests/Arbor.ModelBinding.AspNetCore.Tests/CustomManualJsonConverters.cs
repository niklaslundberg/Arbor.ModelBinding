using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public static class CustomManualJsonConverters
    {
        public static readonly ImmutableArray<JsonConverter> Converters =
            new List<JsonConverter>
            {
                new MyValueObjectJsonConverter(),
                //new Partial1ParserJsonConverter()

            }.ToImmutableArray();
    }
}