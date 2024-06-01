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
        private const string BaseUrl = "https://aviationweather.gov/api/data/metar/";


        public async Task<MetarData> GetLatestMetarForAirport(string airportCode)
        {
            var hoursBeforeNow = 1;

            while (hoursBeforeNow <= 24) // Try for up to 24 hours before now
            {
                var queryString = new Dictionary<string, string>
        {
            { "dataSource", "metars" },
            { "requestType", "retrieve" },
            { "format", "xml" },
            { "ids", airportCode },
            { "hoursBeforeNow", hoursBeforeNow.ToString() },
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
                        // Increase the hoursBeforeNow value and try again
                        hoursBeforeNow++;
                        continue;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    using var myActivity = Telemetry.ActivitySource.StartActivity("Failed METAR request");

                    myActivity?.SetTag("AirportIdentifier", airportCode);
                    myActivity?.SetTag("Exception", ex.Message);

                    // Increase the hoursBeforeNow value and try again
                    hoursBeforeNow++;
                    continue;
                }
            }

            // If we've tried for up to 24 hours and still have no data, return an error
            return new MetarData
            {
                StationId = airportCode,
                IsError = true
            };
        }
    }
}