using System;
using System.Collections.Generic;
using System.Linq;
using TNCovidBedApi.Cache;
using TNCovidBedApi.Models;

namespace TNCovidBedApi
{
    /// <summary>
    /// Extension class to manipulate Enum data that were specific to API
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Converts the DistrictEnum to respective district IDS
        /// </summary>
        /// <param name="districts">List of DistrictEnum</param>
        /// <returns>List of ID of each district</returns>
        /// <exception cref="TNCovidBedApi.NoCacheException">
        /// Download district data first and then execute to avoid exception
        /// </exception>
        public static List<string> GetDistrictIds(this List<DistrictEnum> districts)
        {
            List<string> ids = new List<string>();
            var districtList = new List<District>(ApiCacheManager.CreateCacheManager().GetCachedDistricts());
            if (districtList.Count == 0)
            {
                districtList = CreateDistrictCache();
            }
            for (int i = 0; i < districtList.Count; i++)
            {
                foreach (var district in districts)
                {
                    if (district.ToString() == districtList[i].Name)
                    {
                        ids.Add(districtList[i].ID);
                        districtList.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// Converts the FacilityType enum to the string array to send in RequestHeader
        /// </summary>
        /// <param name="type">FacilityType enum value</param>
        /// <returns>String array of each facility type</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Invalid facility type is passed
        /// </exception>
        public static string[] GetFacilityTypeString(this FacilityType type)
        {
            if (Int32.TryParse(type + "", out _))
                throw new ArgumentOutOfRangeException("Invalid facility type");
            var enumString = type.ToString().Split(",").ToList();
            for (int i = 0; i < enumString.Count; i++)
            {
                enumString[i] = enumString[i].Trim();
            }
            return enumString.ToArray();
        }

        /// <summary>
        /// Converts the HospitalSortValue to string that could be sent in RequestHeader
        /// </summary>
        /// <param name="value">The HospitalSortValue for sorting the result</param>
        /// <returns>String representation of HospitalSortValue</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Invalid sort value is passed
        /// </exception>
        public static string GetHospitalSortString(this HospitalSortValue value)
        {
            switch (value)
            {
                case HospitalSortValue.Alphabetically:
                    return "Alphabetically";

                case HospitalSortValue.ICU_Availability:
                    return "ICU Availability";

                case HospitalSortValue.Normal_Bed_Availability:
                    return "Normal Bed Availability";

                case HospitalSortValue.Oxygen_Bed_Availability:
                    return "O2 Bed Availability";

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid value is passed");
            }
        }
        private static List<District> CreateDistrictCache()
        {
            var currentFetch = Network.ApiNetworkManager.CreateAPINetworkManager().GetAllDistrictsAsync().GetAwaiter().GetResult();
            Cache.ApiCacheManager.CreateCacheManager().UpdateDistrictCache(currentFetch.Result);
            return currentFetch.Result;
        }

        public static bool? BetStatusToBool(this BedStatus status)
        {
            switch(status)
            {
                case BedStatus.Available:
                    return true;
                case BedStatus.NotAvailable:
                    return false;
                default:
                    return null;
            }
        }
    }
}