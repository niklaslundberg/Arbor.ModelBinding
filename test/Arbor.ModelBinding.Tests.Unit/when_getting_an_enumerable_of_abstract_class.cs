using System.Collections.Generic;
using Machine.Specifications;
using Microsoft.Extensions.Primitives;
using Arbor.ModelBinding.NewtonsoftJson;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_getting_an_enumerable_of_abstract_class
    {
        Establish context = () => values = new List<KeyValuePair<string, StringValues>>();

        Because of = () => result = FormsExtensions.ParseFromPairs(values, typeof(MyAbstract[])) as MyAbstract[];

        It should_return_null = () => result.ShouldBeNull();

        static List<KeyValuePair<string, StringValues>> values;
        static MyAbstract[] result;
    }
}