using Crypto_Portfolio_API.Market.models;

namespace Crypto_Portfolio_API.Market.repository
{
    public interface ICryptoPriceRepository
    {
        Task<IEnumerable<CryptoPrice>> GetAllPricesAsync();
        Task SaveOrUpdatePricesAsync(IEnumerable<CryptoPrice> prices);

        Task<IEnumerable<TrendingCoin>> GetAllTrendingCoinsAsync();
        Task GetAndSaveTrendingCoinsAsync(IEnumerable<TrendingCoin> coins);
    }
}
