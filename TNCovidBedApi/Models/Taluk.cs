using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    public class Taluk : IComparable<Taluk>
    {
        public string Code { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        public string District { get; set; }

        [JsonPropertyName("_id")]
        public string ID { get; set; }

        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public long SortOrder { get; set; }
        public string TamilName { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        public int CompareTo(Taluk other)
        {
            if (other is null)
                return 1;
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is Taluk taluk &&
                   ID == taluk.ID &&
                   IsDeleted == taluk.IsDeleted &&
                   SortOrder == taluk.SortOrder &&
                   Code == taluk.Code &&
                   TamilName == taluk.TamilName &&
                   Name == taluk.Name &&
                   District == taluk.District &&
                   CreatedAt == taluk.CreatedAt &&
                   UpdatedAt == taluk.UpdatedAt;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(ID, IsDeleted, SortOrder, Code, TamilName, District, CreatedAt, Tuple.Create(UpdatedAt)).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<Taluk>(this, null);
        }
    }
}