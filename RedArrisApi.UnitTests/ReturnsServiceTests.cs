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
        mapperMock.Setup(x => x.Map<Return>(It.IsAny<Tuple<IexPriceDto, IexPriceDto>>()))
            .Returns((Tuple<IexPriceDto, IexPriceDto> t) => new Return()
            {
                AsOfDate = DateTime.Parse(t.Item2.AsOfDate),
                ClosePrice = t.Item2.ClosePrice,
                Value = 10
            });

        _returnsService = new ReturnsService(mapperMock.Object);
    }

    [TestMethod]
    public void CalculatesReturns()
    {
        var prices = new IexPriceDto[]
        {
            new()
            {
                ClosePrice = 100,
                AsOfDate = DateTime.Now.ToString("yyyy-MM-dd")
            },
            new()
            {
                ClosePrice = 110,
                AsOfDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
            },
            new()
            {
                ClosePrice = 90,
                AsOfDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd")
            }
        };

        var returns = _returnsService.CalculateReturns(prices).ToList();
        returns.Count.Should().Be(2);
        returns.First().Value.Should().Be(10);
    }
}