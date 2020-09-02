namespace BysykkelData.Models.API
{
    public class StationInformation : GbfsBase
    {
        public Data data { get; set; }

        public class Data
        {
            public Station[] stations { get; set; }
        }

        public class Station
        {
            public string station_id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public float lat { get; set; }
            public float lon { get; set; }
            public int capacity { get; set; }
        }
    }
}