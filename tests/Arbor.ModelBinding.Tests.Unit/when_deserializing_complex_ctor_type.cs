using System;
using System.Collections.Generic;
using System.Linq;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
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
    public class when_deserializing_complex_ctor_type
    {
        static object result;
        static Type targetType = typeof(ItemWithServicesCtor);
        static ItemWithServicesCtor target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
            {
                new("description", "myDescription"),
                new("numberOfItems", "33"),
                new("services[0].title", "myFirstServiceTitle"),
                new("services[0].otherProperty", "42"),
                new("services[1].title", "mySecondServiceTitle"),
                new("services[1].otherProperty", "123")
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as ItemWithServicesCtor;
            };

        It should_have_other_simple_property_set = () => target.NumberOfItems.ShouldEqual(33);

        It should_have_simple_property_set = () => target.Description.ShouldEqual("myDescription");

        It should_have_collection_not_null = () => target.Services.ShouldNotBeNull();

        It should_not_be_null = () => target.ShouldNotBeNull();

        It should_return_an_object_of_type_the_requested_type =
            () => result.ShouldBeOfExactType<ItemWithServicesCtor>();

        It should_return_first_nested_object_with_property_set =
            () => target.Services.ToList()[0].Title.ShouldEqual("myFirstServiceTitle");

        It should_return_nested_objects = () => target.Services.ToList().Count.ShouldEqual(2);

        It should_return_second_nested_object_with_property_set =
            () => target.Services.ToList()[1].Title.ShouldEqual("mySecondServiceTitle");
    }
}
