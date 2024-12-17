using System.Text.Json;
using System.Text.Json.Serialization;
using Crypto_Portfolio_API.Data;
using Crypto_Portfolio_API.Market.models;
using Crypto_Portfolio_API.Market.repository;


namespace Crypto_Portfolio_API.Market.service.impl
{
    public class CryptoPriceService : ICryptoPriceService
    {
        private readonly ICryptoPriceRepository _cryptoPriceRepository;
        private readonly HttpClient _httpClient;

        public CryptoPriceService(ICryptoPriceRepository cryptoPriceRepository, HttpClient httpClient)
        {
            _cryptoPriceRepository = cryptoPriceRepository;
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<CryptoPrice>> FetchAndSavePricesAsync()
        {
            //define the api end point for fetching market data (with naira or usd)
            const string endpoint =
               "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&order=market_cap_desc&per_page=50&page=1";

            // Add User-Agent header
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CryptoPriceApp/1.0 (http://localhost:5162)");

            try
            {
                var coinResponse = await _httpClient.GetStringAsync(endpoint);

                // Log the raw response for debugging
                Console.WriteLine("Raw API Response: " + coinResponse);

                //deserialize response into the list of cryptocurrencies
                var marketData = JsonSerializer.Deserialize<List<CoinMarketData>>(coinResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true

                });

                if (marketData == null || !marketData.Any())
                {
                    throw new Exception("Failed to fetch coin list from CoinGheko API.");

                }

                // Convert the API data into CryptoPrice entities
                var cryptoPrices = marketData.Select(data => new CryptoPrice
                {
                    
                    CryptoId = data.Id,
                    Symbol = data.Symbol,
                    Name = data.Name,
                    CurrentPrice = data.CurrentPrice != 0 ? data.CurrentPrice : 0m,// Ensure price is not 0
                    MarketCap = data.MarketCap != 0? data.MarketCap : 0, // Ensure market cap is not 0
                    LastUpdate = DateTime.UtcNow,
                    //LastUpdate = data.LastUpdated

                }).ToList();

                await _cryptoPriceRepository.SaveOrUpdatePricesAsync(cryptoPrices);

                // Reload prices from the database to include the assigned Ids
                var updatedPrices = await _cryptoPriceRepository.GetAllPricesAsync();

                //return cryptoPrices;
                return updatedPrices;

            }
            catch (HttpRequestException ex) when ((int)ex.StatusCode == 403)
            {
                Console.WriteLine("Access forbidden: Check API headers, rate limits, and IP restrictions.");
                throw;
            }

        }

        public async Task<IEnumerable<CryptoPrice>> GetAllPrices()
        {
            return await _cryptoPriceRepository.GetAllPricesAsync();
        }

        public async Task<IEnumerable<TrendingCoin>> GetAllTrendingCoins()
        {
           return await _cryptoPriceRepository.GetAllTrendingCoinsAsync();
        }

        public async Task<IEnumerable<TrendingCoin>> GetAndSaveAllTrendingCoinsAsync()
        {
            const string endpoint = "https://api.coingecko.com/api/v3/search/trending";

            // Add User-Agent header
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CryptoPriceApp/1.0 (http://localhost:5162)");

            try 
            { 
                var response = await _httpClient.GetStringAsync(endpoint);

                // Log the raw response for debugging
                Console.WriteLine("Raw API Response: " + response);

                //Deserialize response to a DTO
                var trendingCoinData = JsonSerializer.Deserialize<TrendingResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (trendingCoinData == null || trendingCoinData.Coins == null || !trendingCoinData.Coins.Any()) 
                
                {
                    throw new Exception("Failed to fetch trending coind from CoinGheko API.");
                
                }
                // Map DTO to TrendingCoin entity
                var trendingCoins = trendingCoinData.Coins.Select(coin => new TrendingCoin 
                
                {
                    TrendingCoinId = coin.Item.Id,
                    Symbol = coin.Item.Symbol,
                    Name = coin.Item.Name,
                    ImageUrl = coin.Item.Small,
                    MarketCap = ParseMarketCap(coin.Item.Data?.MarketCap)  ?? 0,
                    CurrentPrice = coin.Item.Data.Price ?? 0m,  // Use nested `Data` property for current price
                    MarketRank = coin.Item.MarketCapRank   // Use market cap rank directly

                }).ToList();

                // Save to the database
                await _cryptoPriceRepository.GetAndSaveTrendingCoinsAsync(trendingCoins);

                // Reload trendingcoin from the database to include the assigned Ids
                var updatedTrendingCoins = await _cryptoPriceRepository.GetAllTrendingCoinsAsync();
                return updatedTrendingCoins;



            }
            catch (HttpRequestException ex) 
            
            {
                Console.WriteLine($"Error fetching trending coins: {ex.Message}");
                throw;
            } 

        }

        // Helper method to parse market cap string into a decimal value
        private decimal? ParseMarketCap(string marketCapString)
        {
            if (string.IsNullOrEmpty(marketCapString)) return null;

            // Remove any special characters like "$" or "," before parsing
            marketCapString = marketCapString.Replace("$", "").Replace(",", "");

            if (decimal.TryParse(marketCapString, out var marketCap))
            {
                return marketCap;
            }

            return null;
        }
    }

    //Dto for deserializing Coingheko API market data
    public class CoinMarketData//: DomainBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
       
       [JsonPropertyName("last_updated")]
       public DateTime LastUpdated { get; set; }
       
       [JsonPropertyName("symbol")]
       public string Symbol { get; set; } //"BTC"

       [JsonPropertyName("name")]
       public string Name { get; set; } //"Bitcoin"

       [JsonPropertyName("market_cap")]
       public decimal? MarketCap { get; set; } // 1980069276557

       [JsonPropertyName("current_price")]
       public decimal? CurrentPrice { get; set; } // 100000.5
    }

    // DTO for trending data from coinghecko API
    public class TrendingResponse
    {
        public List<TrendingCoinDTO> Coins { get; set; }
    }

    public class TrendingCoinDTO
    {
        public TrendingCoinItem Item { get; set; }
    }

    public class TrendingCoinItem
    {
        public string Id { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("small")]
        public string Small { get; set; } // Image URL

        [JsonPropertyName("market_cap_rank")]
        public int MarketCapRank { get; set; }

        [JsonPropertyName("data")]
        public CoinData Data { get; set; }

       

        public class CoinData
        {
            [JsonPropertyName("price")]
            public decimal? Price { get; set; }

            [JsonPropertyName("market_cap")]
            public string MarketCap { get; set; } // e.g., "$44,963,302"

        }
    }
}
