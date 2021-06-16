using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TNCovidBedApi;
using System;
using TNCovidBedApi.Models;

namespace Tests
{
    [TestFixture]
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

        [Test(Description ="Checks whether districts were automatically downloaded when request header is created")]
        [Order(3)]
        public void RequestHeaderAutomaticDistrictDownload()
        {
            Assert.DoesNotThrow(() => RequestHeader.CreateRequestHeader("", service.AllDistricts, TNCovidBedApi.Models.FacilityType.All, HospitalSortValue.Alphabetically, true, true));
            
        }


        [Test(Description ="Verifies whether 38 districts were downloaded or not")]
        [Order(4)]
        public async Task DistrictApiDownloadAndCacheUpdate()
        {
            var districtRoot = await service.GetAllDistrictsAsync();
            Assert.AreEqual(districtRoot.Result, service.GetAllDistrictsCache());
        }

        [Test(Description ="Checks for each request header and recieved hospitals with cache")]
        [Order(5)]
        public async Task HospitalApiDownloadAndCacheUpdate()
        {
            var recievedHospitals = new List<Hospital>();
            foreach(DistrictEnum district in service.AllDistricts)
            {
               var header = RequestHeader.CreateRequestHeader(new List<DistrictEnum>() { district });
                var data = await service.GetBedDetailsAsync(header);
                recievedHospitals.AddRange(data.Result);
                Assert.AreEqual(recievedHospitals, service.GetBedDetailsCache());
            }
        }

        [Test(Description ="Checks all hospital download data and cache data")]
        [Order(6)]
        public async Task CheckAllHospital()
        {
            var data = await service.GetBedDetailsAsync(RequestHeader.CreateRequestHeader(service.AllDistricts));
            data.Result.Sort();
            Hospital[] array = new Hospital[service.GetBedDetailsCache().Count];
            service.GetBedDetailsCache().CopyTo(array, 0);
            Array.Sort(array);
            Assert.AreEqual(array, data.Result);
        }

        [Test(Description ="Checks the file log stream is same as expected")]
        public void CheckLogStream()
        {
            var stream = service.GetLogData();
            Assert.IsTrue(stream.Name.EndsWith("api-log.txt"));
        }
    }
}