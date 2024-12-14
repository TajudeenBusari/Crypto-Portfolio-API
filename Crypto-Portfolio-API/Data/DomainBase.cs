using System.Text.Json.Serialization;

namespace Crypto_Portfolio_API.Data
{
    public class DomainBase
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } //"BTC"

        [JsonPropertyName("name")]
        public string Name { get; set; } //"Bitcoin"

        [JsonPropertyName("market_cap")]
        public decimal? MarketCap { get; set; } // 1980069276557

        [JsonPropertyName("current_price")]
        public decimal? CurrentPrice { get; set; } // 100000.5
    }
}
