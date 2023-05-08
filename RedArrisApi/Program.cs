namespace RedArrisApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder
            .Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile("secrets.json")
            .AddEnvironmentVariables();

        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder
            .Build()
            .ConfigureWebApplication();

        app.Run();
    }
}