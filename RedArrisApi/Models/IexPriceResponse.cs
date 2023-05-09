using Newtonsoft.Json;

namespace RedArrisApi;

// We will use adjusted OHLC+V values from IEX
public class IexPriceResponse
{
    [JsonProperty("fopen")] public double Open { get; set; }

    [JsonProperty("fhigh")] public double High { get; set; }

    [JsonProperty("flow")] public double Low { get; set; }

    [JsonProperty("fclose")] public double Close { get; set; }

    [JsonProperty("volume")] public int Volume { get; set; }

    [JsonProperty("priceDate")] public string AsOfDate { get; set; }

    [JsonProperty("symbol")] public string Ticker { get; set; }
}