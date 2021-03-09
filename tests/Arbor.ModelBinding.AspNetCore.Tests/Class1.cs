using System.Threading.Tasks;
using Arbor.ModelBinding.AspNetCore.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using WebHostBuilderExtensions = Microsoft.AspNetCore.TestHost.WebHostBuilderExtensions;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
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

        [Fact]
        public async Task Get2()
        {
            // Arrange
            var client = _testServer.CreateClient();

            var httpResponseMessage = await client.GetAsync("/typeconverter/abc");

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(content);

            httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
    }

    //Generate


    //Generate
}
