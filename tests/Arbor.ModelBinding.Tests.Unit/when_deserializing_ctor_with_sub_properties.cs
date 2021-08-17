using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
#if Newtonsoft
using Newtonsoft.Json;
using Arbor.ModelBinding.NewtonsoftJson;
#else
using Arbor.ModelBinding.SystemTextJson;
#endif

using Machine.Specifications;

using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.Tests.Unit
{
    public class when_deserializing_ctor_with_sub_properties
    {
        public class SubType2
        {
            public bool Enabled { get; }

            public bool? Enabled2 { get; }

            public SubType2(bool enabled, bool? enabled2 = null)
            {
                Enabled = enabled;
                Enabled2 = enabled2;
            }
        }

        public class MainTypeCtorMultipleProperties
        {
            public MainTypeCtorMultipleProperties(string a, SubTypeCtor s, SubType2 sub2)
            {
                A = a;
                S = s;
                Sub2 = sub2;
            }

            public string A { get; }

            public SubTypeCtor S { get; }

            public SubType2 Sub2 { get; }
        }

        static object result;

        static List<KeyValuePair<string, StringValues>> values;

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
                     {
                         new("a", "123"),
                         new("s.s1", "234"),
                         new("s.s2", "345"),
                         new("sub2.enabled", "on"),
                         new("sub2.enabled2", "")
                     };
        };

        Because of =
            () => { result = FormsExtensions.ParseFromPairs(values, typeof(MainTypeCtorMultipleProperties)); };

        It should_parse_all_properties = () =>
        {
            result.ShouldNotBeNull();

            var mainType = result as MainTypeCtorMultipleProperties;

            mainType.ShouldNotBeNull();

            mainType.A.ShouldEqual("123");
            mainType.S.ShouldNotBeNull();
            mainType.S.S1.ShouldEqual("234");
            mainType.S.S2.ShouldEqual("345");
            mainType.Sub2.Enabled.ShouldBeTrue();
            mainType.Sub2.Enabled2.ShouldBeNull();
        };
    }
}