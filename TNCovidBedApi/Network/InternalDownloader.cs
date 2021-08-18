using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using NLog;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;

namespace TNCovidBedApi.Network
{
    internal class InternalDownloader : IDisposable
    {
        private HttpClient _httpClient;
        private string _url;
        private HttpContent httpContent;
        private bool disposedValue;
        private ILogger logger;
        readonly JsonSerializerOptions options = new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };

        public InternalDownloader(string URL)
        {
            _url = URL;
            httpContent = null;
            _httpClient = new HttpClient();
            logger = ApiLogger.GetLogger();
        }

        public InternalDownloader(string URL, HttpContent content)
        {
            _url = URL;
            httpContent = content;
            _httpClient = new HttpClient();
            logger = ApiLogger.GetLogger();
        }

        private async Task<string> ReadFromStream(HttpResponseMessage response, IProgress<DownloadProgress> progress, CancellationToken token)
        {
            List<byte> totalBuffer = new List<byte>();
            long? totalBytes = response.Content.Headers.ContentLength;
            int buffersize = 65535;
            byte[] buffer = new byte[buffersize];
            long readSize = 0;
            long totalRecieved = 0;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                do
                {
                    Array.Clear(buffer, 0, buffersize);
                    readSize = await responseStream.ReadAsync(buffer, 0, buffersize);
                    foreach(byte b in buffer)
                    {
                        if (b != 0x00)
                            totalBuffer.Add(b);
                    }
                    totalRecieved += readSize;
                    progress?.Report(new DownloadProgress(totalBytes ?? 0, totalRecieved));
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                } while (readSize != 0 && !token.IsCancellationRequested);
            }
            string data = System.Text.Encoding.UTF8.GetString(totalBuffer.ToArray(), 0, totalBuffer.Count);
            data = data.Trim();
            return data;
        }

        public async Task<string> DownloadDataAsync(IProgress<DownloadProgress> progress, CancellationToken token)
        {
                HttpResponseMessage response =await _httpClient.GetAsync(_url,HttpCompletionOption.ResponseHeadersRead,token);
                if (!response.IsSuccessStatusCode)
                {
                    logger.Error($"Data cannot be downloaded for request {_url}");
                    throw new DataDownloadException("Request failed");
                }
                return await ReadFromStream(response,progress, token);
        }

        public async Task<string> PostDataAsync(IProgress<DownloadProgress> progress, CancellationToken token)
        {
            if (httpContent == null)
                httpContent = new StringContent("");
            HttpResponseMessage response =await _httpClient.PostAsync(_url, httpContent, token);
            if (!response.IsSuccessStatusCode)
            {
                logger.Error($"Data cannot be downloaded for request {_url}");
                throw new DataDownloadException("Request failed");
            }
            return await ReadFromStream(response, progress, token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    httpContent = null;
                }
                _httpClient.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~InternalDownloader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
