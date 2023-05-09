namespace RedArrisApi;

public class Program
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
            .AddJsonFile("secrets.json", false, true)
            .AddEnvironmentVariables();

        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder
            .Build()
            .ConfigureWebApplication();

        app.Run();
    }
}