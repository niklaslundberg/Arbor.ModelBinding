using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;

using Machine.Specifications;

using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;

namespace Arbor.ModelBinding.Tests.Unit
{
    public class when_deserializing_ctor_with_subproperty
    {
        static object result;

        static List<KeyValuePair<string, StringValues>> values;

        Establish context = () =>
            {
                values = new List<KeyValuePair<string, StringValues>>
                             {
                                 new KeyValuePair<string, StringValues>("a", "123"),
                                 new KeyValuePair<string, StringValues>("s.s1", "234"),
                                 new KeyValuePair<string, StringValues>("s.s2", "345")
                             };
            };

        Because of =
            () => { result = FormsExtensions.ParseFromPairs(values, typeof(MainTypeCtor)); };

        It should_parse_all_properties = () =>
            {
                result.ShouldNotBeNull();

                var mainType = result as MainTypeCtor;

                mainType.ShouldNotBeNull();

                mainType.A.ShouldEqual("123");
                mainType.S.ShouldNotBeNull();
                mainType.S.S1.ShouldEqual("234");
                mainType.S.S2.ShouldEqual("345");
            };
    }
}
