using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static RedArrisApi.IntegrationTests.TestManager;

namespace RedArrisApi.IntegrationTests;

[TestClass]
public class PricesServiceTests
{
    private readonly IPricesService _pricesService;

    public PricesServiceTests()
    {
        _pricesService = GetRequiredService<IPricesService>();
    }
    
    [TestMethod]
    public async Task GetsPricesAsync()
    {
        var prices = await _pricesService.GetPricesAsync("msft", "2022-01-01", "2022-06-01");

        prices.Should().NotBeNullOrEmpty();
    } 
}