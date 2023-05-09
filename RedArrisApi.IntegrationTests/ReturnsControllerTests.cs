using System.Threading.Tasks;
using FluentAssertions;
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
    public async Task GetsReturnsAsync()
    {
        var result = await _controller.GetReturnsAsync("msft");

        result.Should().NotBeNull();
    }
}