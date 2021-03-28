using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal class MySyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassData> CommandsToGenerateFor { get; } = new ();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax cds
                && cds.IsPartial() && cds.DataType() is {} dataType)

            {
                CommandsToGenerateFor.Add(new ClassData(cds, cds.Namespace(), dataType));
            }
        }
    }
}