using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace TNCovidBedApi.Models
{
    public class AddressDetail : System.IComparable<AddressDetail>
    {
        [JsonPropertyName("_id")]
        public string ID { get; set; }

        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public Taluk Taluk { get; set; }

        public int CompareTo(AddressDetail other)
        {
            if (other is null)
                return 1;
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is AddressDetail detail &&
                detail.ID == this.ID &&
                detail.Line1 == this.Line1 &&
                detail.Line2 == this.Line2 &&
                detail.Line3 == this.Line3;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return System.Tuple.Create(ID, Line1, Line2, Line3).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            return JsonSerializer.Serialize<AddressDetail>(this, options);
        }
    }
}