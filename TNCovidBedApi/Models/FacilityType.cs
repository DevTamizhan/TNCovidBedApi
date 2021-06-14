namespace TNCovidBedApi.Models
{
    [System.Flags]
    public enum FacilityType
    {
        All = 0,
        CHO = 1,
        CHC = 2,
        CCC = 4,
        ICCC = 8,
    }
}