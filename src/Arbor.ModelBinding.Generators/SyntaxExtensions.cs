﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
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