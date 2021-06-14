using System.Collections.Generic;

namespace TNCovidBedApi.Models
{
    ///<summary>
    ///Common type for the root results returned by API.
    ///All Root Result need to be IRootResult for grouping of result in APIService
    ///</summary>
    public interface IRootResult
    {
        ///<summary>
        ///Converts the current object to it's equivalent JSON string
        ///</summary>
        ///<returns>
        ///JSON string of current object
        ///</returns>
        public string ToJSONString();
    }

    ///<summary>
    ///Generic type for the root results returned by API.
    ///This is abstract structure of the result returned by API
    ///Used for the type definition and other internal APIs and cannot be used external to this library
    ///</summary>
    internal interface IRootResult<T> : IRootResult
    {
        public List<object> Errors { get; set; }
        public object Exception { get; set; }
#nullable enable
        public Pagination? Pagination { get; set; }
#nullable disable
        public List<T> Result { get; set; }
        public string StatusCode { get; set; }
        public List<object> Warnings { get; set; }
    }
}