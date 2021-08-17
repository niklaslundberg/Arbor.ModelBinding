using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.SampleTypes;
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
                new("enabled", "on"),
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, typeof(ItemWithBool));
                target = result as ItemWithBool;
            };

        It should_have_bool_property_set_to_true = () => target.Enabled.ShouldBeTrue();
    }
}