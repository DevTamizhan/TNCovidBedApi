using System;

namespace TNCovidBedApi
{
    /// <summary>
    /// Exception when cache data file cannot be retrieved even file is available
    /// </summary>
    [Serializable]
    class FileCacheException : Exception
    {
        public FileCacheException(string message) : base(message)
        {
        }

        public FileCacheException() : base()
        {
        }

        public FileCacheException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
