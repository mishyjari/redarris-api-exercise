namespace RedArrisApi;

public class IexSettings
{
    public string IexPricesUrl { get; set; } = "https://api.iex.cloud/v1/data/core/historical_prices";
    public string IexHttpClientName { get; set; } = "IexHttpClient";
    public string IexApiKey { get; set; }
}