﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal class MySyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassData> CommandsToGenerateFor { get; } = new List<ClassData>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Business logic to decide what we're interested in goes here
            if (syntaxNode is ClassDeclarationSyntax cds &&
                cds.Identifier.ValueText.EndsWith("Parser", StringComparison.Ordinal))
            {
                SyntaxNode current = cds;
                CommandsToGenerateFor.Add(new ClassData(cds, cds.Namespace()));

            }
        }
    }

    public static class SyntaxExtensions
    {
        public static string? Namespace(this ClassDeclarationSyntax classDeclarationSyntax)
        {

            SyntaxNode current = classDeclarationSyntax;
            string? @namespace = null;
            while (current.Parent is not null)
            {
                if (current.Parent is NamespaceDeclarationSyntax
                    namespaceDeclarationSyntax)
                {
                    return namespaceDeclarationSyntax.Name.ToString();
                }

                current = current.Parent;
            }

            return @namespace;
        }
    }
}