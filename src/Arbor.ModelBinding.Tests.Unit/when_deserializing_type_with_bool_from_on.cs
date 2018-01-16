using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Core;
using Arbor.ModelBinding.Tests.Unit.SampleTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_type_with_bool_from_on
    {
        static object result;
        static Type targetType = typeof(ItemWithBool);
        static ItemWithBool target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("enabled", "on"),
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, typeof(ItemWithBool));
                target = result as ItemWithBool;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_bool_property_set_to_true = () => target.Enabled.ShouldBeTrue();
    }
}