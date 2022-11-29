using System;
using System.Collections.Generic;
using Machine.Specifications;

using Microsoft.Extensions.Primitives;
#if Newtonsoft
using Newtonsoft.Json;
using Arbor.ModelBinding.NewtonsoftJson;
#else
using Arbor.ModelBinding.SystemTextJson;
#endif

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_stringvalues
    {
        public class TypeWithStringValues
        {
            public StringValues Values { get; }

            public TypeWithStringValues(StringValues values)
            {
                Values = values;
            }
        }
        static object result;
        static Type targetType = typeof(TypeWithStringValues);
        static TypeWithStringValues target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        private Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
                     {
                         new(
                             "values",
                             new StringValues(new[] { "a", "b", "c", "d", "e" }))
                     };
        };

        Because of =
            () =>
            {
                result = values.ParseFromPairs(targetType);
                target = result as TypeWithStringValues;
            };

        It should_have_string_values = () => target.Values.Count.ShouldEqual(5);

        It should_have_all_string_values = () => target.Values.ShouldContain("a", "b", "c", "d", "e");

    }
}