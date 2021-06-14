using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLog;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Cache
{
    internal class ApiCacheManager
    {
        private static ApiCacheManager cacheManagerInstance;
        private readonly DistrictCache districtCache;
        private readonly HospitalCache hospitalCache;
        private readonly ILogger logger;
        private ApiCacheManager()
        {
            logger = ApiLogger.GetLogger();
            districtCache = DistrictCache.CreateDistrictCache();
            hospitalCache = HospitalCache.CreateHospitalCache();
        }

        /// <summary>
        /// Creates the instance of APICacheManager.
        /// <para>
        /// This keeps only one instance of cache in memory and wherever we create cache manager all
        /// cache will be same
        /// </para>
        /// </summary>
        /// <returns>The instance of APCacheManager</returns>
        public static ApiCacheManager CreateCacheManager()
        {
            if (cacheManagerInstance is null)
                cacheManagerInstance = new ApiCacheManager();
            return cacheManagerInstance;
        }

        /// <summary>
        /// Gets the cached district data
        /// </summary>
        /// <returns>ReadOnlyCollection of districts from cache</returns>
        public ReadOnlyCollection<District> GetCachedDistricts()
        {
            var allDistricts = districtCache.AllDistricts;
            logger.Info($"Districts data fetched from cache and count of district is {allDistricts.Count}");
            return allDistricts;
        }

        /// <summary>
        /// Gets all the cached hospital details
        /// </summary>
        /// <returns>
        /// ReadOnlyCollection of hospitals that are cached
        /// <para>May or may not have entire hospital details.</para>
        /// </returns>
        public ReadOnlyCollection<Hospital> GetCachedHospitals()
        {
            var allHospitals = hospitalCache.AllHospitals;
            logger.Info($"Hospitals data fetched from cache and count of district is {allHospitals.Count}");
            return allHospitals;
        }

        /// <summary>
        /// Updates the district cache data with new data
        /// </summary>
        /// <param name="districts">List of districts fetched from API</param>
        public void UpdateDistrictCache(List<District> districts)
        {
            logger.Info($"Updating district cache with the new data of count {districts.Count}");
            districtCache.UpdateCache(DateTime.Now, districts);
        }
        /// <summary>
        /// Adds the list of hospitals that were fetched from API
        /// </summary>
        /// <param name="hospitals">
        /// List of hospitals from API.
        /// <para>List may not may not contain entire hospital details</para>
        /// </param>
        public void UpdateHospitalCache(List<Hospital> hospitals)
        {
            logger.Info($"Updating hospital cache with the new data of count {hospitals.Count}");
            hospitalCache.UpdateCache(DateTime.Now, hospitals);
        }
    }
}