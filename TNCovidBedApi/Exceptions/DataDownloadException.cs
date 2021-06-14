namespace TNCovidBedApi
{
    ///<summary>
    ///Common exception for all network related exceptions while interacting with API
    ///</summary>
    [System.Serializable]
    public class DataDownloadException : System.Exception
    {
        public DataDownloadException() : base()
        {
        }

        public DataDownloadException(string message) : base(message)
        {
        }

        public DataDownloadException(string message, System.Exception e) : base(message, e)
        {
        }
    }
}