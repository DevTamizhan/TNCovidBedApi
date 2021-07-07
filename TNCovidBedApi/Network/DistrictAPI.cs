using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Network
{
    ///<summary>
    /// Processes the district data fetching storing it as usable RootDistrict object in memory
    ///</summary>
    internal class DistrictAPI
    {
        private const string ACCESS_URL = @"https://tncovidbeds.tnega.org/api/district";
        private readonly HttpClient httpClient;
        private readonly NLog.ILogger logger;
        public DistrictAPI()
        {
            logger = ApiLogger.GetLogger();
            httpClient = new HttpClient();
        }
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
        public async Task<RootDistrict> DownloadDistrictAsync()
        {
            HttpResponseMessage responseContent;
            try
            {
                responseContent = await httpClient.GetAsync(ACCESS_URL);
                logger.Info("District data is downloaded from the DistrictAPI");
                string responseString = await responseContent.Content.ReadAsStringAsync();
                return RootDistrict.ParseJSON(responseString);
            }
            catch (InvalidOperationException e)
            {
                logger.Error($"District data cannot be downloaded due to invalid API Call {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (HttpRequestException e)
            {
                logger.Error($"District data cannot be downloaded due to invalid API Call {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (TaskCanceledException e)
            {
                logger.Error($"District data cannot be downloaded due to network time out {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (Exception)
            {
                logger.Error("District data is downloaded but cannot be parsed");
                throw;
            }
        }
    }
}