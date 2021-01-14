using gpconnect_appointment_checker.DAL;
using gpconnect_appointment_checker.DAL.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text;
using Xunit;

namespace gpconnect_appointment_checker.IntegrationTest
{
    [Collection("CachingData")]
    public class CachingServiceTests
    {
        private readonly CachingService _cachingService;
        private readonly DataService _dataService;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;

        public CachingServiceTests()
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            var mockLoggerCachingService = new Mock<ILogger<CachingService>>();
            var mockLoggerDataService = new Mock<ILogger<DataService>>();
            var mockConfSection = new Mock<IConfigurationSection>();
            mockConfSection.SetupGet(m => m[It.Is<string>(s => s == "DefaultConnection")]).Returns(connectionString);
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(a => a.GetSection(It.Is<string>(s => s == "ConnectionStrings"))).Returns(mockConfSection.Object);
            _dataService = new DataService(mockConfiguration.Object, mockLoggerDataService.Object); 
            _cachingService = new CachingService(mockConfiguration.Object, mockLoggerCachingService.Object, _dataService);
            _cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow),
                AbsoluteExpirationRelativeToNow = new TimeSpan(DateTime.Now.Day, DateTime.Now.AddHours(1).Hour, DateTime.Now.Minute, DateTime.Now.Second),
                SlidingExpiration = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.AddMinutes(30).Minute, DateTime.Now.Second)
            };
        }

        [Theory]
        [InlineData("02443b12-70c2-4166-9b8b-a6b25b1ce8f2", "Test String Number 1 - She Sells Sea Shells on the Sea Shore")]
        [InlineData("23310df5-c0e3-4f0b-a7ac-bb7a8286d66b", "Test String Number 2 - Peter Piper Picked a Peck of Pickled Peppers")]
        public void SetAndGetCacheItem(string key, string value)
        {
            var byteValue = Encoding.ASCII.GetBytes(value);
            _cachingService.SetCacheItem(key, byteValue, _cacheEntryOptions);
            var cacheItem = _cachingService.GetCacheItem(key);
            var cacheItemAsString = Encoding.ASCII.GetString(cacheItem);
            Assert.NotEmpty(cacheItem);
            Assert.Equal(value, cacheItemAsString);
        }
    }
}
