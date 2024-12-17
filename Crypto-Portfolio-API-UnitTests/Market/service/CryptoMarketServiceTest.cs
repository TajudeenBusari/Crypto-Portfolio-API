using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Crypto_Portfolio_API.Market.models;
using Crypto_Portfolio_API.Market.repository;
using Crypto_Portfolio_API.Market.service.impl;
using Moq;
using Moq.Protected;

namespace Crypto_Portfolio_API_UnitTests.Market.service
{
    public class CryptoMarketServiceTest
    {
        private readonly CryptoPriceService _cryptoPriceService;

        private readonly Mock<ICryptoPriceRepository> _mockCryptoPriceRepository;

        private readonly HttpClient _httpClient;

        private readonly Mock<HttpMessageHandler> _mockHttMessageHandler;

        public CryptoMarketServiceTest()
        {
            _mockCryptoPriceRepository = new Mock<ICryptoPriceRepository>();

            _mockHttMessageHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHttMessageHandler.Object);

            _cryptoPriceService = new CryptoPriceService(_mockCryptoPriceRepository.Object, _httpClient);
            
        }

        [Fact]
        public async Task TestFetchAndSaveCryptoPriceSuccess()
        {
            //Arrange
            //_mockHttMessageHandler = new Mock<HttpMessageHandler>();

            // Mock the HttpClient to simulate API response for fetching market data
            _mockHttMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()) // Name of the protected method
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    // Simulated response JSON from CoinGecko API

                    var resContent = @"
                [
                    {
                        ""id"": ""bitcoin"",
                        ""symbol"": ""btc"",
                        ""name"": ""Bitcoin"",
                        ""current_price"": 101743,
                        ""market_cap"": 2013913157166,
                        ""last_updated"": ""2024-12-15T08:03:05.741Z""
                    },
                    {
                        ""id"": ""ethereum"",
                        ""symbol"": ""eth"",
                        ""name"": ""Ethereum"",
                        ""current_price"": 3845.34,
                        ""market_cap"": 463058440577,
                        ""last_updated"": ""2024-12-15T08:03:10.470Z""
                    }
                ]";

                    // Create the HttpResponseMessage
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(resContent, Encoding.UTF8, "application/json")
                    };

                });


            //mock
            _mockCryptoPriceRepository
                .Setup(repo =>
                repo.SaveOrUpdatePricesAsync(It.IsAny<IEnumerable<CryptoPrice>>()))
                .Returns(Task.CompletedTask);

            _mockCryptoPriceRepository
                .Setup(repo => repo.GetAllPricesAsync())
                .ReturnsAsync(new List<CryptoPrice>

                {
                    new CryptoPrice
                    {
                        CryptoId = "bitcoin",
                        Symbol = "btc",
                        Name = "Bitcoin",
                        CurrentPrice = 101743m,
                        MarketCap = 2013913157166m,
                        LastUpdate = DateTime.UtcNow
                        

                    },
                    new CryptoPrice
                    {
                        CryptoId = "ethereum",
                        Symbol = "eth",
                        Name = "Ethereum",
                        CurrentPrice =  3845.34m,
                        MarketCap = 463058440577,
                        LastUpdate = DateTime.UtcNow
                        

                    }

                });

            //Act
            var result = await _cryptoPriceService.FetchAndSavePricesAsync();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Symbol == "btc");
            Assert.Contains(result, c => c.Symbol == "eth");
            _mockCryptoPriceRepository.Verify(repo => repo.SaveOrUpdatePricesAsync(It.IsAny<IEnumerable<CryptoPrice>>()), Times.Once());
            _mockCryptoPriceRepository.Verify(repo => repo.GetAllPricesAsync(), Times.Once());
        }
        
        [Fact]
        public async Task TestGetAndSaveAllTrendingCoinSuccess()
        {
            //Arrange
            //Mock the HttpMessageHandler to simulate API calls/Responses for fetching trending coins
            _mockHttMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()) // Name of the protected method
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    //Simulated response JSON from Coingheko´s trending endpoint
                    var jsonResponse = @"{
                        ""coins"": 
                    [
                            {
                                ""item"": {
                                    ""id"": ""bitcoin"",
                                    ""symbol"": ""btc"",
                                    ""name"": ""Bitcoin"",
                                    ""small"": ""https://assets.coingecko.com/coins/images/1/small/bitcoin.png?1547033579"",
                                    ""market_cap_rank"": 1,
                                    ""data"": {
                                        ""price"": 34500.12,
                                        ""market_cap"": ""$700,000,000,000""
                                    }
                                }
                            },
                            {
                                ""item"": {
                                    ""id"": ""ethereum"",
                                    ""symbol"": ""eth"",
                                    ""name"": ""Ethereum"",
                                    ""small"": ""https://assets.coingecko.com/coins/images/279/small/ethereum.png?1595348880"",
                                    ""market_cap_rank"": 2,
                                     ""data"": {
                                   ""price"": 2450.45,
                                    ""market_cap"": ""300000000000""
                                }
                            }
                        }
                    ]
                }";
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                    };
                });


            var savedTrendingCoins = new List<TrendingCoin>
            {
                new TrendingCoin()
                {
                    TrendingCoinId = "bitcoin",
                    Symbol = "btc",
                    Name = "Bitcoin",
                    ImageUrl = "https://assets.coingecko.com/coins/images/1/small/bitcoin.png",
                    MarketCap = 700000000000m,
                    CurrentPrice = 34500.12m,
                    MarketRank = 1
                    
                },
                new TrendingCoin()
                {
                    TrendingCoinId = "ethereum",
                    Symbol = "eth",
                    Name = "Ethereum",
                    ImageUrl = "https://assets.coingecko.com/coins/images/279/small/ethereum.png",
                    MarketCap = 300000000000m,
                    CurrentPrice = 2450.45m,
                    MarketRank = 2
                    
                }

            };
            
            //Mock
            _mockCryptoPriceRepository.Setup(repo => 
                    repo.GetAndSaveTrendingCoinsAsync(It.IsAny<IEnumerable<TrendingCoin>>()))
                .Returns(Task.CompletedTask);
            
            _mockCryptoPriceRepository.Setup(repo => repo.GetAllTrendingCoinsAsync())
                .ReturnsAsync(savedTrendingCoins);
            
            //Act
            var result = await _cryptoPriceService.GetAndSaveAllTrendingCoinsAsync();
            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Symbol == "btc");
            Assert.Contains(result, c => c.Symbol == "eth");
            _mockCryptoPriceRepository.Verify(repo => repo.GetAndSaveTrendingCoinsAsync(It.IsAny<IEnumerable<TrendingCoin>>()), Times.Once());
            _mockCryptoPriceRepository.Verify(repo => repo.GetAllTrendingCoinsAsync(), Times.Once());
        }

        [Fact]
        public async Task TestGetAllPricesSuccess()
        {
            _mockCryptoPriceRepository.Setup(repo => repo.GetAllPricesAsync()).ReturnsAsync(new List<CryptoPrice>

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
            });

            //Act;
            var result = await _cryptoPriceService.GetAllPrices();

            //Assert
            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockCryptoPriceRepository.Verify(repo =>  repo.GetAllPricesAsync(), Times.Once());
            
        }

        [Fact]
        public async Task TestGetAllTrendingCoinSuccess()
        {
            //Arrange

            //Mock
            _mockCryptoPriceRepository.Setup(repo =>
                repo.GetAllTrendingCoinsAsync()).ReturnsAsync(new TrendingCoin[] 
                
                { 
                    new TrendingCoin 
                    { 
                        TrendingCoinId = "bitcoin",
                        Symbol = "btc",
                        ImageUrl = "image.png",
                        MarketRank = 1,
                        MarketCap = 300000000000m,
                        Name = "Bitcoin",
                        CurrentPrice = 103025

                    },
                    new TrendingCoin 
                    {
                        TrendingCoinId = "ethereum",
                        Symbol = "eth",
                        ImageUrl = "image2.png",
                        MarketRank = 1,
                        MarketCap = 469777547940,
                        Name = "Ethereum",
                        CurrentPrice = 3899

                    }
                
                });
                

            //Act
            var result = await _cryptoPriceService.GetAllTrendingCoins();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockCryptoPriceRepository.Verify(repo => repo.GetAllTrendingCoinsAsync(), Times.Once());
        }
    }
    
    
}
