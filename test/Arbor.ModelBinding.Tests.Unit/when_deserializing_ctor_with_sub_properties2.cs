using System.Collections.Generic;
using Arbor.ModelBinding.NewtonsoftJson;

using Machine.Specifications;

using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.Tests.Unit
{
    public class when_deserializing_ctor_with_sub_properties2
    {
        public class SubType3
        {
            public SubType3(bool enabled, bool? enabled2 = null)
            {
                Enabled = enabled;
                Enabled2 = enabled2;
            }

            public bool Enabled { get; }

            public bool? Enabled2 { get; }
        }

        static object result;

        static List<KeyValuePair<string, StringValues>> values;

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
                     {
                         new KeyValuePair<string, StringValues>("enabled", "on"),
                         new KeyValuePair<string, StringValues>("enabled2", "true")
                     };
        };

        Because of =
            () => { result = FormsExtensions.ParseFromPairs(values, typeof(SubType3)); };

        It should_parse_all_properties = () =>
        {
            result.ShouldNotBeNull();

            var mainType = result as SubType3;

            mainType.ShouldNotBeNull();

            mainType.Enabled.ShouldBeTrue();
            mainType.Enabled2.HasValue.ShouldBeTrue();
            mainType.Enabled2.Value.ShouldBeTrue();
        };
    }
}