using Crypto_Portfolio_API.Market.controllers;
using Crypto_Portfolio_API.Market.models;
using Crypto_Portfolio_API.Market.service;
using Crypto_Portfolio_API.System;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Crypto_Portfolio_API_UnitTests.Market.controller
{
    public class CryptoMarketControllerTest
    {
        private readonly CryptoMarketController _controller;
        private readonly Mock<ICryptoPriceService> _mockCryptoPriceService;

        public CryptoMarketControllerTest()
        {
            _mockCryptoPriceService = new Mock<ICryptoPriceService>();
            _controller = new CryptoMarketController(_mockCryptoPriceService.Object);
        }



        [Fact]
        public async Task TestGetPricesSuccess()
        {
            //Arrange
            var prices = new List<CryptoPrice>()
            {
                new CryptoPrice 
                {
                     CryptoId = "bitcoin",
                     Symbol = "btc",
                     Name = "Bitcoin",
                     CurrentPrice = 34500.12m,
                     MarketCap = 700000000000m,
                     LastUpdate = DateTime.UtcNow
                },
                new CryptoPrice 
                {
                    CryptoId = "ethereum",
                    Symbol = "eth",
                    Name = "Ethereum",
                    CurrentPrice = 2450.45m,
                    MarketCap = 300000000000m,
                    LastUpdate = DateTime.UtcNow
                }

            };

            //mock
            _mockCryptoPriceService
                .Setup(service => 
                service.GetAllPrices())
                .ReturnsAsync(prices);

            //Act
            var result = await _controller.GetPrices();

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<Result>(okResult.Value);
            Assert.True(actualResult.flag);
            Assert.Equal(200, actualResult.code);
            Assert.Equal("Find All Success", actualResult.message);
            Assert.IsType<List<CryptoPrice>>(actualResult.data);
            var actualResultData = actualResult.data as List<CryptoPrice>;
            Assert.Equal(prices[0].CryptoId, actualResultData?[0].CryptoId);
            Assert.Equal(prices[0].Symbol, actualResultData?[0].Symbol);
            Assert.Equal(prices[0].Name, actualResultData?[0].Name);
            Assert.Equal(prices[0].CurrentPrice, actualResultData?[0].CurrentPrice);
            Assert.Equal(prices[0].MarketCap, actualResultData?[0].MarketCap);
            Assert.Equal(prices[0].LastUpdate, actualResultData?[0].LastUpdate);

        }

        [Fact]
        public async Task TestGetPricesReturnsNullWhenNoPriceExists()
        {
            //Arrange


            //Mock
            _mockCryptoPriceService.Setup(service => service.GetAllPrices()).ReturnsAsync((List<CryptoPrice>)null);

            //Act
            var result = await _controller.GetPrices();
            
            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No data available", notFoundResult.Value);
        }

        [Fact]
        public async Task TestFetchAndUpdatePricesSuccess() 
        {
            //Arrange
            //Arrange
            var prices = new List<CryptoPrice>()
            {
                new CryptoPrice
                {
                     CryptoId = "bitcoin",
                     Symbol = "btc",
                     Name = "Bitcoin",
                     CurrentPrice = 34500.12m,
                     MarketCap = 700000000000m,
                     LastUpdate = DateTime.UtcNow
                },
                new CryptoPrice
                {
                    CryptoId = "ethereum",
                    Symbol = "eth",
                    Name = "Ethereum",
                    CurrentPrice = 2450.45m,
                    MarketCap = 300000000000m,
                    LastUpdate = DateTime.UtcNow
                }
            };

            //Mock
            _mockCryptoPriceService.Setup(service => 
            service.FetchAndSavePricesAsync())
                .ReturnsAsync(prices);

            //Act
            var result = await _controller.FetchAndUpdatePrices();

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<Result>(okResult.Value);
            Assert.True(actualResult.flag);
            Assert.Equal(200, actualResult.code);
            Assert.Equal("Find All Success", actualResult.message);
            Assert.IsType<List<CryptoPrice>>(actualResult.data);
            var actualResultData = actualResult.data as List<CryptoPrice>;
            Assert.Equal(prices[0].CryptoId, actualResultData?[0].CryptoId);
            Assert.Equal(prices[0].Symbol, actualResultData?[0].Symbol);
            Assert.Equal(prices[0].Name, actualResultData?[0].Name);
        }

        [Fact]
        public async Task TestGetAllTrendingCoins()
        {
            //Arrange
            var trending = new List<TrendingCoin>()
            {
                new TrendingCoin
                {
                     TrendingCoinId = "bitcoin",
                     ImageUrl = "image.jpg",
                     Symbol = "btc",
                     Name = "Bitcoin",
                     CurrentPrice = 34500.12m,
                     MarketCap = 700000000000m,
                     MarketRank = 1,
                    
                },
                new TrendingCoin
                {
                    TrendingCoinId = "ethereum",
                    ImageUrl = "image1.jpg",
                    Symbol = "eth",
                    Name = "Ethereum",
                    CurrentPrice = 2450.45m,
                    MarketCap = 300000000000m,
                    MarketRank = 2,
                    
                }
            };
            //Mock
            _mockCryptoPriceService.Setup(service => service.GetAllTrendingCoins()).ReturnsAsync(trending);

            //Act
            var result = await _controller.GetTrendingCoins();

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<Result>(okResult.Value);
            Assert.True(actualResult.flag);
            Assert.Equal(200, actualResult.code);
            Assert.Equal("Find All Success", actualResult.message);
            Assert.IsType<List<TrendingCoin>>(actualResult.data);
            var actualResultData = actualResult.data as List<TrendingCoin>;
            Assert.Equal(trending[0].TrendingCoinId, actualResultData?[0].TrendingCoinId);
            Assert.Equal(trending[0].ImageUrl, actualResultData?[0].ImageUrl);
            Assert.Equal(trending[0].Symbol, actualResultData?[0].Symbol);
            Assert.Equal(trending[0].Name, actualResultData?[0].Name);
            Assert.Equal(trending[0].CurrentPrice, actualResultData?[0].CurrentPrice);
            Assert.Equal(trending[0].MarketCap, actualResultData?[0].MarketCap);
            Assert.Equal(trending[0].MarketRank, actualResultData?[0].MarketRank);
        }

        [Fact]
        public async Task TestGetTrendingCoinsReturnsNotFoundWhenNoTrendingCoinsExist()
        {
            //Arrange

            //Mock
            _mockCryptoPriceService.Setup(service => service.GetAllTrendingCoins()).ReturnsAsync((List<TrendingCoin>)null);

            //Act
            var result = await _controller.GetTrendingCoins();

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No data available", notFoundResult.Value);

        }

        [Fact]
        public async Task TestFetchAndUpdateTrendingCoinsSuccess()
        {
            //Arrange
            var trending = new List<TrendingCoin>()
            {
                new TrendingCoin
                {
                     TrendingCoinId = "bitcoin",
                     ImageUrl = "image.jpg",
                     Symbol = "btc",
                     Name = "Bitcoin",
                     CurrentPrice = 34500.12m,
                     MarketCap = 700000000000m,
                     MarketRank = 1,

                },
                new TrendingCoin
                {
                    TrendingCoinId = "ethereum",
                    ImageUrl = "image1.jpg",
                    Symbol = "eth",
                    Name = "Ethereum",
                    CurrentPrice = 2450.45m,
                    MarketCap = 300000000000m,
                    MarketRank = 2,

                }
            };

            //Mock
            _mockCryptoPriceService.Setup(service => service.GetAndSaveAllTrendingCoinsAsync()).ReturnsAsync(trending);

            //Act
            var result = await _controller.FetchAndUpdateTrendingCoins();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualResult = Assert.IsType<Result>(okResult.Value);
            Assert.True(actualResult.flag);
            Assert.Equal(200, actualResult.code);
            Assert.Equal("Find All Success", actualResult.message);
            Assert.IsType<List<TrendingCoin>>(actualResult.data);
            var actualResultData = actualResult.data as List<TrendingCoin>;
            Assert.Equal(trending[0].TrendingCoinId, actualResultData?[0].TrendingCoinId);
            Assert.Equal(trending[0].ImageUrl, actualResultData?[0].ImageUrl);
            Assert.Equal(trending[0].Symbol, actualResultData?[0].Symbol);
            Assert.Equal(trending[0].Name, actualResultData?[0].Name);
            Assert.Equal(trending[0].CurrentPrice, actualResultData?[0].CurrentPrice);
            Assert.Equal(trending[0].MarketCap, actualResultData?[0].MarketCap);
            Assert.Equal(trending[0].MarketRank, actualResultData?[0].MarketRank);
        }
    }
}
