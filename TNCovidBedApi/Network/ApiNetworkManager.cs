using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Network
{
    internal class ApiNetworkManager
    {
        private static ApiNetworkManager managerReference = null;

        private readonly DistrictAPI districtAPI;

        private readonly HospitalAPI hospitalAPI;
        private readonly NLog.ILogger logger;

        private ApiNetworkManager()
        {
            logger = ApiLogger.GetLogger();
            districtAPI = new DistrictAPI();
            hospitalAPI = new HospitalAPI();
        }

        ///<summary>
        ///Create an object of APINetworkManager to use
        ///</summary>
        ///<returns>
        ///An object of APINetworkManager
        ///</returns>
        public static ApiNetworkManager CreateAPINetworkManager()
        {
            if (managerReference is null)
                managerReference = new ApiNetworkManager();
            return managerReference;
        }

        ///<summary>
        ///Downloads the contents from District API asynchronously
        ///</summary>
        ///<returns>
        ///RootDistrict object wrapped by Task
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
        public async Task<RootDistrict> GetAllDistrictsAsync(CancellationToken token, IProgress<DownloadProgress> progress = null)
        {
            try
            {
                var rootDistrict = await districtAPI.DownloadDistrictAsync(token, progress);
                logger.Info("District data is downloaded");
                return rootDistrict;
            }
            catch (DataDownloadException e)
            {
                logger.Error($"District data cannot be downloaded due to {e.Message}");
                throw;
            }
            catch (System.Text.Json.JsonException e)
            {
                logger.Error($"District data downloaded but cannot be parsed due to {e.Message}");
                throw;
            }
            catch (System.NotSupportedException e)
            {
                logger.Error($"District data downloaded but cannot be parsed due to {e.Message}");
                throw;
            }
            catch(Exception e)
            {
                logger.Error($"Some error due to {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Download the hospitals from Hospital API
        /// </summary>
        /// <param name="header">RequestHeader to filter the hospital details that need to be fetched</param>
        /// <returns>RootBed Object</returns>
        /// ///<exception cref="TNCovidBedApi.DataDownloadException">
        ///Thrown when Invalid request URI or network error
        ///</exception>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public async Task<RootBed> GetBedDetailsAsync(RequestHeader header, CancellationToken token, IProgress<DownloadProgress> progress = null)
        {
            try
            {
                var rootBed = await hospitalAPI.DownloadHospitalDataAsync(header, token, progress);
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.Append("Hospital data successfully downloaded for districts ");
                var districtName = new List<District>(Cache.ApiCacheManager.GetCacheManagerInstance.GetCachedDistricts());
                foreach (var item in header.DistrictsIDs)
                {
                    int index = districtName.FindIndex((district) => district.ID == item);
                    builder.Append(index>=0? districtName[index].Name + " " : "");
                }
                logger.Info(builder.ToString());
                return rootBed;
            }
            catch (DataDownloadException e)
            {
                logger.Error($"Hospital data cannot be downloaded due to {e.Message}");
                throw;
            }
            catch (System.Text.Json.JsonException e)
            {
                logger.Error($"Hospital data downloaded but cannot be parsed due to {e.Message}");
                throw;
            }
            catch (System.NotSupportedException e)
            {
                logger.Error($"Hospital data downloaded but cannot be parsed due to {e.Message}");
                throw;
            }
        }
    }
}