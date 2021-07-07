using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TNCovidBedApi.Models
{
    public class HospitalType : IComparable<HospitalType>
    {
        [JsonPropertyName("_id")]
        public string ID { get; set; }

        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public int CompareTo(HospitalType other)
        {
            if (other is null)
                return 1;
            return ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is HospitalType type &&
                   ID == type.ID &&
                   Name == type.Name &&
                   IsDeleted == type.IsDeleted;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(ID, Name, IsDeleted).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            return JsonSerializer.Serialize<HospitalType>(this, options);
        }
    }
}