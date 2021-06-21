using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TNCovidBedApi.Models
{
    public class CovidBedDetails : System.IComparable<CovidBedDetails>
    {
        public long AllotedICUBeds { get; set; }
        public long AllotedNonO2Beds { get; set; }
        public long AllotedO2Beds { get; set; }
        public long BedsAllotedForCovidTreatment { get; set; }
        [JsonPropertyName("_id")]
        public string ID { get; set; }

        public string LastUpdatedBy { get; set; }
        public string LastUpdatedByUser { get; set; }
        public long LastUpdatedTime { get; set; }
        public long OccupancyICUBeds { get; set; }
        public long OccupancyNonO2Beds { get; set; }
        public long OccupancyO2Beds { get; set; }
        public string Remarks { get; set; }
        public long StatusAsOf { get; set; }
        public long TotalBedsInHospital { get; set; }
        public long TotalVaccantBeds { get; set; }
        public long UpdatedOn { get; set; }
        public long UpdateMissedCount { get; set; }
        public long VaccantICUBeds { get; set; }
        public long VaccantNonO2Beds { get; set; }
        public long VaccantO2Beds { get; set; }
        public int CompareTo(CovidBedDetails other)
        {
            if (other is null)
                return 1;
            return this.ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is CovidBedDetails details &&
                   Remarks == details.Remarks &&
                   LastUpdatedTime == details.LastUpdatedTime &&
                   LastUpdatedBy == details.LastUpdatedBy &&
                   LastUpdatedByUser == details.LastUpdatedByUser &&
                   UpdateMissedCount == details.UpdateMissedCount &&
                   ID == details.ID &&
                   TotalBedsInHospital == details.TotalBedsInHospital &&
                   BedsAllotedForCovidTreatment == details.BedsAllotedForCovidTreatment &&
                   AllotedO2Beds == details.AllotedO2Beds &&
                   AllotedNonO2Beds == details.AllotedNonO2Beds &&
                   AllotedICUBeds == details.AllotedICUBeds &&
                   OccupancyO2Beds == details.OccupancyO2Beds &&
                   OccupancyNonO2Beds == details.OccupancyNonO2Beds &&
                   OccupancyICUBeds == details.OccupancyICUBeds &&
                   VaccantO2Beds == details.VaccantO2Beds &&
                   VaccantNonO2Beds == details.VaccantNonO2Beds &&
                   VaccantICUBeds == details.VaccantICUBeds &&
                   StatusAsOf == details.StatusAsOf &&
                   TotalVaccantBeds == details.TotalVaccantBeds &&
                   UpdatedOn == details.UpdatedOn;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Remarks, LastUpdatedBy, LastUpdatedTime, LastUpdatedByUser, UpdateMissedCount, ID, TotalBedsInHospital,
                Tuple.Create(BedsAllotedForCovidTreatment, AllotedO2Beds, AllotedNonO2Beds, AllotedICUBeds, OccupancyICUBeds, OccupancyNonO2Beds, OccupancyO2Beds,
                Tuple.Create(VaccantICUBeds, VaccantNonO2Beds, VaccantO2Beds, StatusAsOf, TotalBedsInHospital, UpdatedOn)
                )).GetHashCode();
        }

        /// <summary>
        /// Json string of current object
        /// </summary>
        /// <returns>JSON string value</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize<CovidBedDetails>(this, null);
        }
    }
}