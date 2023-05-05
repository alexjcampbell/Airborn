using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace Airborn.web.Models
{
    class FaaDataParser
    {
        public Dictionary<string, string> Parse()
        {
            string filePath = "APT_AIXM.xml";

            //RemoveInvalidXmlCharacters(filePath, "APT_AIXM_Cleaned.xml");

            Dictionary<string, string> airportData = GetAirportData(filePath);

            return airportData;
        }

        static void RemoveInvalidXmlCharacters(string filePath, string outputPath)
        {
            using (var input = File.OpenText(filePath))
            using (var output = File.CreateText(outputPath))
            {
                int current;
                while ((current = input.Read()) >= 0)
                {
                    char ch = (char)current;
                    if (XmlConvert.IsXmlChar(ch))
                    {
                        output.Write(ch);
                    }
                }
            }
        }

        static Dictionary<string, string> GetAirportData(string filePath)
        {
            XNamespace aixm = "http://www.aixm.aero/schema/5.1";
            XNamespace gml = "http://www.opengis.net/gml/3.2";
            Dictionary<string, string> airportData = new Dictionary<string, string>();

            XDocument xmlDoc = XDocument.Load(filePath);

            foreach (XElement airportHeliport in xmlDoc.Descendants(aixm + "AirportHeliport"))
            {
                XElement timeSlice = airportHeliport.Element(aixm + "timeSlice");
                XElement airportHeliportTimeSlice = timeSlice.Element(aixm + "AirportHeliportTimeSlice");

                if (airportHeliportTimeSlice.Element(aixm + "locationIndicatorICAO") == null)
                {
                    continue;
                }
                if (airportHeliportTimeSlice.Element(aixm + "magneticVariation") == null)
                {
                    continue;
                }
                string airportIdentifier = airportHeliportTimeSlice.Element(aixm + "locationIndicatorICAO").Value;
                string magneticVariation = airportHeliportTimeSlice.Element(aixm + "magneticVariation").Value;

                airportData[airportIdentifier] = magneticVariation;
            }

            return airportData;

        }
    }
}