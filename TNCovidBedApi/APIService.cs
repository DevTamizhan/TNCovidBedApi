using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using TNCovidBedApi.Cache;
using TNCovidBedApi.Models;
using TNCovidBedApi.Network;

namespace TNCovidBedApi
{
    /// <summary>
    /// The service class to access all the operations both on District API and Bed Detail API of TN government.
    /// </summary>
    public partial class ApiService
    {
        public readonly FacilityType AllFacilityType = FacilityType.CHO | FacilityType.CHC | FacilityType.CCC | FacilityType.ICCC;
        private readonly List<DistrictEnum> allDistricts;
        private readonly ApiCacheManager cacheManager;
        private readonly List<Hospital> hospitalsInLocation;
        private readonly ILogger logger;
        private readonly ApiNetworkManager networkManager;
        private readonly Timer updateScheduler;
        private int fullHospitalCacheLength = -1;
        private float previousDistance = -1;
        private PointF previousFetchLocation;

        public ApiService()
        {
            logger = ApiLogger.GetLogger();
            logger.Info("API initialized");
            allDistricts = new List<DistrictEnum>((DistrictEnum[])Enum.GetValues(typeof(DistrictEnum)));
            networkManager = ApiNetworkManager.CreateAPINetworkManager();
            cacheManager = ApiCacheManager.CreateCacheManager();
            updateScheduler = new Timer(1800000) { AutoReset = true };
            updateScheduler.Elapsed += OnTimeElapsedEvent;
            previousFetchLocation = new PointF(-1, -1);
            hospitalsInLocation = new List<Hospital>();
        }

        public event EventHandler<ApiExecutedEventArgs> APIExecuted;

        public List<DistrictEnum> AllDistricts { get => allDistricts; }

        ///<summary>
        ///Downloads the contents from District API asynchronously
        ///</summary>
        ///<returns>
        ///RootDistrict object
        ///</returns>
        ///<exception cref="TNCovidBedApi.DataDownloadException">
        ///Thrown when Invalid request URI or network error
        ///</exception>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public async Task<RootDistrict> GetAllDistrictsAsync()
        {
            var _rootDistrict = await networkManager.GetAllDistrictsAsync();
            ApiExecutedEventArgs args = new ApiExecutedEventArgs(_rootDistrict, typeof(RootDistrict));
            OnAPIExecuted(args);
            cacheManager.UpdateDistrictCache(_rootDistrict.Result);
            return _rootDistrict;
        }

        ///<summary>
        ///Downloads the contents from Hospital Bed details API asynchronously
        ///</summary>
        ///<param name="header">The request header to download specific data</param>
        ///<returns>
        ///RootBed object
        ///</returns>
        ///<exception cref="TNCovidBedApi.DataDownloadException">
        ///Thrown when Invalid request URI or network error
        ///</exception>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public async Task<RootBed> GetBedDetailsAsync(RequestHeader header)
        {
            var rootBed = await GetBedDetailsInternalAsync(header);
            ApiExecutedEventArgs args = new ApiExecutedEventArgs(rootBed, typeof(RootBed));
            OnAPIExecuted(args);
            return rootBed;
        }

        ///<summary>
        ///Downloads the contents from Hospital Bed details API asynchronously
        ///</summary>
        ///<returns>
        ///RootBed object
        ///</returns>
        ///<param name="header">The request header to download specific data</param>
        /// <param name="ICUAvailable">Filter ICU beds based on BedStatus</param>
        /// <param name="normalBedAvailable">Filter Normal beds based on BedStatus</param>
        /// <param name="oxygenBedAvailable">Filters Oxygen supported beds based on BedStatus</param>
        ///<exception cref="TNCovidBedApi.DataDownloadException">
        ///Thrown when Invalid request URI or network error
        ///</exception>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public async Task<RootBed> GetBedDetailsAsync(RequestHeader header, BedStatus oxygenBedAvailable, BedStatus ICUAvailable, BedStatus normalBedAvailable)
        {
            var rootBed = await GetBedDetailsInternalAsync(header);
            var hospitals = rootBed.Result;
            for (int i = 0; i < hospitals.Count; i++)
            {
                if (!IsBedsAvailable(hospitals[i], oxygenBedAvailable.BetStatusToBool(), ICUAvailable.BetStatusToBool(), normalBedAvailable.BetStatusToBool()))
                {
                    hospitals.RemoveAt(i);
                    i--;
                }
            }
            logger.Info($"Filtered Hospital bed details downloaded for the request header {header.ToJSONString()} and returning {rootBed.Result.Count} hospitals");
            return rootBed;
        }

        /// <summary>
        /// Gets the FileStream of log data
        /// </summary>
        /// <returns>FileStream object with only access to read</returns>
        public FileStream GetLogData()
        {
            return new FileStream($"{Directory.GetCurrentDirectory()}/api-log.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        private async Task<RootBed> GetBedDetailsInternalAsync(RequestHeader header)
        {
            var rootBed = await networkManager.GetBedDetailsAsync(header);
            if (!updateScheduler.Enabled)
            {
                updateScheduler.Enabled = true;
            }

            updateScheduler.Interval = 1800000;
            cacheManager.UpdateHospitalCache(rootBed.Result);
            if(header.Equals(RequestHeader.CreateRequestHeader("",AllDistricts,AllFacilityType,HospitalSortValue.Alphabetically,true,true)))
            {
                fullHospitalCacheLength = cacheManager.GetCachedHospitals().Count;
            }
            return rootBed;
        }

        private void OnAPIExecuted(ApiExecutedEventArgs e)
        {
            APIExecuted?.Invoke(this, e);
        }

        /// <summary>
        ///Invoked every 30 minutes after the final cache updated
        /// </summary>
        private void OnTimeElapsedEvent(object sender, ElapsedEventArgs e)
        {
            var completeRequestHeader = RequestHeader.CreateRequestHeader("", AllDistricts, AllFacilityType, HospitalSortValue.Alphabetically, true, true);
            logger.Info("Initializing the re-download for Hospital data to fetch all data");
            Task.Run(() => this.GetBedDetailsAsync(completeRequestHeader)).Wait();
        }
    }
}