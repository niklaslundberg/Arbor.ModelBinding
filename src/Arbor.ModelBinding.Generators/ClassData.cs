using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal class ClassData
    {
        public ClassData(ClassDeclarationSyntax syntax, string? @namespace, string? dataType)
        {
            Syntax = syntax;
            Namespace = @namespace;
            DataType = dataType;
        }

        public string NetType => DataType switch
        {
            "string" => "string",
            "int" => "int",
            "Custom" => "int",
            _ => "string"
        };

        public ClassDeclarationSyntax Syntax { get; }

        public string? Namespace { get; }

        public string? DataType { get; }
    }
}