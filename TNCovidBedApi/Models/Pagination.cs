using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    public class Pagination
    {
        [JsonPropertyName("pageLimit")]
        public ulong PageLimit { get; set; }

        [JsonPropertyName("pageNumber")]
        public long PageNumber { get; set; }
        [JsonPropertyName("skipCount")]
        public long SkipCount { get; set; }

        [JsonPropertyName("totalCount")]
        public long TotalCount { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Pagination pagination &&
                   PageNumber == pagination.PageNumber &&
                   PageLimit == pagination.PageLimit &&
                   SkipCount == pagination.SkipCount &&
                   TotalCount == pagination.TotalCount;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(PageLimit, PageNumber, SkipCount,TotalCount).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<Pagination>(this, null);
        }
    }
}