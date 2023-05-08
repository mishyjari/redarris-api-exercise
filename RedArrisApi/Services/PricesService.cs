using RedArrisApi.Models;
using static RedArrisApi.DateTimeExtensions;

namespace RedArrisApi;

public class PricesService : IPricesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IexSettings _iexSettings;

    public PricesService(IHttpClientFactory httpClientFactory, IexSettings iexSettings)
    {
        _httpClientFactory = httpClientFactory;
        _iexSettings = iexSettings;
    }

    public async Task<IEnumerable<IexPriceResponse>> GetPricesAsync(string ticker, string? start = null,
        string? end = null)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker is required in request");

        // We want normalize date input to yyyy-MM-dd format if provided, or assume YTD if not.
        start = start?.NormalizeDateString() ?? GetFirstDayOfYearString();
        end = end?.NormalizeDateString() ?? DateTime.Now.ToString("yyyy-MM-dd");

        // DateTime.Parse() will work wonders, but we should handle cases in which it fails to parse
        if (!DateTime.TryParse(start, out _) || !DateTime.TryParse(end, out _))
            throw new ArgumentException("Invalid date input in request. Please use YYYY-MM-DD format.");

        // Using an arbitrary limit of 1 year for results
        if (DateTime.Parse(end) - DateTime.Parse(start) > TimeSpan.FromDays(365))
            throw new ArgumentException("Maximum allowed range is one year");

        var client = _httpClientFactory.CreateClient(_iexSettings.IexHttpClientName);

        var requestUrl = $"{_iexSettings.IexPricesUrl}/{ticker}?from={start}&to={end}&token={_iexSettings.IexApiKey}";

        var response = await client.GetAsync(requestUrl);

        return await response.Content.ReadAsAsync<IEnumerable<IexPriceResponse>>();
    }
}