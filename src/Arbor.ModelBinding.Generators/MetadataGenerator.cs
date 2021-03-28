using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Arbor.ModelBinding.Generators
{
    [Generator]
    public class MetadataGenerator : ISourceGenerator
    {
        private static readonly DiagnosticDescriptor WarningMessage = new("ARB1000",
            "General message",
            "'{0}'.",
            "Arbor",
            DiagnosticSeverity.Warning,
            true);

        private static readonly DiagnosticDescriptor CustomInformation = new("ARB2000",
            "Codegen info",
            "Info from parser '{0}'.",
            "Arbor",
            DiagnosticSeverity.Info,
            true);

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());


        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not MySyntaxReceiver mySyntaxReceiver)
            {
                return;
            }

            string all = string.Join(", ",
                mySyntaxReceiver.CommandsToGenerateFor.Select(s => s.Syntax.Identifier.ValueText));

            context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None,
                $"Received nodes {all}"));

            try
            {
                var model = new
                {
                    Mappings = mySyntaxReceiver.CommandsToGenerateFor.Select(n =>
                        new
                        {
                            Key = n.Syntax.Identifier.ValueText,
                            n.Namespace,
                            Value = "TestValue",
                            n.DataType,
                            n.NetType
                        }).ToArray(),
                    MainNamespace = mySyntaxReceiver.CommandsToGenerateFor.FirstOrDefault()?.Namespace
                };

                using var manifestResourceStream = typeof(MetadataGenerator).Assembly.GetManifestResourceStream("Arbor.ModelBinding.Generators.Handlers.sbntxt");

                if (manifestResourceStream is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None,
                        "Embedded template could not be found"));

                    return;
                }

                using var streamReader = new StreamReader(manifestResourceStream, Encoding.UTF8);

                string templateData = streamReader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(templateData))
                {
                    context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None,
                        "Template could not be found"));

                    return;
                }

                var template1 = Template.Parse(templateData);
                context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None,
                    $"Rendering model with {model.Mappings.Length} items"));

                string? output = template1.Render(model);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    context.AddSource("Parsers.g.cs", SourceText.From(output, Encoding.UTF8));
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None,
                        $"No source content was rendered"));
                }
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(WarningMessage, Location.None, ex.ToString()));
            }
        }
    }
}