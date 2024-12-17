using System.Net;
using Crypto_Portfolio_API_IntegrationTests.Fixtures;
using Crypto_Portfolio_API_IntegrationTests.Helper;
using Crypto_Portfolio_API.Market.models;
using Crypto_Portfolio_API.System;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace Crypto_Portfolio_API_IntegrationTests.SUT
{
    public class CryptoMarketControllerTestWithDocker: IClassFixture<CustomDockerWebApplicationFactory>
    {
        private readonly CustomDockerWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _testOutputHelper;

        public CryptoMarketControllerTestWithDocker(CustomDockerWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        {
            _factory = factory;
            _testOutputHelper = testOutputHelper;
            _client = _factory.CreateClient();
        }


        [Fact, TestPriority(1)]
        public async Task TestFetchAndUpdateCryptoPrices()
        {
            //Arrange
            
            //Act
            /***
             * Because the post request does not take a request body in the controller,
             * an empty string content must be passed along the url using the PostAsync method
             */
            var response = await _client.PostAsync(HttpHelper.Urls.FetchAndUpdatePrices, new StringContent(""));
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<Result>(responseString);
            Assert.True(deserializedResponse?.flag);
            Assert.Equal(200, deserializedResponse?.code);
            Assert.Equal("Find All Success", deserializedResponse?.message);
            var jsonArray = deserializedResponse?.data;
            _testOutputHelper.WriteLine(jsonArray?.ToString());
            var deserializedArray = JsonConvert.DeserializeObject<List<CryptoPrice>>(jsonArray?.ToString());
            deserializedArray.Should().NotBeEmpty();

        }
        
        /// <summary>
        /// if you want to run this test alone, an empty array
        /// will be printed because no data is seeded to the DB on start.
        /// Running all tests at once will print some data out because the TestFetchAndUpdateCryptoPrices()
        /// runs first
        /// </summary>
        [Fact, TestPriority(2)]
        public async Task TestGetCryptoPricesSuccess()
        {
            //Arrange
            
            //Act
            var response = await _client.GetAsync(HttpHelper.Urls.GetPrices);
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<Result>(content);
            Assert.True(deserializedObject?.flag);
            Assert.Equal(200, deserializedObject?.code);
            Assert.Equal("Find All Success", deserializedObject?.message);
            var jsonArray = deserializedObject?.data;
            //_testOutputHelper.WriteLine(JsonConvert.SerializeObject(jsonArray)); //also works
            _testOutputHelper.WriteLine(jsonArray?.ToString());

        }

        [Fact, TestPriority(3)]
        public async Task TestFetchAndUpdateTrendingCoinsSuccess()
        {
            //Arrange
            //Act
            var response = await _client.PostAsync(HttpHelper.Urls.FetchAndUpdateTrendingCoins, new StringContent(""));
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<Result>(content);
            Assert.True(deserializedObject?.flag);
            Assert.Equal(200, deserializedObject?.code);
            Assert.Equal("Find All Success", deserializedObject?.message);
            var jsonArray = deserializedObject?.data;
            _testOutputHelper.WriteLine(jsonArray?.ToString());
            var deserializedArray = JsonConvert.DeserializeObject<List<TrendingCoin>>(jsonArray.ToString());
            deserializedArray.Should().NotBeEmpty();

            
        }
        
        /// <summary>
        /// if you want to run this test alone, an empty array
        /// will be printed because no data is seeded to the DB on start.
        /// Running all tests at once will print some data out because the TestFetchAndUpdateTrendingCoinsSuccess()
        /// runs first
        /// </summary>
        [Fact, TestPriority(4)]
        public async Task TestGetTrendingCoinsSuccess()
        {
           //Arrange
           //Act
           var response = await _client.GetAsync(HttpHelper.Urls.GetTrendingCoins);
           
           //Assert
           response.StatusCode.Should().Be(HttpStatusCode.OK);
           var content = await response.Content.ReadAsStringAsync();
           var deserializedObject = JsonConvert.DeserializeObject<Result>(content);
           Assert.True(deserializedObject?.flag);
           Assert.Equal(200, deserializedObject?.code);
           Assert.Equal("Find All Success", deserializedObject?.message);
           var jsonArray = deserializedObject?.data;
           _testOutputHelper.WriteLine(jsonArray?.ToString());
           
        }
    }
}
