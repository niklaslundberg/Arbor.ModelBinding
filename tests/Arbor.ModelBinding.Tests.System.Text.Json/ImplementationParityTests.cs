using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Arbor.ModelBinding.Tests.System.Text.Json
{
    public class ImplementationParityTests
    {
        [Fact]
        public void Parse_bool_from_on_should_match_across_implementations()
        {
            var values = new List<KeyValuePair<string, StringValues>>
            {
                new("Value", "on")
            };

            var newtonsoft = (BoolContainer?)Arbor.ModelBinding.NewtonsoftJson.FormsExtensions.ParseFromPairs(values, typeof(BoolContainer));
            var systemTextJson = (BoolContainer?)Arbor.ModelBinding.SystemTextJson.FormsExtensions.ParseFromPairs(values, typeof(BoolContainer));

            newtonsoft.Should().NotBeNull();
            systemTextJson.Should().NotBeNull();
            systemTextJson!.Value.Should().Be(newtonsoft!.Value);
        }

        [Fact]
        public void Parse_invalid_bool_should_throw_for_both()
        {
            var values = new List<KeyValuePair<string, StringValues>>
            {
                new("Value", "invalid")
            };

            Action parseNewtonsoft = () => Arbor.ModelBinding.NewtonsoftJson.FormsExtensions.ParseFromPairs(values, typeof(BoolContainer));
            Action parseSystemTextJson = () => Arbor.ModelBinding.SystemTextJson.FormsExtensions.ParseFromPairs(values, typeof(BoolContainer));

            parseNewtonsoft.Should().Throw<ArgumentException>();
            parseSystemTextJson.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Parse_invalid_nullable_bool_should_throw_for_both()
        {
            var values = new List<KeyValuePair<string, StringValues>>
            {
                new("Value", "invalid")
            };

            Action parseNewtonsoft = () => Arbor.ModelBinding.NewtonsoftJson.FormsExtensions.ParseFromPairs(values, typeof(NullableBoolContainer));
            Action parseSystemTextJson = () => Arbor.ModelBinding.SystemTextJson.FormsExtensions.ParseFromPairs(values, typeof(NullableBoolContainer));

            parseNewtonsoft.Should().Throw<ArgumentException>();
            parseSystemTextJson.Should().Throw<ArgumentException>();
        }

        private class BoolContainer
        {
            public bool Value { get; set; }
        }

        private class NullableBoolContainer
        {
            public bool? Value { get; set; }
        }
    }
}
