﻿using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Core;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_getting_a_struct_from_empty_collection
    {
        Establish context = () => values = new List<KeyValuePair<string, StringValues>>();

        Because of = () => result = FormsExtensions.ParseFromPairs(values, typeof(DateTime));

        It should_return_null = () => result.ShouldBeNull();

        static List<KeyValuePair<string, StringValues>> values;

        static object result;
    }
}
