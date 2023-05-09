using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RedArrisApi.UnitTests;

[TestClass]
public class ReturnsControllerTests
{
    private readonly ReturnsController _controller;
    private readonly Mock<IReturnsService> _returnsServiceMock = new();
    private readonly Mock<IPricesService> _pricesServiceMock = new();

    public ReturnsControllerTests()
    {
        _returnsServiceMock.Setup(x => x.CalculateReturns(It.IsAny<IexPriceResponse[]>()))
            .Returns(() => new Return[10]);
        _pricesServiceMock.Setup(x => x.GetPricesAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(() => new IexPriceResponse[10]);
        _controller = new ReturnsController(_returnsServiceMock.Object, _pricesServiceMock.Object);
    }
    
    [DataTestMethod]
    [DataRow("msft", null, null)]
    [DataRow("msft", "2022-01-01", "2022-06-01")]
    [DataRow("msft", "2023-02-01", null)]
    [DataRow("msft", null, "2023-03-01")]
    public async Task GetPricesSucceedsAsync(string ticker, string start, string end)
    {
        var response = await _controller.GetReturnsAsync(ticker, start, end);

        response.Should().NotBeNull();

        var result = response.As<OkObjectResult>();
        result.Should().NotBeNull();
        
        var returnDto = result.Value.As<ReturnDto>();
        returnDto.Should().NotBeNull();

        returnDto.Ticker.Should().BeUpperCased(ticker);
        returnDto.Count.Should().Be(10);
        returnDto.Returns.Count().Should().Be(10);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    public async Task Returns400ForNoTickerAsync(string ticker)
    {
        var response = await _controller.GetReturnsAsync(ticker);
        response.Should().NotBeNull();
        response.As<BadRequestObjectResult>()
            .Should().NotBeNull();
    }

    [DataTestMethod]
    [DataRow("msft", "2022-06-01", "2022-01-01")]
    [DataRow("msft", "2000-01-01", null)]
    [DataRow("msft", "2000-01-01", "2022-01-01")]
    public async Task Returns400ForInvalidDatesAsync(string ticker, string start, string end)
    {
        var response = await _controller.GetReturnsAsync(ticker, start, end);
        response.Should().NotBeNull();
        response.As<BadRequestObjectResult>()
            .Should().NotBeNull();
    }

    [TestMethod]
    public async Task Returns422ForFewerThanTwoPriceResultsAsync()
    {
        _pricesServiceMock.Setup(x => x.GetPricesAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(() => new IexPriceResponse[1]);

        var response = await _controller.GetReturnsAsync("msft");
        response.Should().NotBeNull();
        response.As<UnprocessableEntityObjectResult>()
            .Should().NotBeNull();
    }

    [TestMethod]
    public async Task GracefullyReturns500ForFailedPricesServiceAsync()
    {
        _pricesServiceMock.Setup(x => x.GetPricesAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Throws<Exception>();
        
        var response = await _controller.GetReturnsAsync("msft");

        response.Should().NotBeNull();

        response.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
    
    [TestMethod]
    public async Task GracefullyReturns500ForFailedReturnsServiceAsync()
    {
        _returnsServiceMock.Setup(x => x.CalculateReturns(It.IsAny<IexPriceResponse[]>()))
            .Throws<Exception>();
        
        var response = await _controller.GetReturnsAsync("msft");

        response.Should().NotBeNull();

        response.As<StatusCodeResult>().StatusCode.Should().Be(500);
    }
}