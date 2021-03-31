using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Arbor.ModelBinding.Generators.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void Test()
        {
            Compilation inputCompilation = CreateCompilation(@"
namespace MyCode
{
    [Arbor.ModelBinding.Primitives.StringValueType(System.StringComparison.OrdinalIgnoreCase)]
    public partial class Program
    {
        
    }
}
");
            MetadataGenerator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            GeneratorDriverRunResult runResult = driver.GetRunResult();

            GeneratorRunResult generatorResult = runResult.Results[0];

            generatorResult.GeneratedSources.Length.Should().Be(1);
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
