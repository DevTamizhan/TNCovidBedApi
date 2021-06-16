using NUnit.Framework;
using TNCovidBedApi;

namespace Tests
{
    public class ApiServiceUnitTests
    {

        private ApiService service;

        [SetUp]
        public void Setup()
        {
            service = new ApiService();
        }

        [Test(Description ="Checks for the NoCacheException when district cache data is empty")]
        [Order(1)]
        public void NoCacheExceptionDistrcits()
        {
            Assert.Throws(typeof(NoCacheException), () => service.GetAllDistrictsCache());
        }

        [Test(Description ="Checks for the NoCacheException when hospital cache data is empty")]
        [Order(2)]
        public void NoCacheExceptionHospitals()
        {
            Assert.Throws(typeof(NoCacheException), () => service.GetBedDetailsCache());
        }
    }
}