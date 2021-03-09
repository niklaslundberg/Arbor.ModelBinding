using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Arbor.ModelBinding.AspNetCore.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Type = System.Type;
using WebHostBuilderExtensions = Microsoft.AspNetCore.TestHost.WebHostBuilderExtensions;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("/{id}")]
        public IActionResult Get([FromRoute] MyValueObject? id)
        {
            if (id is null)
            {
                return StatusCode(500);
            }

            return Ok(id.Value);
        }
    }

    public abstract class ValueObject<T> : IEquatable<ValueObject<T>>
    {
        private readonly IComparer<T>? _comparer;

        protected ValueObject(T value, IComparer<T>? comparer = null)
        {
            Value = value;
            _comparer = comparer;
        }

        public bool Equals(ValueObject<T>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (_comparer is { })
            {
                return _comparer.Compare(Value, other.Value) == 0;
            }

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj) => Equals(obj as ValueObject<T>);


        public override int GetHashCode()
        {
            if (_comparer is StringComparer comparer && comparer == StringComparer.OrdinalIgnoreCase && Value is string stringValue)
            {
                return stringValue.ToLowerInvariant().GetHashCode();
            }

            return EqualityComparer<T>.Default.GetHashCode(Value!);
        }

        public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right) => Equals(left, right);

        public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right) => !Equals(left, right);

        public T Value { get; }
    }

    public sealed class MyValueObject : ValueObject<string>
    {
        public MyValueObject(string value): base(value, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
    public sealed class MyOrdinalValueObject : ValueObject<string>
    {
        public MyOrdinalValueObject(string value): base(value)
        {
        }
    }

    public sealed    class MyIntValueObject : ValueObject<int>
    {
        public MyIntValueObject(int value): base(value)
        {
        }

        public MyIntValueObject(string value) : this(int.Parse(value))
        {

        }
    }

    public class TestParser
    {
        public static void Consume()
        {
            Test.GeneratedTest.DoIt();
        }
    }


    public class Class1

    {
        private readonly ITestOutputHelper _testOutputHelper;
        private TestServer _testServer;

        public Class1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var builder = new WebHostBuilder()
                .ConfigureServices(services => services.AddControllers(
                    c =>
                    {
                        c.ModelBinderProviders.Insert(0, new CustomModelBindingProvider());

                    }).AddJsonOptions(options =>
                {
                    foreach (var jsonConverter in CustomJsonConverters.Converters)
                    {
                        options.JsonSerializerOptions.Converters.Add(jsonConverter);
                    }
                }))
                .UseStartup<TestStartup>();
            _testServer = new TestServer(builder);
    }

    [Fact]
    public void Do()
    {
        var myValueObject = new MyValueObject("Abc");
        var valueObject = new MyValueObject("abc");
        myValueObject.Equals(valueObject).Should().BeTrue();
        myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
    }
    [Fact]
    public void Do3()
    {
        var myValueObject = new MyIntValueObject(1);
        var valueObject = new MyIntValueObject(1);
        myValueObject.Equals(valueObject).Should().BeTrue();
        myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
    }
    [Fact]
    public void D4()
    {
        var myValueObject = new MyIntValueObject("1");
        var valueObject = new MyIntValueObject(1);
        myValueObject.Equals(valueObject).Should().BeTrue();
        myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
    }
    [Fact]
    public void Do1()
    {
        var myValueObject = new MyOrdinalValueObject("Abc");
        var valueObject = new MyOrdinalValueObject("abc");
        myValueObject.Equals(valueObject).Should().BeFalse();
        myValueObject.GetHashCode().Should().NotBe(valueObject.GetHashCode());
    }

    [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _testServer.CreateClient();

            var httpResponseMessage = await client.GetAsync("/123");

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(content);

            httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();

            _ = await client.GetAsync("/123");
        }
    }

    public class TestStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class CustomModelBindingProvider : IModelBinderProvider
    {

        private static int counter = 1;
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(MyValueObject))
            {
                var name = counter++;
                return new CustomModelBinder(name);
                Console.WriteLine("Creating new model binder " + name);
            }

            return default;
        }
    }

    //Generate
    public class CustomModelBinder : IModelBinder
    {
        private readonly int _name;

        public CustomModelBinder(int name)
        {
            _name = name;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Console.WriteLine("Running bind " + _name);
            if (bindingContext.ModelType == typeof(MyValueObject) && bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue is { } value)
            {
                bindingContext.Result = ModelBindingResult.Success(new MyValueObject(value));
            }

            return Task.CompletedTask;
        }
    }


    public static class CustomJsonConverters
    {
        public static readonly ImmutableArray<JsonConverter> Converters =
            new List<JsonConverter>()
            {

                new MyConverter() //Generate
            }.ToImmutableArray();
    }

    //Generate
    public class MyConverter : System.Text.Json.Serialization.JsonConverter<MyValueObject>
    {
        public override MyValueObject? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.GetString() is { } value ? new MyValueObject(value) : null;

        public override void Write(Utf8JsonWriter writer, MyValueObject value, JsonSerializerOptions options) => writer.WriteStringValue(value.Value);
    }
}
