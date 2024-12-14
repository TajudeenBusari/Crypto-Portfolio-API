using Crypto_Portfolio_API.Market.service;
using Crypto_Portfolio_API.System;
using Microsoft.AspNetCore.Mvc;

namespace Crypto_Portfolio_API.Market.controllers
{
    [ApiController]
    [Route("api/v1/market")]
    public class CryptoMarketController: ControllerBase
    {
        private readonly ICryptoPriceService _cryptoPriceService;

        public CryptoMarketController(ICryptoPriceService cryptoPriceService)
        {
            _cryptoPriceService = cryptoPriceService;
            
        }

        [HttpGet("prices")]
        public async Task <ActionResult<Result>> GetPrices()
        {
            var prices = await _cryptoPriceService.GetAllPrices();

            if (prices == null) 
            {
                return NotFound("No data available");
            }
            return Ok(new Result (true, System.StatusCode.Success, "Find All Success", prices));
        }

        [HttpPost("prices/fetch")]
        public async Task <ActionResult<Result>> FetchAndUpdatePrices()
        {
            var prices = await _cryptoPriceService.FetchAndSavePricesAsync();

            return Ok(new Result(true, System.StatusCode.Success, "Find All Success", prices));
        }

        [HttpGet("trending")]
        public async Task <ActionResult<Result>> GetTrendingCoins()
        {
            var trending = await _cryptoPriceService.GetAllTrendingCoins();
            if (trending == null)
            {
                return NotFound("No data available");
            }
            
            return Ok(new Result(true, System.StatusCode.Success, "Find All Success", trending));
        }

        [HttpPost("trending/fetch")]
        public async Task <ActionResult<Result>> FetchAndUpdateTrendingCoins()
        {
            var trending = await _cryptoPriceService.GetAndSaveAllTrendingCoinsAsync();
            return Ok(new Result(true, System.StatusCode.Success, "Find All Success", trending));
        }
    }
}
