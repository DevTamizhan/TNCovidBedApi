using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    ///<summary>
    ///The container to store district detail which is returned from both District API and Bed details API
    ///</summary>
    public class District : IComparable<District>
    {

        public string Code { get; set; }

        public string ColorCode { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }


        ///<summary>
        ///<para>
        ///Unique ID for the district which can be used to get district specific details for bed
        ///</para>
        ///</summary>
        ///<value>
        ///A String value containing district ID
        [JsonPropertyName("_id")]
        public string ID { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsSpecial { get; set; }

        ///<summary>
        ///English Name of district
        ///</summary>
        ///<value>
        ///A String value containing English Name of the district
        public string Name { get; set; }

        public string ShortCode { get; set; }

        public int SortOrder { get; set; }

        public object State { get; set; }

        public string StateCode { get; set; }

        ///<summary>
        ///Support phone number for district
        ///</summary>
        ///<value>
        ///A String value for support phone number for district
        ///</value>
        public string SupportNumber { get; set; }
        ///<summary>
        ///Tamil Name of district
        ///</summary>
        ///<value>
        ///A String value containing Tamil Name of the district
        ///</value>
        public string TamilName { get; set; }
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        public int CompareTo(District other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is District district &&
                   SupportNumber == district.SupportNumber &&
                   IsDeleted == district.IsDeleted &&
                   ID == district.ID &&
                   SortOrder == district.SortOrder &&
                   StateCode == district.StateCode &&
                   Code == district.Code &&
                   TamilName == district.TamilName &&
                   Name == district.Name &&
                   ShortCode == district.ShortCode &&
                   CreatedAt == district.CreatedAt &&
                   UpdatedAt == district.UpdatedAt &&
                   EqualityComparer<object>.Default.Equals(State, district.State) &&
                   ColorCode == district.ColorCode &&
                   IsSpecial == district.IsSpecial;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(SupportNumber);
            hash.Add(IsDeleted);
            hash.Add(ID);
            hash.Add(SortOrder);
            hash.Add(StateCode);
            hash.Add(Code);
            hash.Add(TamilName);
            hash.Add(Name);
            hash.Add(ShortCode);
            hash.Add(CreatedAt);
            hash.Add(UpdatedAt);
            hash.Add(State);
            hash.Add(ColorCode);
            hash.Add(IsSpecial);
            return hash.ToHashCode();
        }

        ///<summary>
        ///Serializes district object to JSON string
        ///</summary>
        ///<returns>
        ///JSON String of district
        ///</returns>
        public string GetJsonString()
        {
            return JsonSerializer.Serialize<District>(this, null);
        }

        ///<summary>
        ///String result of District
        ///</summary>
        ///<returns>
        ///Returns a string in JSON format with id and Name property
        ///</returns>
        public override string ToString()
        {
            return "{" + $"id : \"{this.ID}\", Name : \"{this.Name}\"" + "}";
        }
    }
}