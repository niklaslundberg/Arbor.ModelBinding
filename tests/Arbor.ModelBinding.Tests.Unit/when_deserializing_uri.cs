using System;
using System.Collections.Generic;
using Arbor.ModelBinding.NewtonsoftJson;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_uri
    {
        static object result;
        static Type targetType = typeof(TypeWithUri);
        static TypeWithUri target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("url", "http://example.local"),
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as TypeWithUri;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_url_not_null = () => target.Url.ShouldNotBeNull();

    }
}