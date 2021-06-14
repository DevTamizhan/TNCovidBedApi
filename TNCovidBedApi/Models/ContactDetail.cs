using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    public class ContactDetail : System.IComparable<ContactDetail>
    {
        public string ContactName { get; set; }

        public string ContactNumber { get; set; }

        [JsonPropertyName("_id")]
        public string ID { get; set; }
        public string Timing { get; set; }

        public int CompareTo(ContactDetail other)
        {
            if (other is null)
                return 1;
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is ContactDetail detail &&
                   ID == detail.ID &&
                   ContactNumber == detail.ContactNumber &&
                   ContactName == detail.ContactName &&
                   Timing == detail.Timing;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, ContactNumber, ContactName, Timing);
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<ContactDetail>(this, null);
        }
    }
}