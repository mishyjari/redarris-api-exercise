using Microsoft.Extensions.Primitives;

namespace RedArrisApi;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyNameHeaders = "X-API-KEY";
    private const string ApiKeyNameQuery = "apiKey";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var providedApiKey = GetApiKeyFromContext(context);
        
        
        if (providedApiKey is null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api Key Not Provided.");
            return;
        }

        var apiKey = context
            .RequestServices
            .GetRequiredService<IConfiguration>()
            .GetValue<string>("apiKey");

        if (apiKey is null)
        {
            throw new NullReferenceException("Internal api key not provided.");
        }

        if (!apiKey!.Equals(providedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Api Key");
            return;
        }

        await _next(context);
    }

    private static StringValues? GetApiKeyFromContext(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(ApiKeyNameHeaders, out var extractedApiKeyHeaders))
        {
            return extractedApiKeyHeaders;
        }

        if (context.Request.Query.TryGetValue(ApiKeyNameQuery, out var extractedApiKeyQuery))
        {
            return extractedApiKeyQuery;
        }

        return null;
    }
}