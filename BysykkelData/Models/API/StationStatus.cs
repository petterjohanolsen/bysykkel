namespace BysykkelData.Models.API
{
    public class StationStatus : GbfsBase
    {
        public Data data { get; set; }

        public class Data
        {
            public Station[] stations { get; set; }
        }

        public class Station
        {
            public string station_id { get; set; }
            public int is_installed { get; set; }
            public int is_renting { get; set; }
            public int is_returning { get; set; }
            public int last_reported { get; set; }
            public int num_bikes_available { get; set; }
            public int num_docks_available { get; set; }
        }
    }
}