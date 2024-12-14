using Crypto_Portfolio_API.Market.models;

namespace Crypto_Portfolio_API.Market.service
{
    public interface ICryptoPriceService
    {
        Task<IEnumerable<CryptoPrice>> GetAllPrices();
        Task<IEnumerable<CryptoPrice>> FetchAndSavePricesAsync();
        Task<IEnumerable<TrendingCoin>> GetAndSaveAllTrendingCoinsAsync();
        Task<IEnumerable<TrendingCoin>> GetAllTrendingCoins();
    }
}
