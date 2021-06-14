using TNCovidBedApi.Models;

namespace TNCovidBedApi
{
    ///<summary>
    ///<para>
    ///Event argument when API is executed.
    ///</para>
    ///<para>
    ///Contains result of API and type of root API result that is being returned
    ///</para>
    ///</summary>
    public class ApiExecutedEventArgs : System.EventArgs
    {
        private readonly IRootResult apiResult;
        private readonly System.Type apiResultType;
        public ApiExecutedEventArgs(IRootResult result, System.Type resultType)
        {
            this.apiResult = result;
            this.apiResultType = resultType;
        }

        public IRootResult APIResult { get => apiResult; }
        public System.Type APIResultType { get => apiResultType; }
    }
}