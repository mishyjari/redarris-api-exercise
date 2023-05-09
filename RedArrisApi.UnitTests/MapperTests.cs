using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedArrisApi.UnitTests;

[TestClass]
public class MapperTests
{
    private readonly IMapper _mapper;

    public MapperTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<PriceMapper>(); });
        _mapper = mapperConfig.CreateMapper();
    }

    [TestMethod]
    public void MapsIexDtoTupleToReturnObject()
    {
        var previousPrice = new IexPriceDto {ClosePrice = 100, AsOfDate = "2023-01-01"};
        var currentPrice = new IexPriceDto {ClosePrice = 110, AsOfDate = "2023-01-02"};
        var priceTuple = new Tuple<IexPriceDto, IexPriceDto>(previousPrice, currentPrice);

        var result = _mapper.Map<Tuple<IexPriceDto, IexPriceDto>, Return>(priceTuple);

        result.Should().NotBeNull();
        result.Value.Should().Be(10);
        result.ClosePrice.Should().Be(currentPrice.ClosePrice);
        result.AsOfDate.Should().Be(DateTime.Parse(currentPrice.AsOfDate));
    }
}