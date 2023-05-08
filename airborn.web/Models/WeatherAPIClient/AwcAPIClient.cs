using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Airborn.web.Models
{

    public class AwcApiClient
    {
        private const string BaseUrl = "https://aviationweather.gov/adds/dataserver_current/httpparam";

        public async Task<MetarData> GetLatestMetarForAirport(string airportCode)
        {
            var queryString = new Dictionary<string, string>
            {
                { "dataSource", "metars" },
                { "requestType", "retrieve" },
                { "format", "xml" },
                { "stationString", airportCode },
                { "hoursBeforeNow", "1" },
                { "mostRecent", "true" },
            };

            try
            {
                var url = $"{BaseUrl}?{string.Join("&", queryString.Select(x => $"{x.Key}={x.Value}"))}";

                using var client = new HttpClient();
                using var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var doc = XDocument.Parse(content);

                var result = doc.Root.Descendants("METAR").Select(x => new MetarData
                {
                    StationId = x.Element("station_id")?.Value,
                    ObservationTime = DateTime.ParseExact(x.Element("observation_time")?.Value, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
                    Temperature = float.Parse(x.Element("temp_c")?.Value ?? "0"),
                    WindSpeed = float.Parse(x.Element("wind_speed_kt")?.Value ?? "0"),
                    WindDirection = float.Parse(x.Element("wind_dir_degrees")?.Value ?? "0"),
                    AltimeterSetting = float.Parse(x.Element("altim_in_hg")?.Value ?? "0"),
                    IsError = false

                }).FirstOrDefault();

                if (result == null)
                {
                    using var myActivity = Telemetry.ActivitySource.StartActivity("Failed METAR request");

                    myActivity?.SetTag("AirportIdentifier", airportCode);
                    myActivity?.SetTag("Exception", content);


                    return new MetarData
                    {
                        StationId = airportCode,
                        IsError = true
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                using var myActivity = Telemetry.ActivitySource.StartActivity("Failed METAR request");

                myActivity?.SetTag("AirportIdentifier", airportCode);
                myActivity?.SetTag("Exception", ex.Message);

                var result = new MetarData
                {
                    StationId = "ERROR",
                };

                return null;
            }


        }
    }
}