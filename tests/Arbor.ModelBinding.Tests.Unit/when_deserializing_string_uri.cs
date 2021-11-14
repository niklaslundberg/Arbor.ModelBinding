using System;
using System.Collections.Generic;
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
    public class when_deserializing_string_uri
    {
        static object result;
        static Type targetType = typeof(TypeWithStringUri);
        static TypeWithStringUri target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
                     {
                         new("url", "http://example.local"),
                     };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as TypeWithStringUri;
            };

        It should_have_url_not_null = () => target.Url.ShouldNotBeNull();

    }
}