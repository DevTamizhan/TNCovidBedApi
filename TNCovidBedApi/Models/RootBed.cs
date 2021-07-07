using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TNCovidBedApi.Models
{
    public class RootBed : IRootResult<Hospital>
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
        public List<Hospital> Result { get; set; }

        [JsonPropertyName("statusCode")]
        public string StatusCode { get; set; }
        [JsonPropertyName("warnings")]
        public List<object> Warnings { get; set; }

        public static RootBed ParseJSON(string data)
        {
            return JsonSerializer.Deserialize<RootBed>(data, null);
        }

        public override bool Equals(object obj)
        {
            return obj is RootBed bed &&
                   EqualityComparer<List<Hospital>>.Default.Equals(Result, bed.Result) &&
                   EqualityComparer<object>.Default.Equals(Exception, bed.Exception) &&
                   EqualityComparer<Pagination>.Default.Equals(Pagination, bed.Pagination) &&
                   StatusCode == bed.StatusCode &&
                   EqualityComparer<List<object>>.Default.Equals(Errors, bed.Errors) &&
                   EqualityComparer<List<object>>.Default.Equals(Warnings, bed.Warnings);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Result, Exception, Pagination, StatusCode, Errors, Warnings).GetHashCode();
        }

        ///<summary>
        ///Convert the RootBed object to it's JSON String
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
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                return JsonSerializer.Serialize<RootBed>(this, options);
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