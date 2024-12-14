using Crypto_Portfolio_API.Data;
using Crypto_Portfolio_API.Market.models;
using Microsoft.EntityFrameworkCore;

namespace Crypto_Portfolio_API.Market.repository.impl
{
    public class CryptoPriceRepository : ICryptoPriceRepository
    {
        private readonly ApplicationDbContext _dbcontext;
        public CryptoPriceRepository(ApplicationDbContext dbContext) 
        {
            _dbcontext = dbContext;
        }
        public async Task<IEnumerable<CryptoPrice>> GetAllPricesAsync()
        {
            return await _dbcontext.CryptoPrices.ToListAsync();
            
        }

        public async Task GetAndSaveTrendingCoinsAsync(IEnumerable<TrendingCoin> coins)
        {
            //clear existing trending coins
             _dbcontext.TrendingCoins.RemoveRange(_dbcontext.TrendingCoins);

            //Add new trending coins
            await _dbcontext.TrendingCoins.AddRangeAsync(coins);

            //save changes
            await _dbcontext.SaveChangesAsync();
            
            
        }

        public async Task<IEnumerable<TrendingCoin>> GetAllTrendingCoinsAsync()
        {
            return await _dbcontext.TrendingCoins.OrderBy(tc => tc.MarketRank).ToListAsync();
        }




        //public async Task SaveOrUpdatePricesAsync(IEnumerable<CryptoPrice> prices)
        //{
        //    var exsitingPrices = await _dbcontext
        //        .CryptoPrices
        //        .Where(p => prices.Select(price => price.Symbol)
        //        .Contains(p.Symbol)).ToDictionaryAsync(p => p.Symbol);   



        //    foreach (var price in prices) 

        //    {

        //        if (exsitingPrices.TryGetValue(price.Symbol, out var existing))
        //        {
        //            existing.CurrentPrice = price.CurrentPrice;
        //            existing.LastUpdate = price.LastUpdate;
        //        }
        //        else 
        //        {
        //            await _dbcontext.CryptoPrices.AddAsync(price);
        //        }

        //    }
        //    await _dbcontext.SaveChangesAsync();
        //}

        public async Task SaveOrUpdatePricesAsync(IEnumerable<CryptoPrice> prices)
        {
            foreach (var cryptoPrice in prices)
            {
                // Check if the entity already exists in the database
                var existingEntity = await _dbcontext.CryptoPrices
                    .FirstOrDefaultAsync(cp => cp.CryptoId == cryptoPrice.CryptoId);

                if (existingEntity != null)
                {
                    // Update the existing entity
                    existingEntity.CurrentPrice = cryptoPrice.CurrentPrice;
                    existingEntity.LastUpdate = cryptoPrice.LastUpdate;
                    existingEntity.MarketCap = cryptoPrice.MarketCap;
                }
                else
                {
                    // Add the new entity
                    await _dbcontext.CryptoPrices.AddAsync(cryptoPrice);
                }
            }

            // Save changes to the database
            await _dbcontext.SaveChangesAsync();
        }

    }
    
}
