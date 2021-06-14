using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            return HashCode.Combine(ID, Name, IsDeleted);
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<HospitalType>(this, null);
        }
    }
}