namespace TNCovidBedApi
{
    /// <summary>
    /// Cache is not available.
    /// <para>
    /// Download data then access the cache to avoid this exception
    /// </para>
    /// </summary>
    [System.Serializable]
    public class NoCacheException : System.Exception
    {
        public NoCacheException(string message) : base(message)
        {
        }

        public NoCacheException() : base()
        {
        }

        public NoCacheException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}