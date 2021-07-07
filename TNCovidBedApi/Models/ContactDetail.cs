using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

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
            return Tuple.Create(ID, ContactNumber, ContactName, Timing).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            return JsonSerializer.Serialize<ContactDetail>(this, options);
        }
    }
}