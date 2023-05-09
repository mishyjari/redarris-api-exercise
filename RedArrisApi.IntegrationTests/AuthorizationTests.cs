using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedArrisApi.IntegrationTests;

[TestClass]
public class AuthorizationTests
{
    private static WebApplicationFactory<Program> _factory = new();
    private static string _apiKey = string.Empty;

    [ClassInitialize]
    public static void Initialize(TestContext _)
    {
        _factory = new WebApplicationFactory<Program>();
        var apiKey = TestManager
            .GetRequiredService<IConfiguration>()
            .GetValue<string>("apiKey");
        
        if (apiKey is null)
        {
            throw new NullReferenceException("Internal api key not provided.");
        }
        _apiKey = apiKey!;
    }

    [TestMethod]
    public async Task SuccessfulResponseWithApiKeyInHeaderAsync()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);

        var response = await client.GetAsync("returns/msft");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [TestMethod]
    public async Task SuccessfulResponseWithApiKeyInQueryAsync()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"returns/msft?apiKey={_apiKey}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [TestMethod]
    public async Task UnauthorizedResponseWithBadApiKeyInHeaderAsync()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "definitely_wrong");

        var response = await client.GetAsync("returns/msft");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [TestMethod]
    public async Task UnauthorizedResponseWithBadApiKeyInQueryAsync()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"returns/msft?apiKey=definitely-wrong");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [TestMethod]
    public async Task UnauthorizedResponseWithNoApiKeyAsync()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("returns/msft");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
