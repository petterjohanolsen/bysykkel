using BysykkelData.Models;
using BysykkelData.Models.API;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BysykkelData
{
    public class BysykkelService
    {
        private readonly ILogger<BysykkelService> _logger;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public BysykkelService(ILogger<BysykkelService> logger, IConfiguration config, IMemoryCache cache)
        {
            _logger = logger;
            _config = config;
            _cache = cache;

            _httpClient = new HttpClient();
            var clientIdentifier = _config.GetSection("clientIdentifier").Value;
            _httpClient.DefaultRequestHeaders.Add("Client-Identifier", clientIdentifier);
        }

        string BaseUrl
        {
            get { return _config.GetSection("baseUrl").Value; }
        }

        void Debug(string message)
        {
            _logger.LogDebug($"{DateTime.Now.ToString("hh:mm:ss.fff")} {message}");
        }

        /// <summary>
        ///     Fetch data from endpoint using HttpClient, returns object T.
        //      Honors the GBFS ttl cache duration, returns from memory cache if fresh.
        /// </summary>
        async Task<T> FetchDataAsync<T>(string endpoint) where T : GbfsBase
        {
            var url = $"{BaseUrl}{endpoint}";

            var cached = _cache.Get<T>(endpoint);
            if (cached != null) {
                Debug($"{endpoint} Returned from cache");
                return cached;
            }

            Debug($"{endpoint} Start request");
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var obj = await JsonSerializer.DeserializeAsync<T>(response.Content.ReadAsStreamAsync().Result);
                _cache.Set(endpoint, obj, TimeSpan.FromSeconds(obj.ttl));

                Debug($"{endpoint} End request");
                return obj;
            }

            throw new System.Exception($"Could not load data from {url}, status code = {response.StatusCode}");
        }

        /// <summary>
        ///     Generate OpenStreetMap URL.
        /// </summary>
        string GenMapLink(StationInformation.Station station)
        {
            NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

            var lat = station?.lat.ToString(nfi);
            var lon = station?.lon.ToString(nfi);
            return $"https://www.openstreetmap.org/#map=18/{lat}/{lon}";
        }

        /// <summary>
        ///     Load data from Bike Service, and return BikeServiceInfo with general info and list of stations
        ///     with bikes and free locks.
        /// </summary>
        public async Task<BikeServiceInfo> GetData(string filterStations)
        {
            var t0 = new Stopwatch();
            t0.Start();

            var systemInfo = Task.Run(() => FetchDataAsync<SystemInformation>("system_information.json"));
            var stationInfo = Task.Run(() => FetchDataAsync<StationInformation>("station_information.json"));
            var stationList = Task.Run(() => FetchDataAsync<StationStatus>("station_status.json"));

            await Task.WhenAll(systemInfo, stationInfo, stationList);

            Debug($"Fetch bysykkel data {t0.ElapsedMilliseconds}ms");

            var bikeServiceInfo = new BikeServiceInfo
            {
                ServiceOperator = systemInfo.Result.data._operator,
                Name = systemInfo.Result.data.name,
                LastUpdated = DateTimeOffset.FromUnixTimeSeconds(systemInfo.Result.last_updated).UtcDateTime
            };

            var bikeList = stationList.Result.data.stations
                .Where(x => x.is_installed != 0)
                .Select(x =>
                {
                    var lookupStation = stationInfo.Result.data.stations.SingleOrDefault(y => y.station_id == x.station_id);

                    return new BikeStation
                    {
                        StationName = lookupStation?.name,
                        AvailableBikes = x.num_bikes_available,
                        AvailableDocks = x.num_docks_available,
                        MapLink = GenMapLink(lookupStation)
                    };
                });

            if (!string.IsNullOrWhiteSpace(filterStations))
            {
                bikeList = bikeList.Where(x => x.StationName.ToLower().Contains(filterStations.ToLower()));
            }

            bikeServiceInfo.BikeStations = bikeList.OrderByDescending(x => x.AvailableBikes);
            return bikeServiceInfo;
        }
    }
}
