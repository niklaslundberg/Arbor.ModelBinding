using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.ModelBinding.Generators.Tests
{
    public class GeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GeneratorTests(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;

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

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
                out var diagnostics);

            GeneratorDriverRunResult runResult = driver.GetRunResult();

            var generatorResult = runResult.Results[0];

            generatorResult.GeneratedSources.Length.Should().Be(1);
        }

        [Fact]
        public void Test2()
        {
            Compilation inputCompilation = CreateCompilation(@"
namespace MyCode
{
    [Arbor.ModelBinding.Primitives.IntValueType(1)]
    public partial class Program
    {
        
    }
}
");
            MetadataGenerator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
                out var diagnostics);

            GeneratorDriverRunResult runResult = driver.GetRunResult();

            var generatorResult = runResult.Results[0];

            generatorResult.GeneratedSources.Length.Should().Be(1);
        }

        [Fact]
        public void Test3()
        {
            Compilation inputCompilation = CreateCompilation(@"
namespace MyCode
{
    [Arbor.ModelBinding.Primitives.LongValueType(long.MinValue)]
    public partial class Program
    {
        
    }
}
");
            MetadataGenerator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
                out var diagnostics);

            GeneratorDriverRunResult runResult = driver.GetRunResult();

            var generatorResult = runResult.Results[0];

            foreach (var outputCompilationSyntaxTree in outputCompilation.SyntaxTrees)
            {
                _testOutputHelper.WriteLine(outputCompilationSyntaxTree.ToString());
            }

            generatorResult.GeneratedSources.Length.Should().Be(1);
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}