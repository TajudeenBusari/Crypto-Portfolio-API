using Crypto_Portfolio_API.Market.models;
using Microsoft.EntityFrameworkCore;

namespace Crypto_Portfolio_API.Data
{
    /// <summary>
    /// dotnet ef migrations add AddTrendingCryptoTable
    /// 
    /// </summary>
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        
        { 
           
        
        }
        public DbSet<CryptoPrice> CryptoPrices { get; set; }

        public DbSet<TrendingCoin> TrendingCoins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //1. Configure CryptoPrice

            // Configure the primary key
            modelBuilder.Entity<CryptoPrice>().HasKey(cp => cp.Id);

            modelBuilder.Entity<CryptoPrice>()
                .HasIndex(cp => cp.CryptoId)
                .IsUnique(); // Ensure CryptoId is unique

            modelBuilder.Entity<CryptoPrice>()
                .Property(cp => cp.CryptoId)
                .IsRequired();

            // Ensure EF Core recognizes inherited properties from DomainBase
            modelBuilder.Entity<CryptoPrice>()
                .Property(cp => cp.Symbol)
                .IsRequired();

            modelBuilder.Entity<CryptoPrice>()
                .Property(cp => cp.Name)
                .IsRequired();

            //2. Configure TrendingCoin

            modelBuilder.Entity<TrendingCoin>().HasKey(tc => tc.Id);
            modelBuilder.Entity<TrendingCoin>().HasIndex(tc => tc.TrendingCoinId).IsUnique();
            modelBuilder.Entity<TrendingCoin>().Property(tc => tc.TrendingCoinId).IsRequired();
        }
    }
}
