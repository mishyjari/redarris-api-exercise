using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static RedArrisApi.IntegrationTests.TestManager;

namespace RedArrisApi.IntegrationTests;

[TestClass]
public class ReturnsControllerTests
{
    private readonly ReturnsController _controller;

    public ReturnsControllerTests()
    {
        _controller = GetRequiredService<ReturnsController>();
    }

    [TestMethod]
    public async Task GetsReturnsDefaultDatesAsync()
    {
        var result = await _controller.GetReturnsAsync("msft");

        result.Should().NotBeNull();

        var returnDto = result.As<OkObjectResult>().Value.As<ReturnDto>();
        returnDto.Should().NotBeNull();
        returnDto.Returns.Should().NotBeNullOrEmpty();
        returnDto.Symbol.Should().Be("MSFT");
        returnDto.FromDate.Should().Be(new DateTime(DateTime.Now.Year, 1, 1));
        returnDto.ToDate.Should().Be(DateTime.Today);

        returnDto.Returns.First().AsOfDate.Should().BeBefore(returnDto.Returns.Last().AsOfDate);

        var firstReturnClose = returnDto.Returns.ElementAt(0).ClosePrice;
        var secondReturnClose = returnDto.Returns.ElementAt(1).ClosePrice;
        var expectedSecondReturnValue = Math.Round((secondReturnClose / firstReturnClose - 1) * 100, 4);

        returnDto.Returns.ElementAt(1).Value.Should().Be(expectedSecondReturnValue);
    }

    [TestMethod]
    public async Task GetReturnsSpecifiedDatesAsync()
    {
        var result = await _controller.GetReturnsAsync("msft", "2022-01-01", "2022-06-01");

        result.Should().NotBeNull();

        var returnDto = result.As<OkObjectResult>().Value.As<ReturnDto>();
        returnDto.Should().NotBeNull();
        returnDto.Returns.Should().NotBeNullOrEmpty();
        returnDto.Symbol.Should().Be("MSFT");
        returnDto.FromDate.Should().Be(DateTime.Parse("2022-01-01"));
        returnDto.ToDate.Should().Be(DateTime.Parse("2022-06-01"));

        returnDto.Returns.First().AsOfDate.Should().BeBefore(returnDto.Returns.Last().AsOfDate);

        var firstReturnClose = returnDto.Returns.ElementAt(0).ClosePrice;
        var secondReturnClose = returnDto.Returns.ElementAt(1).ClosePrice;
        var expectedSecondReturnValue = Math.Round((secondReturnClose / firstReturnClose - 1) * 100, 4);

        returnDto.Returns.ElementAt(1).Value.Should().Be(expectedSecondReturnValue);
    }

    [DataTestMethod]
    [DataRow(null, null, null)]
    [DataRow("", "2022-01-01", "2022-06-01")]
    [DataRow("msft", "2000-01-01", "2022-01-01")]
    [DataRow("msft", "2022-06-01", "2022-01-01")]
    public async Task ReturnsBadRequestAsync(string symbol, string from, string to)
    {
        var result = await _controller.GetReturnsAsync(symbol, from, to);
        result.As<BadRequestObjectResult>().Should().NotBeNull();
    }

    [TestMethod]
    public async Task ReturnsUnprocessableEntityAsync()
    {
        var result = await _controller.GetReturnsAsync("foobar");
        result.As<UnprocessableEntityObjectResult>().Should().NotBeNull();
    }
}