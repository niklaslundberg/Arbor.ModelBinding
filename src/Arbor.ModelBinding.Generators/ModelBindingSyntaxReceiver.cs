using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal class ModelBindingSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassData> CommandsToGenerateFor { get; } = new ();

        private readonly HashSet<string> _supportedDataTypes =
            new(StringComparer.OrdinalIgnoreCase) {"int", "string"};

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds
                && cds.IsPartial()
                && cds.DataType() is {} dataType
                && _supportedDataTypes.Contains(dataType))
            {
                CommandsToGenerateFor.Add(new ClassData(cds, cds.Namespace(), dataType));
            }
        }
    }
}