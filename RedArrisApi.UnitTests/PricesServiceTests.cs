using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RedArrisApi.UnitTests;

[TestClass]
public class PricesServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    private Mock<IexSettings> _iexSettingsMock = new();
    private readonly IPricesService _pricesService;

    public PricesServiceTests()
    {
        var httpClient = new HttpClient(new HttpClientHandler());
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _pricesService = new PricesService(_httpClientFactoryMock.Object, new IexSettings());
    }

    [TestMethod]
    public async Task GetsPricesAsync()
    {
        var httpClient = new Mock<HttpClient>();
        httpClient.Setup(x => x.BaseAddress).Returns(new Uri("http://localhost"));
        httpClient.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), CancellationToken.None))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[{\"date\":\"2022-05-01\",\"open\":100.0,\"close\":101.0}]"),
            });
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient.Object);
        
        _iexSettingsMock.SetupGet(x => x.IexPricesUrl)
            .Returns("https://sandbox.iexapis.com/stable/stock");
        _iexSettingsMock.SetupGet(x => x.IexApiKey)
            .Returns("fakeApiKey");
        _iexSettingsMock.SetupGet(x => x.IexHttpClientName)
            .Returns("iexHttpClient");

        var pricesService = new PricesService(
            _httpClientFactoryMock.Object, 
            _iexSettingsMock.Object);

        // Act
        var result = await pricesService.GetPricesAsync("AAPL", "2022-05-01", "2022-05-01");

        // Assert
        Assert.IsNotNull(result);
    } 
}