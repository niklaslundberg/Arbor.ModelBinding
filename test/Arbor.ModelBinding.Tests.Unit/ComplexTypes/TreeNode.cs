using System.Collections.Generic;
using System.Linq;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class TreeNode
    {
        public TreeNode(string name, IEnumerable<TreeNode> nodes)
        {
            Name = name;
            Nodes = nodes?.ToList();
        }

        public string Name { get; }

        public List<TreeNode> Nodes { get; }
    }
}