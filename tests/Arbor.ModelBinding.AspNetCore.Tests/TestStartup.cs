using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
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
}