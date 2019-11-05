using System;
using System.Collections.Generic;
using System.Linq;
using Arbor.ModelBinding.Core;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_nested_complex_type
    {
        static object result;
        static Type targetType = typeof(ComplexRootObject);
        static ComplexRootObject target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            KeyValuePair<string, StringValues> Pair(string key, string value)
            {
                return new KeyValuePair<string, StringValues>(key, value);
            }

            values = new List<KeyValuePair<string, StringValues>>
            {
                Pair("rootTitle", "myRootTitle"),
                Pair("rootOtherProperty", "911"),
                Pair("subTypes[0].subTitle", "myFirstSubTitle"),
                Pair("subTypes[0].subOtherProperty", "42"),
                Pair("subTypes[0].subListItems[0].note", "The quick brown"),
                Pair("subTypes[0].subListItems[1].note", "fox"),
                Pair("subTypes[0].subListItems[2].note", "jumps"),
                Pair("subTypes[1].subTitle", "mySecondSubTypeTitle"),
                Pair("subTypes[1].subOtherProperty", "123")
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as ComplexRootObject;

                Console.WriteLine("Instance: " +
                                  JsonConvert.SerializeObject(target, Formatting.Indented));
            };

        It should_have_other_simple_property_set = () => target.RootOtherProperty.ShouldEqual(911);

        It should_have_simple_property_set = () => target.RootTitle.ShouldEqual("myRootTitle");

        It should_not_be_null = () => target.SubTypes.First().SubTitle.ShouldEqual("myFirstSubTitle");

        It should_return_an_object_of_type_the_requested_type =
            () =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                result.ShouldBeOfExactType<ComplexRootObject>();
            };

        It should_return_first_nested_object_with_property_set =
            () => target.SubTypes.First().SubListItems.Count.ShouldEqual(3);

        It should_return_nested_objects = () => target.SubTypes.Count.ShouldEqual(2);

        It should_return_second_nested_object_with_property_set =
            () => target.SubTypes.First().SubTitle.ShouldEqual("myFirstSubTitle");
    }
}
