using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    public class ClassData
    {
        public ClassDeclarationSyntax Syntax { get; }
        public string Namespace { get; }

        public ClassData(ClassDeclarationSyntax syntax, string @namespace)
        {
            Syntax = syntax;
            Namespace = @namespace;
        }
    }
}