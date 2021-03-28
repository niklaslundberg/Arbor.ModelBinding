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

        public static bool IsPartial(this ClassDeclarationSyntax classDeclarationSyntax) =>
            classDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

        public static string? DataType(
            this ClassDeclarationSyntax classDeclarationSyntax) =>
            classDeclarationSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .SingleOrDefault()?.ArgumentList?.Arguments.FirstOrDefault()
                ?.DescendantNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.Identifier.ValueText;
    }
}