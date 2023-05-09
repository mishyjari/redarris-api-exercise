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

    public async Task<IEnumerable<IexPriceDto>> GetPricesAsync(string ticker, string from, string to)
    {
        var client = _httpClientFactory.CreateClient(_iexSettings.IexHttpClientName);

        var requestUrl =
            $"{_iexSettings.IexPricesUrl}/{ticker}?from={from}&to={to}&sort=ASC&token={_iexSettings.IexApiKey}";

        var response = await client.GetAsync(requestUrl);

        // Exception will be caught in the controller
        if (!response.IsSuccessStatusCode)
            throw new Exception("Non-successful response from IEX API while trying to retrieve price data. " +
                                $"Status: {response.StatusCode} - {response.ReasonPhrase}");

        return await response.Content.ReadAsAsync<IEnumerable<IexPriceDto>>();
    }
}