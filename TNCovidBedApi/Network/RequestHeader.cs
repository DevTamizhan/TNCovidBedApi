using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TNCovidBedApi.Models;

namespace TNCovidBedApi
{
    /// <summary>
    /// The Request header to filter the hospitals fetched from the hospital API
    /// </summary>
    public class RequestHeader
    {
        /// <summary>
        /// The search string to search for any hospital
        /// </summary>
        [JsonPropertyName("searchString")]
        public string SearchString { get; private set; }

        /// <summary>
        /// Number of items in a page
        /// </summary>
        [JsonPropertyName("pageLimit")]
        public ulong PageLimit { get => ulong.MaxValue - 10000; }

        /// <summary>
        /// Sort Value to sort the result.Use HospitalSortValue to set sort value
        /// </summary>
        [JsonPropertyName("SortValue")]
        public string SortValue { get; private set; }

        /// <summary>
        /// List of district ids on which the hospital need to be searched
        /// </summary>
        [JsonPropertyName("Districts")]
        public List<string> DistrictsIDs { get; private set; }

        /// <summary>
        /// Set true to fetch government hospitals or else false
        /// </summary>
        [JsonPropertyName("IsGovernmentHospital")]
        public bool IsGovernmentHospital { get; private set; }

        /// <summary>
        /// Set true to fetch private hospitals or else false
        /// </summary>
        [JsonPropertyName("IsPrivateHospital")]
        public bool IsPrivateHospital { get; private set; }

        /// <summary>
        /// Corona hospital facility type that need to be searched
        /// <para>
        /// Set value using FacitilyType enum
        /// </para>
        /// </summary>
        [JsonPropertyName("FacilityTypes")]
        public List<string> FacilityTypes { get; private set; }

        private RequestHeader()
        {
            SetDefaultValues(this);
        }

        private static void SetDefaultValues(RequestHeader requestHeaderInstance)
        {
            requestHeaderInstance.SearchString = "";
            requestHeaderInstance.DistrictsIDs = null;
            requestHeaderInstance.IsGovernmentHospital = true;
            requestHeaderInstance.IsPrivateHospital = true;
            requestHeaderInstance.SortValue = HospitalSortValue.Alphabetically.GetHospitalSortString();
            FacilityType type = FacilityType.CHO | FacilityType.CHC | FacilityType.CCC | FacilityType.ICCC;
            requestHeaderInstance.FacilityTypes = new List<string>(type.GetFacilityTypeString());
        }

        /// <summary>
        /// Creates the RequestHeader with default values.
        /// <para>
        /// Default values cause entire data to be fetched
        /// </para>
        /// </summary>
        /// <returns>RequestHeader instance</returns>
        private static RequestHeader CreateRequestHeader()
        {
            RequestHeader requestHeaderInstance = new RequestHeader();
            SetDefaultValues(requestHeaderInstance);
            return requestHeaderInstance;
        }

        /// <summary>
        /// Creates the requst header
        /// </summary>
        /// <param name="districts">The districts that need to be filtered</param>
        /// <returns>RequestHeaderRequest header fetches all kinds of hospitals under specified districts</returns>
        public static RequestHeader CreateRequestHeader(List<DistrictEnum> districts)
        {
            RequestHeader requestHeaderInstance = CreateRequestHeader();
            requestHeaderInstance.DistrictsIDs = districts.GetDistrictIds();
            return requestHeaderInstance;
        }

        /// <summary>
        /// Creates the RequestHeader
        /// </summary>
        /// <param name="searchString">Normal search string for filering hospitals and do not use regular expression</param>
        /// <param name="districts">The districts that need to be filtered</param>
        /// <returns>RequestHeaderRequest header fetches all kinds of hospitals under specified districts that matching the search format</returns>
        public static RequestHeader CreateRequestHeader(string searchString, List<DistrictEnum> districts)
        {
            RequestHeader requestHeaderInstance = CreateRequestHeader(districts);
            requestHeaderInstance.SearchString = searchString;
            return requestHeaderInstance;
        }

        /// <summary>
        /// Creates the RequestHeader
        /// </summary>
        /// <param name="districts">The districts that need to be filtered</param>
        /// <param name="type">FaciltyType need to fetched
        /// <para>
        /// Default : CHO, CHC, CCC, ICCC if null
        /// </para>
        /// </param>
        /// <param name="isGovernmentHospital">Specifies government hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <param name="isPrivateHospital">
        /// Specifies private hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <returns>RequestHeader with all the filters</returns>
        public static RequestHeader CreateRequestHeader(List<DistrictEnum> districts, FacilityType type, bool isGovernmentHospital = true, bool isPrivateHospital = true)
        {
            if (type == FacilityType.All)
                type = FacilityType.CCC | FacilityType.CHC | FacilityType.CHO | FacilityType.ICCC;
            RequestHeader requestHeaderInstance = CreateRequestHeader(districts);
            requestHeaderInstance.FacilityTypes = new List<string>(type.GetFacilityTypeString());
            requestHeaderInstance.IsGovernmentHospital = isGovernmentHospital;
            requestHeaderInstance.IsPrivateHospital = isPrivateHospital;
            return requestHeaderInstance;
        }

        /// <summary>
        /// Creates the RequestHeader
        /// </summary>
        /// <param name="districts">The districts that need to be filtered</param>
        /// <param name="type">FaciltyType need to fetched
        /// <para>
        /// Default : CHO, CHC, CCC, ICCC
        /// </para>
        /// </param>
        /// <param name="sortValue">Sorting order of the hospital result
        /// <para>
        /// By default sort alphabetically if null
        /// </para>
        /// </param>
        /// <param name="isGovernmentHospital">Specifies government hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <param name="isPrivateHospital">
        /// Specifies private hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <returns>RequestHeader with all the filters</returns>
        public static RequestHeader CreateRequestHeader(List<DistrictEnum> districts, FacilityType type, HospitalSortValue sortValue = HospitalSortValue.Alphabetically, bool isGovernmentHospital = true, bool isPrivateHospital = true)
        {
            RequestHeader requestHeaderInstance = CreateRequestHeader(districts, type, isGovernmentHospital, isPrivateHospital);
            requestHeaderInstance.SortValue = sortValue.GetHospitalSortString();
            return requestHeaderInstance;
        }

        /// <summary>
        /// Creates the RequestHeader
        /// </summary>
        /// <param name="searchString">Normal search string for filering hospitals and do not use regular expression</param>
        /// <param name="districts">The districts that need to be filtered</param>
        /// <param name="type">FaciltyType need to fetched
        /// <para>
        /// Default : CHO, CHC, CCC, ICCC
        /// </para>
        /// </param>
        /// <param name="sortValue">Sorting order of the hospital result
        /// <para>
        /// By default sort alphabetically
        /// </para>
        /// </param>
        /// <param name="isGovernmentHospital">Specifies government hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <param name="isPrivateHospital">
        /// Specifies private hospitals need to be fetched or not.
        /// <para>Default : true</para>
        /// </param>
        /// <returns>RequestHeader with all the filters</returns>
        public static RequestHeader CreateRequestHeader(string searchString, List<DistrictEnum> districts, FacilityType type, HospitalSortValue sortValue = HospitalSortValue.Alphabetically, bool isGovernmentHospital = true, bool isPrivateHospital = true)
        {
            RequestHeader requestHeaderInstance = CreateRequestHeader(districts, type, sortValue, isGovernmentHospital, isPrivateHospital);
            requestHeaderInstance.SearchString = searchString;
            return requestHeaderInstance;
        }

        /// <summary>
        /// Converts the current object to it's JSON string
        /// </summary>
        /// <returns>JSON string</returns>
        public string ToJSONString()
        {
            return JsonSerializer.Serialize<RequestHeader>(this, null);
        }

        /// <summary>
        /// Converts the current object to it's JSON string
        /// </summary>
        /// <returns>JSON string</returns>
        public override string ToString()
        {
            return ToJSONString();
        }

        public override bool Equals(object obj)
        {
            return obj is RequestHeader header &&
                   SearchString == header.SearchString &&
                   PageLimit == header.PageLimit &&
                   SortValue == header.SortValue &&
                   EqualityComparer<List<string>>.Default.Equals(DistrictsIDs, header.DistrictsIDs) &&
                   IsGovernmentHospital == header.IsGovernmentHospital &&
                   IsPrivateHospital == header.IsPrivateHospital &&
                   EqualityComparer<List<string>>.Default.Equals(FacilityTypes, header.FacilityTypes);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SearchString, PageLimit, SortValue, DistrictsIDs, IsGovernmentHospital, IsPrivateHospital, FacilityTypes);
        }
    }
}