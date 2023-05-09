using Newtonsoft.Json;

namespace RedArrisApi;

// IEX is giving us much more data than this, currently using date, adj. close price and symbol
// See docs for IEX Historical Prices for more: https://iexcloud.io/docs/core/HISTORICAL_PRICES
public class IexPriceDto
{
    [JsonProperty("fclose")] public double ClosePrice { get; set; }

    [JsonProperty("priceDate")] public string AsOfDate { get; set; } = string.Empty;

    [JsonProperty("symbol")] public string Symbol { get; set; } = string.Empty;
}