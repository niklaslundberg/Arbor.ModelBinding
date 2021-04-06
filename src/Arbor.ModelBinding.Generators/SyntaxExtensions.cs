using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arbor.ModelBinding.Generators
{
    internal static class SyntaxExtensions
    {
        public static string? Namespace(this ClassDeclarationSyntax classDeclarationSyntax)
        {
            SyntaxNode current = classDeclarationSyntax;
            while (current.Parent is not null)
            {
                if (current.Parent is NamespaceDeclarationSyntax
                    namespaceDeclarationSyntax)
                {
                    return namespaceDeclarationSyntax.Name.ToString();
                }

                current = current.Parent;
            }

            return null;
        }

        public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax) =>
            classDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

        public static string? DataType(
            this ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.AttributeLists.Count == 0)
            {
                return null;
            }

            var stringAttribute = classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .SingleOrDefault(attribute => attribute.Name.ToString().EndsWith("StringValueType"));

            if (stringAttribute is {})
            {
                return "string";
            }

            var intAttribute = classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .SingleOrDefault(attribute => attribute.Name.ToString().EndsWith("IntValueType"));

            if (intAttribute is {})
            {
                return "int";
            }

            var longAttribute = classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .SingleOrDefault(attribute => attribute.Name.ToString().EndsWith("LongValueType"));

            if (longAttribute is {})
            {
                return "long";
            }

            return null;
        }
        public static StringComparison? StringComparison(
            this ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (classDeclarationSyntax.AttributeLists.Count == 0)
            {
                return null;
            }

            var stringAttribute = classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .SingleOrDefault(attribute => attribute.Name.ToString().EndsWith("StringValueType"));

            if (stringAttribute is null)
            {
                return null;
            }

            string? value =
                stringAttribute.ArgumentList?.Arguments.FirstOrDefault()
                ?.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.Identifier.ValueText;

            if (Enum.TryParse(value, true, out StringComparison comparison))
            {
                return comparison;
            }

            return value switch
            {
                "StringComparison.OrdinalIgnoreCase" => System.StringComparison.OrdinalIgnoreCase,
                _ => System.StringComparison.Ordinal,
            };
        }
    }
}