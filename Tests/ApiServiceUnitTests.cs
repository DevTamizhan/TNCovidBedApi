using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TNCovidBedApi;
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

        [Test(Description = "Checks for the NoCacheException when district cache data is empty")]
        [Order(1)]
        public void NoCacheExceptionDistrcits()
        {
            Assert.Throws(typeof(NoCacheException), () => service.GetAllDistrictsCache());
        }

        [Test(Description = "Checks for the NoCacheException when hospital cache data is empty")]
        [Order(2)]
        public void NoCacheExceptionHospitals()
        {
            Assert.Throws(typeof(NoCacheException), () => service.GetBedDetailsCache());
        }

        [Test(Description = "Checks whether districts were automatically downloaded when request header is created")]
        [Order(3)]
        public void RequestHeaderNoCacheException()
        {
            Assert.Throws<NoCacheException>(() => RequestHeader.CreateRequestHeader(service.AllDistricts));
        }


        [Test(Description = "Verifies whether 38 districts were downloaded or not")]
        [Order(4)]
        public async Task DistrictApiDownloadAndCacheUpdate()
        {
            var districtRoot = await service.GetAllDistrictsAsync();
            Assert.AreEqual(districtRoot.Result, service.GetAllDistrictsCache());
        }

        [Test(Description = "Checks for each request header and recieved hospitals with cache")]
        [Order(5)]
        public async Task HospitalApiDownloadAndCacheUpdate()
        {
            var recievedHospitals = new List<Hospital>();
            foreach (DistrictEnum district in service.AllDistricts)
            {
                var header = RequestHeader.CreateRequestHeader(new List<DistrictEnum>() { district });
                var data = await service.GetBedDetailsAsync(header);
                recievedHospitals.AddRange(data.Result);
                Assert.AreEqual(recievedHospitals, service.GetBedDetailsCache());
            }
        }

        [Test(Description = "Checks all hospital download data and cache data")]
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

        [Test(Description = "Checks the file log stream is same as expected")]
        [Order(7)]
        public void CheckLogStream()
        {
            var stream = service.GetLogData();
            Assert.IsTrue(stream.Name.EndsWith("api-log.txt"));
        }

        [Test(Description = "Checks the recent fetched district with cache data in file")]
        [Order(8)]
        public async Task CheckDistrictCacheFile()
        {
            ApiService service = new ApiService(true);
            var districtData = await service.GetAllDistrictsAsync();
            Assert.AreEqual(districtData.Result.AsReadOnly(), service.GetAllDistrictsCacheFromFile());            
        }

        [Test(Description ="Checks the file cache of hospitals with the fetched details of all hospitals")]
        [Order(9)]
        public async Task CheckHospitalCacheFile()
        {
            ApiService service = new ApiService(true);
            var districtData = await service.GetAllDistrictsAsync();
            var hospitalData = await service.GetBedDetailsAsync(RequestHeader.CreateRequestHeader(service.AllDistricts));
            hospitalData.Result.Sort();
            var cacheData = new List<Hospital>(service.GetBedDetailsCache());
            cacheData.Sort();
            Assert.AreEqual(hospitalData.Result, cacheData);
        }

        [Test(Description ="Deletes district cache file")]
        [Order(10)]
        public void DeleteDistrictCacheFile()
        {
            service.DeleteDistrictCacheFile();
            Assert.IsFalse(service.IsDistrictCacheFileAvailable);
        }
        
        [Test(Description = "Deletes hospital cache file")]
        [Order(11)]
        public void DeleteHospitalCacheFile()
        {
            service.DeleteHospitalCacheFile();
            Assert.IsFalse(service.IsHospitalCacheFileAvailable);
        }

        [Test(Description ="ApiService should not create new district cache file")]
        [Order(12)]
        public async Task NoDistrictCacheFileCreated()
        {
            ApiService service = new ApiService(false);
            await service.GetAllDistrictsAsync();
            Assert.IsFalse(service.IsDistrictCacheFileAvailable);
        }


        [Test(Description = "ApiService should not create new hospital cache file")]
        [Order(13)]
        public async Task NoHospitalCacheFileCreated()
        {
            ApiService service = new ApiService(false);
            await service.GetAllDistrictsAsync();
            await service.GetBedDetailsAsync(RequestHeader.CreateRequestHeader(new List<DistrictEnum>() { DistrictEnum.Ariyalur }));
            Assert.IsFalse(service.IsHospitalCacheFileAvailable);
        }
    }
}