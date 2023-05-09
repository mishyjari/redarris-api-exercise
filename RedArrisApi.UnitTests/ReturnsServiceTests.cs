using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RedArrisApi.UnitTests;

[TestClass]
public class ReturnsServiceTests
{
    private readonly IReturnsService _returnsService;
    
    public ReturnsServiceTests()
    {
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<Return>(It.IsAny<IexPriceResponse>()))
            .Returns((IexPriceResponse p) => new Return()
            {
                AsOfDate = DateTime.Parse(p.AsOfDate),
                ClosePrice = p.Close
            });

        _returnsService = new ReturnsService(mapperMock.Object);
    }

    [TestMethod]
    public void CalculatesReturns()
    {
        var prices = new IexPriceResponse[]
        {
            new()
            {
                Close = 100,
                AsOfDate = DateTime.Now.ToString("yyyy-MM-dd")
            },
            new()
            {
                Close = 110,
                AsOfDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
            },
            new()
            {
                Close = 90,
                AsOfDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")
            }
        };

        var returns = _returnsService.CalculateReturns(prices);
        returns.Count().Should().Be(2);
        returns.First().Value.Should().Be(10);
        returns.Last().Value.Should().Be(-18.1818);
    }
}