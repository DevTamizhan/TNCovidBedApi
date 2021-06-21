using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    public class Hospital : System.IComparable<Hospital>
    {
        public AddressDetail AddressDetail { get; set; }

        public long BedDetailProcessedDate { get; set; }

        public List<ContactDetail> ContactDetails { get; set; }

        public CovidBedDetails CovidBedDetails { get; set; }

        public District District { get; set; }

        public string FacilityType { get; set; }

        [JsonPropertyName("_id")]
        public string ID { get; set; }
        public string Landline { get; set; }
        public long Last6HoursUpdate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string MobileNumber { get; set; }
        public string Name { get; set; }
        public string PrimaryContactPerson { get; set; }
        public HospitalType Type { get; set; }
        public long UpdatedDateTime { get; set; }
        public int CompareTo(Hospital other)
        {
            if (other is null)
                return 1;
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is Hospital hospital &&
                   ID == hospital.ID &&
                   BedDetailProcessedDate == hospital.BedDetailProcessedDate &&
                   Name == hospital.Name &&
                   EqualityComparer<District>.Default.Equals(District, hospital.District) &&
                   FacilityType == hospital.FacilityType &&
                   EqualityComparer<HospitalType>.Default.Equals(Type, hospital.Type) &&
                   Landline == hospital.Landline &&
                   MobileNumber == hospital.MobileNumber &&
                   PrimaryContactPerson == hospital.PrimaryContactPerson &&
                   EqualityComparer<CovidBedDetails>.Default.Equals(CovidBedDetails, hospital.CovidBedDetails) &&
                   EqualityComparer<List<ContactDetail>>.Default.Equals(ContactDetails, hospital.ContactDetails) &&
                   EqualityComparer<AddressDetail>.Default.Equals(AddressDetail, hospital.AddressDetail) &&
                   Latitude == hospital.Latitude &&
                   Longitude == hospital.Longitude &&
                   UpdatedDateTime == hospital.UpdatedDateTime &&
                   Last6HoursUpdate == hospital.Last6HoursUpdate;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(ID, BedDetailProcessedDate,Name,District,FacilityType,Type,Landline,
                Tuple.Create(MobileNumber,PrimaryContactPerson,CovidBedDetails,ContactDetails,AddressDetail,Latitude,Longitude,
                Tuple.Create(UpdatedDateTime,Last6HoursUpdate))
                ).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<Hospital>(this, null);
        }
    }
}