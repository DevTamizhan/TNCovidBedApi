using System;

namespace TNCovidBedApi
{
    public class DownloadProgress
    {
        public float PercentageDownloaded { get; private set; }
        public long TotalBytes {  get; private set; }
        public long DownloadedBytes {  get; private set; }
        public DownloadProgress(long totalBytes, long DownloadedBytes)
        {
            this.TotalBytes = totalBytes;
            this.DownloadedBytes = DownloadedBytes;
            PercentageDownloaded = (DownloadedBytes / TotalBytes) * 100f;
        }
    }
}
