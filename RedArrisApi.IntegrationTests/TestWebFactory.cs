using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RedArrisApi.IntegrationTests;

public class TestWebFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(".");

        builder.ConfigureAppConfiguration((context, builder) =>
        {
            context.HostingEnvironment.ApplicationName = "Cph.WebApi.Tests.Integration";
        });

        base.ConfigureWebHost(builder);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices((builder, services) =>
        {
            services.AddControllers();
            services.ConfigureServices(builder.Configuration);
        });
        return base.CreateHost(builder);
    }
}
