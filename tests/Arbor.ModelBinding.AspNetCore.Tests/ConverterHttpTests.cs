using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class ConverterHttpTests

    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly TestServer _testServer;

        public ConverterHttpTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            var builder = new WebHostBuilder()
                .ConfigureServices(services => services.AddControllers(
                    c =>
                    {
                        c.ModelBinderProviders.Insert(0, new CustomModelBindingProvider());
                    }).AddJsonOptions(options =>
                {
                    foreach (var jsonConverter in CustomManualJsonConverters.Converters)
                    {
                        options.JsonSerializerOptions.Converters.Add(jsonConverter);
                    }
                }))
                .UseStartup<TestStartup>();
            _testServer = new TestServer(builder);
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

        [Fact]
        public async Task Get3()
        {
            // Arrange
            var client = _testServer.CreateClient();

            var httpResponseMessage = await client.GetAsync("/typeconvertergenerated/abc");

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(content);

            httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task Post2()
        {
            // Arrange
            var client = _testServer.CreateClient();

            var jsonSerializerOptions = new JsonSerializerOptions()
            {

            };
            jsonSerializerOptions.Converters.Add(new Partial1ParserJsonConverter());

            var httpResponseMessage = await client.PostAsJsonAsync("/typeconvertergenerated/",new PostObject{Value =  new Partial1Parser("abc")},
                jsonSerializerOptions);

            string content = await httpResponseMessage.Content.ReadAsStringAsync();

            _testOutputHelper.WriteLine(content);

            httpResponseMessage.IsSuccessStatusCode.Should().BeTrue();
        }
    }

    //Generate


    //Generate
}