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
    /// <param name="start">The start date of the time range in YYYY-MM-DD format.</param>
    /// <param name="end">The end date of the time range in YYYY-MM-DD format.</param>
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
        [FromQuery(Name = "start")] string? start = null,
        [FromQuery(Name = "end")] string? end = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return new BadRequestObjectResult("Ticker is required in request");

            // We want normalize date input to yyyy-MM-dd format if provided, or assume YTD values if not.
            start = start?.NormalizeDateString() ?? GetFirstDayOfYearString();
            end = end?.NormalizeDateString() ?? DateTime.Now.ToString("yyyy-MM-dd");

            // This shouldn't be necessary, as anything that fails to parse will return null and get passed off to the YTD default
            // But we may want to use this should we decide to throw a 400 instead of falling back to default values
            if (!DateTime.TryParse(start, out _) || !DateTime.TryParse(end, out _))
                return new BadRequestObjectResult("Invalid date input in request. Please use YYYY-MM-DD format.");

            // Using an arbitrary limit of 1 year for results, also catch backwards start and end here
            if (DateTime.Parse(end) - DateTime.Parse(start) > TimeSpan.FromDays(365))
                return new BadRequestObjectResult("Maximum allowed range is one year");

            // Handle backwards start/end dates.
            if (DateTime.Parse(end) < DateTime.Parse(start))
                return new BadRequestObjectResult("Requested end date is before start date.");

            var prices = await _pricesService.GetPricesAsync(ticker, start, end);
            
            // We need at least two values to calculate return, throw exception (caught in controller) if we don't meet this condition
            if (prices.Count() < 2)
                return new UnprocessableEntityObjectResult(
                    "At least two price entities are required to calculate returns, however fewer than two results were returned from Price Service.");
            
            var returns = _returnsService.CalculateReturns(prices.ToArray()).ToList();

            return Ok(new ReturnDto()
            {
                Returns = returns,
                Ticker = ticker.ToUpper(),
                Start = DateTime.Parse(start),
                End = DateTime.Parse(end),
                Count = returns.Count
            });
        }
        catch (Exception _)
        {
            return new StatusCodeResult(500);
        }
    }
}