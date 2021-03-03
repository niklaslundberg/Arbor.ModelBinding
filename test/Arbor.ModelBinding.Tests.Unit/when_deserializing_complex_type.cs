using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
using Arbor.ModelBinding.NewtonsoftJson;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_complex_type
    {
        static object result;
        static Type targetType = typeof(ItemWithServices);
        static ItemWithServices target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("description", "myDescription"),
                new KeyValuePair<string, StringValues>("numberOfItems", "33"),
                new KeyValuePair<string, StringValues>("services[0].title", "myFirstServiceTitle"),
                new KeyValuePair<string, StringValues>("services[0].otherProperty", "42"),
                new KeyValuePair<string, StringValues>("services[1].title", "mySecondServiceTitle"),
                new KeyValuePair<string, StringValues>("services[1].otherProperty", "123")
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, typeof(ItemWithServices));
                target = result as ItemWithServices;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_other_simple_property_set = () => target.NumberOfItems.ShouldEqual(33);

        It should_have_simple_property_set = () => target.Description.ShouldEqual("myDescription");

        It should_not_be_null = () => target.Services.ShouldNotBeNull();

        It should_return_an_object_of_type_the_requested_type =
            () => result.ShouldBeOfExactType<ItemWithServices>();

        It should_return_first_nested_object_with_property_set =
            () => target.Services[0].Title.ShouldEqual("myFirstServiceTitle");

        It should_return_nested_objects = () => target.Services.Count.ShouldEqual(2);

        It should_return_second_nested_object_with_property_set =
            () => target.Services[1].Title.ShouldEqual("mySecondServiceTitle");
    }
}
