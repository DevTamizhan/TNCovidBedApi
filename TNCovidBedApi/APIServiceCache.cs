using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TNCovidBedApi.Models;

namespace TNCovidBedApi
{
    public partial class ApiService
    {
        public bool IsDistrictCacheFileAvailable { get => File.Exists(Path.Combine(Environment.CurrentDirectory, "districtCache.json")); }

        public bool IsHospitalCacheFileAvailable { get => File.Exists(Path.Combine(Environment.CurrentDirectory, "hospitalCache.json")); }

        #region Hospital

        /// <summary>
        /// Gets all the district objects from the cached data that are nearby a location.
        /// <para>
        /// May or may not all hospitals nearby will be feteched. So, call GetBedDetailsAsync() and
        /// make the cache to be with updated data
        /// </para>
        /// </summary>
        /// <returns>ReadOnlyCollection of all the districts</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">Cache data is not available</exception>
        /// <param name="distance">Radius to check from the a specific location</param>
        /// <param name="location">Location from which need to search the hospitals</param>
        public ReadOnlyCollection<Hospital> FindHospitalsNearby(PointF location, float distance)
        {
            var completeRequestHeader = RequestHeader.CreateRequestHeader("", AllDistricts, AllFacilityType, HospitalSortValue.Alphabetically, true, true);
            if (fullHospitalCacheLength != cacheManager.GetCachedHospitals().Count)
            {
                Task.Run(() => this.GetBedDetailsAsync(completeRequestHeader)).Wait();
            }

            if (previousFetchLocation.X == -1 || previousFetchLocation != location || previousDistance != distance)
            {
                AddHospitalsToList(location, distance);
                previousDistance = distance;
                previousFetchLocation = location;
            }
            return hospitalsInLocation.AsReadOnly();
        }

        /// <summary>
        /// Gets all the bed detail of each hospital from the cached data
        /// <para>May or may not contain all hospital details</para>
        /// </summary>
        /// <returns>ReadOnlyCollection of all the hospitals in cache</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">Cache data is not available</exception>
        public ReadOnlyCollection<Hospital> GetBedDetailsCache()
        {
            var hospitals = cacheManager.GetCachedHospitals();
            if (hospitals is null || hospitals.Count == 0)
            {
                logger.Error("The hospital cache is empty");
                throw new NoCacheException("Hospital cache is not available");
            }
            return hospitals;
        }

        /// <summary>
        /// Gets all the bed detail of each hospital from the cached data that matches the Request header
        /// /// <para>May or may not contain all hospital details</para>
        /// </summary>
        /// <param name="header">Thr request header to filter data fro cache</param>
        /// <returns>ReadOnlyCollection of all the hospitals in cache</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">Cache data is not available</exception>
        public ReadOnlyCollection<Hospital> GetBedDetailsCache(RequestHeader header)
        {
            var hospitals = new List<Hospital>(GetBedDetailsCache());
            var filtered = from hospital in hospitals
                           where FilterBedCache(header, hospital, BedStatus.Anything, BedStatus.Anything, BedStatus.NotAvailable)
                           select hospital;
            logger.Info($"Filtered bed details based from cache for the request header {header.ToString()} and returning {filtered.Count()} hospitals");
            return filtered.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets all the bed detail of each hospital from the cached data that matches the Request header as well as with the bed status
        /// /// <para>May or may not contain all hospital details</para>
        /// </summary>
        /// <param name="header">Thr request header to filter data fro cache</param>
        /// <param name="icuAvailability">Filter ICU beds based on BedStatus</param>
        /// <param name="normalBedAvailability">Filter Normal beds based on BedStatus</param>
        /// <param name="o2Availability">Filters Oxygen supported beds based on BedStatus</param>
        /// <returns>ReadOnlyCollection of all the hospitals in cache</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">Cache data is not available</exception>
        public ReadOnlyCollection<Hospital> GetBedDetailsCache(RequestHeader header, BedStatus o2Availability, BedStatus icuAvailability, BedStatus normalBedAvailability)
        {
            var hospitals = new List<Hospital>(GetBedDetailsCache());
            var filtered = from hospital in hospitals
                           where FilterBedCache(header, hospital, o2Availability, icuAvailability, normalBedAvailability)
                           select hospital;
            logger.Info($"Filtered bed details based from cache for the request header {header.ToString()} and returning {filtered.Count()} hospitals");
            return filtered.ToList().AsReadOnly();
        }

        public ReadOnlyCollection<Hospital> GetBedDetailsCacheFromFile()
        {
            var cachedHospitals = cacheManager.GetCachedHospitalsFromFile();
            if(cachedHospitals.Count == 0)
            {
                logger.Error("The hospital cache is empty");
                throw new NoCacheException("Hospital cache is not available");
            }
            return cachedHospitals;
                
        }

        /// <summary>
        /// Adds the hospitals which match the search criteria
        /// </summary>
        /// <param name="location">GPS co-ordinate of the location</param>
        /// <param name="distance">Raduis within which hospitals need to be searched</param>
        private void AddHospitalsToList(PointF location, float distance)
        {
            hospitalsInLocation.Clear();
            foreach (Hospital hospital in cacheManager.GetCachedHospitals())
            {
                if (hospital.Latitude is null || hospital.Longitude is null || GeoCoordinate.FindDistance(location, new PointF(hospital.Latitude.Value, hospital.Longitude.Value)) > distance)
                {
                    continue;
                }
                hospitalsInLocation.Add(hospital);
            }
        }

        private bool FilterBedCache(RequestHeader header, Hospital hospital, BedStatus o2Availability, BedStatus icuAvailability, BedStatus normalBedAvailability)
        {
            bool bedTypeMatch = IsBedsAvailable(hospital, o2Availability.BetStatusToBool(), icuAvailability.BetStatusToBool(), normalBedAvailability.BetStatusToBool());
            bool searchStringMatch = Regex.Match(hospital.Name, header.SearchString).Success;
            var retValue = bedTypeMatch && searchStringMatch && header.DistrictsIDs.Contains(hospital.District.ID) && header.FacilityTypes.Contains(hospital.FacilityType);
            return retValue;
        }

        private bool IsBedsAvailable(Hospital hospital, bool? o2Availability, bool? icuAvailability, bool? normalBedAvailability)
        {
            bool o2Available = hospital.CovidBedDetails.OccupancyO2Beds > 0;
            bool icuAvailable = hospital.CovidBedDetails.OccupancyICUBeds > 0;
            bool normalAvailable = hospital.CovidBedDetails.OccupancyNonO2Beds > 0;
            return (o2Available == (o2Availability ?? o2Available) && icuAvailable == (icuAvailability ?? icuAvailable) && normalAvailable == (normalBedAvailability ?? normalAvailable));
        }

        public bool DeleteHospitalCacheFile()
        {
            return cacheManager.DeleteHospitalFileCache();
        }
        #endregion Hospital

        #region District

        /// <summary>
        /// Gets all the district objects from the cached data
        /// </summary>
        /// <returns>ReadOnlyCollection of all the districts</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">Cache data is not available</exception>
        public ReadOnlyCollection<District> GetAllDistrictsCache()
        {
            var cachedDistricts = cacheManager.GetCachedDistricts();
            if (cachedDistricts is null || cachedDistricts.Count == 0)
            {
                logger.Error("The district cache is empty");
                throw new NoCacheException("District cache is not available");
            }
            return cachedDistricts;
        }

        public ReadOnlyCollection<District> GetAllDistrictsCacheFromFile()
        {
            var cachedDistricts = cacheManager.GetCachedDistrictsFromFile();
            if (cachedDistricts.Count == 0)
            {
                logger.Error("No district cache from file is available");
                throw new NoCacheException("District cache is not available");
            }
            return cachedDistricts;
        }

        public bool DeleteDistrictCacheFile()
        {
            return cacheManager.DeleteDistrictFileCache();
        }

        #endregion District


    }
}