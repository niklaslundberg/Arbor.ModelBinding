using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_with_bad_data
    {
        static Exception exception;

        Establish context = () => { };

        Because of = () =>
        {
            exception = Catch.Exception(() =>
            {
                FormsExtensions.ParseFromPairs(
                    new List<KeyValuePair<string, StringValues>>
                    {
                        new KeyValuePair<string, StringValues>("bad", "data")
                    },
                    typeof(DateWrapper));
            });
        };

        It should_throw_exception = () =>
        {
            Console.WriteLine(exception);

            exception.ShouldNotBeNull();
        };

        It should_throw_exception_of_type_form_parse_exception =
            () => { exception.ShouldBeOfExactType<ArgumentException>(); };
    }
}
