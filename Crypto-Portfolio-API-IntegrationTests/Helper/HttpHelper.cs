namespace Crypto_Portfolio_API_IntegrationTests.Helper;

public static class HttpHelper
{
    public static class Urls
    {
        //Market controller
        public const string GetPrices = "api/v1/market/prices";
        public const string FetchAndUpdatePrices = "api/v1/market/prices/fetch";
        public const string GetTrendingCoins = "api/v1/market/trending";
        public const string FetchAndUpdateTrendingCoins = "api/v1/market/trending/fetch";
    }
    
}