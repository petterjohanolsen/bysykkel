using System.Text.Json.Serialization;

namespace BysykkelData.Models.API
{
    public class SystemInformation : GbfsBase
    {
        public Data data { get; set; }

        public class Data
        {
            public string system_id { get; set; }
            public string language { get; set; }
            public string name { get; set; }
            [JsonPropertyName("operator")]
            public string _operator { get; set; }
            public string timezone { get; set; }
            public string phone_number { get; set; }
            public string email { get; set; }
        }
    }
}