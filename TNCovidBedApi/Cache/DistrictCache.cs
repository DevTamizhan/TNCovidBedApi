using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Cache
{
    internal class DistrictCache
    {
        private static DistrictCache cacheInstance;

        private readonly object lockObject = new object();

        private List<District> allDistricts;
        private readonly NLog.ILogger logger;
        private DistrictCache()
        {
            logger = ApiLogger.GetLogger();
            allDistricts = new List<District>();
        }

        /// <summary>
        /// The district cache data
        /// </summary>
        public ReadOnlyCollection<District> AllDistricts
        {
            get => allDistricts.AsReadOnly();
            private set => allDistricts = new List<District>(value);
        }

        public DateTime LastUpdated { get; private set; }
        /// <summary>
        /// Create the separate cache storage for storing district details
        /// </summary>
        /// <returns>
        /// The instance of DistrictCache.
        /// <para>
        /// All the DistrictCache instance will be same and updating at one location will reflect at other
        /// </para>
        /// </returns>
        public static DistrictCache CreateDistrictCache()
        {
            if (cacheInstance is null)
                cacheInstance = new DistrictCache();
            return cacheInstance;
        }

        /// <summary>
        /// Updates the old cache data with new data
        /// </summary>
        /// <param name="updateTime">Time at which the update is initiated</param>
        /// <param name="districts">The list of districts.</param>
        public void UpdateCache(DateTime updateTime, List<District> districts)
        {
            this.LastUpdated = updateTime;
            //Old details will be replaced with new detail and new details will be added directly
            lock(lockObject)
            {
                for (int i = 0; i < districts.Count; i++)
                {
                    int position = allDistricts.FindIndex((h) => h.ID == districts[i].ID);
                    if (position == -1)
                        allDistricts.Add(districts[i]);
                    else
                        allDistricts[position] = districts[i];
                }
            }
            logger.Info($"District cache update successful and updated {districts.Count} districts");
        }
    }
}