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

    public async Task<IEnumerable<IexPriceResponse>> GetPricesAsync(string ticker, string start, string end)
    {
        var client = _httpClientFactory.CreateClient(_iexSettings.IexHttpClientName);

        var requestUrl = $"{_iexSettings.IexPricesUrl}/{ticker}?from={start}&to={end}&sort=ASC&token={_iexSettings.IexApiKey}";

        var response = await client.GetAsync(requestUrl);

        return await response.Content.ReadAsAsync<IEnumerable<IexPriceResponse>>();
    }
}