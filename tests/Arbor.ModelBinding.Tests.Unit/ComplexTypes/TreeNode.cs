using System.Collections.Generic;
using System.Linq;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class TreeNode
    {
        public TreeNode(string name, IReadOnlyList<TreeNode> nodes)
        {
            Name = name;
            Nodes = nodes?.ToList();
        }

        public string Name { get; }

        public IReadOnlyList<TreeNode> Nodes { get; }
    }
}