using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    ///<summary>
    ///API Result for the District API request from the URL https://tncovidbeds.tnega.org/api/district
    ///</summary>
    public class RootDistrict : IRootResult<District>, IComparable<RootDistrict>
    {
        [JsonPropertyName("errors")]
        public List<object> Errors { get; set; }

        [JsonPropertyName("exception")]
        public object Exception { get; set; }

#nullable enable
        [JsonPropertyName("pagination")]
        public Pagination? Pagination { get; set; }
#nullable disable

        [JsonPropertyName("result")]
        public List<District> Result { get; set; }



        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }
        [JsonPropertyName("warnings")]
        public List<object> Warnings { get; set; }

        ///<summary>
        ///Parse the JSON string to the RootDistrict Object
        ///</summary>
        ///<returns>
        ///A RootDistrict object
        ///</returns>
        ///<param name="data">
        ///JSON string value of the District API response or JSON String value of RootDistrict object
        ///</param>
        ///<exception cref="System.Text.Json.JsonException">
        ///Exception occurs when JSON string cannot be serialized
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when JSON string does not match RootDistrict type
        ///</exception>
        public static RootDistrict ParseJSON(string data)
        {
            try
            {
                //TODO add log
                return JsonSerializer.Deserialize<RootDistrict>(data, null);
            }
            catch (JsonException)
            {
                //TODO add log
                throw;
            }
            catch (NotSupportedException)
            {
                //TODO add log
                throw;
            }
        }

        int IComparable<RootDistrict>.CompareTo(RootDistrict other)
        {
            if (this.Result.Count != other.Result.Count)
                return this.Result.Count - other.Result.Count;
            else
            {
                int value = 0;
                for (int i = 0; i < Result.Count; i++)
                {
                    value += this.Result[i].CompareTo(other.Result[i]);
                }
                return value;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is RootDistrict district &&
                   EqualityComparer<List<District>>.Default.Equals(Result, district.Result) &&
                   EqualityComparer<object>.Default.Equals(Exception, district.Exception) &&
                   EqualityComparer<Pagination>.Default.Equals(Pagination, district.Pagination) &&
                   StatusCode == district.StatusCode &&
                   EqualityComparer<List<object>>.Default.Equals(Errors, district.Errors) &&
                   EqualityComparer<List<object>>.Default.Equals(Warnings, district.Warnings);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Result, Exception, Pagination, StatusCode, Errors, Warnings).GetHashCode();
        }

        ///<summary>
        ///Convert the RootDistrict object to it's JSON String
        ///</summary>
        ///<returns>
        ///A Json string of current object
        ///</returns>
        ///<exception cref="System.ArgumentException">
        ///Exception occurs when current object cannot be converted to string
        ///</exception>
        ///<exception cref="System.NotSupportedException">
        ///Exception is throwed when current object is invalid
        ///</exception>
        public string ToJSONString()
        {
            try
            {
                return JsonSerializer.Serialize<RootDistrict>(this, null);
            }
            catch (ArgumentException)
            {
                //TODO add log
                throw;
            }
            catch (NotSupportedException)
            {
                //TODO add log
                throw;
            }
        }
        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return this.ToJSONString();
        }
    }
}