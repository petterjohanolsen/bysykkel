namespace BysykkelData.Models
{
    public class BikeStation
    {
        public string StationName { get; set; }
        public int AvailableBikes { get; set; }
        public int AvailableDocks { get; set; }
        public string MapLink { get; set; }
    }
}