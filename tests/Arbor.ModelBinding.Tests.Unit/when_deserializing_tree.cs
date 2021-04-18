using System;
using System.Collections.Generic;
using Arbor.ModelBinding.Tests.Unit.ComplexTypes;

using Machine.Specifications;

using Arbor.ModelBinding.NewtonsoftJson;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace Arbor.ModelBinding.Tests.Unit
{
    [Subject(typeof(FormsExtensions))]
    public class when_deserializing_tree
    {
        static object result;

        static TreeNode target;

        static Type targetType = typeof(TreeNode);

        static List<KeyValuePair<string, StringValues>> values;

        Cleanup after = () => { };

        Establish context = () =>
        {
            values = new List<KeyValuePair<string, StringValues>>
                     {
                         new KeyValuePair<string, StringValues>("name", "root"),
                         new KeyValuePair<string, StringValues>("nodes[0].name", "a"),
                         new KeyValuePair<string, StringValues>("nodes[0].nodes[0].name", "a-1"),
                         new KeyValuePair<string, StringValues>("nodes[1].name", "b"),
                         new KeyValuePair<string, StringValues>("nodes[1].nodes[0].name", "b-1"),
                         new KeyValuePair<string, StringValues>("nodes[1].nodes[1].name", "b-2")
                     };
        };

        Because of =
            () =>
            {
                result = FormsExtensions.ParseFromPairs(values, targetType);
                target = result as TreeNode;

                Console.WriteLine(
                    $"Instance: {JsonConvert.SerializeObject(target, Formatting.Indented)}");
            };

        It should_have_name_set = () => target.Name.ShouldEqual("root");

        It should_have_notes_not_null = () => target.Nodes.Count.ShouldEqual(2);

        It should_have_sub_node_0_name_not_null = () => target.Nodes[0].Name.ShouldEqual("a");

        It should_have_sub_node_1_name_not_null = () => target.Nodes[1].Name.ShouldEqual("b");

        It should_have_sub_node_1_nodes = () => target.Nodes[1].Nodes.Count.ShouldEqual(2);

        It should_not_be_null = () => { result.ShouldNotBeNull(); };

        It should_have_sub_node_0_nodes = () => target.Nodes[0].Nodes.Count.ShouldEqual(1);
    }
}