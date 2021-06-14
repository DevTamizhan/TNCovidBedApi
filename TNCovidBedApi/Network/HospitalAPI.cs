using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Network
{
    internal class HospitalAPI
    {
        private readonly string ACCESS_URL = @"https://tncovidbeds.tnega.org/api/hospitals";
        private readonly HttpClient httpClient;
        private readonly NLog.ILogger logger;

        public HospitalAPI()
        {
            logger = ApiLogger.GetLogger();
            httpClient = new HttpClient();
        }
        /// <summary>
        /// Downloads the hospital details asynchronously
        /// </summary>
        /// <param name="header">The RequestHeader stating the filter values of hospital details to be downloaded</param>
        /// <returns>The RootBed object</returns>
        /// ///<exception cref="TNCovidBedApi.DataDownloadException">
        ///Thrown when Invalid request URI or network error
        ///</exception>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public async Task<RootBed> DownloadHospitalDataAsync(RequestHeader header)
        {
            try
            {
                HttpContent content = new StringContent(header.ToJSONString(), null, "application/json");
                HttpResponseMessage responseContent = await httpClient.PostAsync(ACCESS_URL, content);
                logger.Info("Hospital data is downloaded from the HospitalAPI");
                var recieved = await responseContent.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RootBed>(recieved, null);
            }
            catch (InvalidOperationException e)
            {
                logger.Error($"Hospital data cannot be downloaded due to invalid API Call {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (HttpRequestException e)
            {
                logger.Error($"Hospital data cannot be downloaded due to invalid API Call {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (TaskCanceledException e)
            {
                logger.Error($"Hospital data cannot be downloaded due to network time out {e.Message}");
                throw new DataDownloadException("District data cannot be downloaded", e);
            }
            catch (Exception)
            {
                logger.Error("Hospital data is downloaded but cannot be parsed");
                throw;
            }
        }
    }
}