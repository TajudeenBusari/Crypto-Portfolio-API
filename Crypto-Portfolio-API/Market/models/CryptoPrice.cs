using System.Text.Json.Serialization;
using Crypto_Portfolio_API.Data;
using Newtonsoft.Json;

namespace Crypto_Portfolio_API.Market.models
{
    public class CryptoPrice: DomainBase
    {
        public int Id { get; set; } // Auto-increment identity column
       
        public string CryptoId { get; set; } // Unique, provided by the API

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdate { get; set; }
    }
}
