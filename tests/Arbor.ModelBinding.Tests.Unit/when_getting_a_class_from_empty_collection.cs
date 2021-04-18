using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.SampleTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_getting_a_class_from_empty_collection
    {
        Establish context = () => values = new List<KeyValuePair<string, StringValues>>();

        Because of = () => result = FormsExtensions.ParseFromPairs(values, typeof(ItemWithBool)) as ItemWithBool;

        It should_return_null = () => result.ShouldBeNull();

        static List<KeyValuePair<string, StringValues>> values;
        static ItemWithBool result;
    }
}
