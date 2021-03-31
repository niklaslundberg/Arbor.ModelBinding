using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal class ClassData
    {
        public ClassData(ClassDeclarationSyntax syntax, string? @namespace, string? dataType,
            StringComparison? stringComparison)
        {
            Syntax = syntax;
            Namespace = @namespace;
            DataType = dataType;
            StringComparison = stringComparison;
        }

        public string NetType => DataType switch
        {
            "string" => "string",
            "int" => "int",
            "String" => "string",
            "Int" => "int",
            _ => "string"
        };

        public ClassDeclarationSyntax Syntax { get; }

        public string? Namespace { get; }

        public string? DataType { get; }
        public StringComparison? StringComparison { get; }
    }
}