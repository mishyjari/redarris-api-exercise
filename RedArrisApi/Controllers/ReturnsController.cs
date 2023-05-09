using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using static RedArrisApi.DateTimeExtensions;

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
    /// <param name="from">The start date of the time range in YYYY-MM-DD format.</param>
    /// <param name="to">The end date of the time range in YYYY-MM-DD format.</param>
    /// <response code="200">Returns the list of returns.</response>
    /// <response code="400">The specified ticker or date range is invalid.</response>
    /// <response code="422">Not enough price results to calculate returns</response>
    /// <response code="500">An error occurred while processing the request.</response>
    [HttpGet("{ticker}")]
    [ProducesResponseType(typeof(ReturnDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 422)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetReturnsAsync(
        string ticker,
        [FromQuery(Name = "from")] string? from = null,
        [FromQuery(Name = "to")] string? to = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return new BadRequestObjectResult("Ticker is required in request");

            // We want normalize date input to yyyy-MM-dd format if provided, or assume YTD values if not.
            from = from?.NormalizeDateString() ?? GetFirstDayOfYearString();
            to = to?.NormalizeDateString() ?? DateTime.Now.ToString("yyyy-MM-dd");

            // Using an arbitrary limit of 1 year for results, also catch backwards from and to here
            if (DateTime.Parse(to) - DateTime.Parse(from) > TimeSpan.FromDays(365))
                return new BadRequestObjectResult("Maximum allowed range is one year");

            // Handle backwards from/to dates.
            if (DateTime.Parse(to) < DateTime.Parse(from))
                return new BadRequestObjectResult("Requested to date is before from date.");

            var prices = await _pricesService.GetPricesAsync(ticker, from, to);
            
            // We need at least two values to calculate return, throw exception (caught in controller) if we don't meet this condition
            if (prices.Count() < 2)
                return new UnprocessableEntityObjectResult(
                    "At least two price entities are required to calculate returns, however fewer than two results were returned from Price Service.");
            
            var returns = _returnsService.CalculateReturns(prices.ToArray());
            
            return Ok(new ReturnDto()
            {
                Returns = returns,
                Symbol = ticker.ToUpper(),
                FromDate = DateTime.Parse(from),
                ToDate = DateTime.Parse(to),
            });
        }
        catch (Exception _)
        {
            return new StatusCodeResult(500);
        }
    }
}