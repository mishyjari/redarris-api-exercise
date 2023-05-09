using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using static RedArrisApi.UnitTests.Helpers;

namespace RedArrisApi.UnitTests;

[TestClass]
public class PricesServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    private IexSettings _iexSettings = new();
    private readonly IPricesService _pricesService;

    public PricesServiceTests()
    {
        var httpClient = new HttpClient(new HttpClientHandler());
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _iexSettings.IexApiKey = "fake_key";

        _pricesService = new PricesService(_httpClientFactoryMock.Object, _iexSettings);
    }

    [TestMethod]
    public async Task GetsPricesAsync()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(IexSampleResponseJsonString, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var result = await _pricesService.GetPricesAsync("MSFT", "2022-05-01", "2022-05-01");

        result.Should().NotBeNull();

        var deserializedSampleData =
            JsonConvert.DeserializeObject<IEnumerable<IexPriceDto>>(IexSampleResponseJsonString);

        result.Count().Should().Be(deserializedSampleData.Count());
    } 
}