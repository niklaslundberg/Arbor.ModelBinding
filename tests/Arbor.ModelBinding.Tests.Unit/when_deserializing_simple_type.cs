﻿using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.SampleTypes;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;
using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_simple_type
    {
        static object result;
        static Type targetType = typeof(BookingCancellationRequest);
        static BookingCancellationRequest target;

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues>("bookingId", "123"),
                new KeyValuePair<string, StringValues>("reason", "sunshine"),
            };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, typeof(BookingCancellationRequest));
                target = result as BookingCancellationRequest;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_string_property_set = () => target.Reason.Equals("sunshine");

        It should_have_int_property_set = () => target.BookingId.Equals(123);
    }
}
