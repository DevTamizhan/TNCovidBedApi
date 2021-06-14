using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Cache
{
    internal class HospitalCache
    {
        private static HospitalCache cacheInstance;
        private readonly object lockObject = new object();
        private List<Hospital> allHospitals;
        private readonly NLog.ILogger logger;
        private HospitalCache()
        {
            logger = ApiLogger.GetLogger();
            allHospitals = new List<Hospital>();
        }

        /// <summary>
        /// The readonly collection of entire cached hospitals
        /// <para>
        /// WARNING : May or may not contain entire hospital details. This is because the previous
        /// request executed while fetching the hospitals
        /// </para>
        /// </summary>
        public ReadOnlyCollection<Hospital> AllHospitals
        {
            get => allHospitals.AsReadOnly();
            private set => allHospitals = new List<Hospital>(value);
        }

        public DateTime LastUpdated { get; private set; }
        /// <summary>
        /// Create the separate cache storage for storing hospital details
        /// </summary>
        /// <returns>
        /// The instance of HospitalCache
        /// <para>
        /// All the HospitalCache instance will be same and updating at one location will reflect at other
        /// </para>
        /// </returns>
        public static HospitalCache CreateHospitalCache()
        {
            if (cacheInstance is null)
                cacheInstance = new HospitalCache();
            return cacheInstance;
        }

        /// <summary>
        /// </summary>
        /// <param name="updateTime">Time at which the update is initiated</param>
        /// <param name="hospitals">The list of all hospitals that need to be cached</param>
        public void UpdateCache(DateTime updateTime, List<Hospital> hospitals)
        {
            this.LastUpdated = updateTime;
            //Old details will be replaced with new detail and new details will be added directly
            lock (lockObject)
            {
                for (int i = 0; i < hospitals.Count; i++)
                {
                    int position = allHospitals.FindIndex((h) => h.ID == hospitals[i].ID);
                    if (position == -1)
                        allHospitals.Add(hospitals[i]);
                    else
                        allHospitals[position] = hospitals[i];
                }
                logger.Info($"Hospital cache update successful and updated {hospitals.Count} hospitals");
            }
        }
    }
}