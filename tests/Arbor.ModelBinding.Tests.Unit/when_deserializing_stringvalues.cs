using System;
using System.Collections.Generic;
using Machine.Specifications;

using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;

using Newtonsoft.Json;

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
                         new KeyValuePair<string, StringValues>(
                             "values",
                             new StringValues(new[] { "a", "b", "c", "d", "e" }))
                     };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as TypeWithStringValues;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_string_values = () => target.Values.Count.ShouldEqual(5);

    }
}