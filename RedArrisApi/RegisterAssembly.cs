using System.Net.Http.Headers;

namespace RedArrisApi;

public static class RegisterAssembly
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(RegisterAssembly));
        services.AddSwaggerGen();
        services.AddControllers();

        services.ConfigureIexSettings(configuration);

        services.AddScoped<ReturnsController>();
        services.AddTransient<IPricesService, PricesService>();
        services.AddTransient<IReturnsService, ReturnsService>();
    }

    // Configure settings used for IEX, taking the API Key from secrets.json and register with DI
    // Create HttpClient for IEX and register with DI
    private static void ConfigureIexSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration.GetValue<string>("IexApiKey");

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentNullException(nameof(apiKey), "IEX API Key is missing. Please create a secrets.json" +
                                                            "file in the project root and create a property 'IexApiKey'");

        var settings = new IexSettings
        {
            IexApiKey = apiKey
        };
        services.AddSingleton(typeof(IexSettings), settings);
        services.AddHttpClient(new IexSettings().IexHttpClientName,
            client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
    }
    
    // Configure Swagger and Endpoints (referenced in Program.cs)
    public static WebApplication ConfigureWebApplication(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedArris Stock API v1"); });

        app.UseRouting();

        app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}