using System;
using System.Collections.Generic;

namespace BysykkelData.Models
{
    public class BikeServiceInfo
    {
        public string ServiceOperator { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }

        public IEnumerable<BikeStation> BikeStations { get; set; }
    }
}
