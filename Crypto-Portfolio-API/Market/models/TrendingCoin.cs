using System.Text.Json.Serialization;
using Crypto_Portfolio_API.Data;

namespace Crypto_Portfolio_API.Market.models
{
    public class TrendingCoin: DomainBase
    {
        public int Id { get; set; } // Auto-increment identity column
        public string TrendingCoinId { get; set; } // Unique identifier from CoinGecko

        [JsonPropertyName("small")]
        public string ImageUrl { get; set; } // URL for the coin image

        [JsonPropertyName("market_cap_rank")]
        public int MarketRank { get; set; } // Rank based on market cap
    }
}
