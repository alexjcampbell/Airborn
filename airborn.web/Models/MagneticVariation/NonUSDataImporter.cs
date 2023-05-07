using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;

public class GeomagClient
{
    private const string BaseUrl = "http://geomag.bgs.ac.uk/web_service/GMModels/wmm/2020/";

    public static async Task GetMagneticVariationForLatLong(double latitude, double longitude, double altitude, DateTime date)
    {
        var geomagClient = new GeomagClient();

        try
        {
            var geomagData = await geomagClient.GetGeomagDataAsync(latitude, longitude, altitude, date);
            Console.WriteLine(geomagData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static double? ExtractMagneticDeclination(XDocument geomagData)
    {
        if (geomagData == null)
        {
            return null;
        }

        var fieldValueElement = geomagData.Element("geomagnetic-field-model-result")?.Element("field-value");

        if (fieldValueElement == null)
        {
            return null;
        }

        var declinationElement = fieldValueElement.Element("declination");

        if (declinationElement == null)
        {
            return null;
        }

        if (double.TryParse(declinationElement.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double declination))
        {
            // If declination is east, it's negative
            string units = declinationElement.Attribute("units")?.Value;
            if (units == "deg (east)")
            {
                declination = -declination;
            }

            return declination;
        }

        return null;
    }


    public async Task<double?> GetGeomagDataAsync(double latitude, double longitude, double altitude, DateTime date)
    {
        using (var httpClient = new HttpClient())
        {
            var url = BuildRequestUrl(latitude, longitude, altitude, date);
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                double? result = ExtractMagneticDeclination(XDocument.Parse(content));

                return result;
            }
            else
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}");
            }
        }
    }

    private string BuildRequestUrl(double latitude, double longitude, double altitude, DateTime date)
    {
        var queryString = $"?latitude={latitude}&longitude={longitude}&altitude={altitude}&date={date:yyyy-MM-dd}&format=xml";
        return BaseUrl + queryString;
    }
}
