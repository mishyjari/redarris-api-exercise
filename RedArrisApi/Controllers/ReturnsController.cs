using Microsoft.AspNetCore.Mvc;
using RedArrisApi.Models;

namespace RedArrisApi;

[ApiController]
[Route("[controller]")]
public class ReturnsController : ControllerBase
{
    private readonly IPricesService _pricesService;
    private readonly IReturnsService _returnsService;

    public ReturnsController(IReturnsService returnsService, IPricesService pricesService)
    {
        _returnsService = returnsService;
        _pricesService = pricesService;
    }

    /// <summary>
    ///     Gets the returns for the specified ticker within the specified time range.
    /// </summary>
    /// <param name="ticker">The ticker symbol for the stock.</param>
    /// <param name="start">The start date of the time range in YYYY-MM-DD format.</param>
    /// <param name="end">The end date of the time range in YYYY-MM-DD format.</param>
    /// <response code="200">Returns the list of returns.</response>
    /// <response code="400">The specified ticker or date range is invalid.</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet("{ticker}")]
    [ProducesResponseType(typeof(IEnumerable<Return>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetReturnsAsync(
        string ticker,
        [FromQuery(Name = "start")] string? start = null,
        [FromQuery(Name = "end")] string? end = null)
    {
        try
        {
            var prices = await _pricesService.GetPricesAsync(ticker, start, end);
            var returns = _returnsService.CalculateReturns(prices.ToArray());

            return Ok(returns);
        }
        catch (ArgumentException ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
        catch (Exception _)
        {
            return new StatusCodeResult(500);
        }
    }
}