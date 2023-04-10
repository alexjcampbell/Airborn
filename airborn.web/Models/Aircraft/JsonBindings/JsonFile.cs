using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Airborn.web.Models
{
    public class JsonFile
    {

        public JsonFile()
        {
        }

        public JsonFile(string path)
        {
            LoadJson(path);
        }

        protected JsonPerformanceProfileList _takeoffProfiles;

        public virtual JsonPerformanceProfileList TakeoffProfiles
        {
            get
            {
                return _takeoffProfiles;
            }
            set
            {
                _takeoffProfiles = value;
            }
        }

        protected JsonPerformanceProfileList _landingProfiles;

        public virtual JsonPerformanceProfileList LandingProfiles
        {
            get
            {
                return _landingProfiles;
            }
            set
            {
                _landingProfiles = value;
            }
        }

        public int AircraftWeight
        {
            get;
            set;
        }

        public void LoadJson(string path)
        {


            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            };

            StreamReader sr = new StreamReader(path);

            var json = sr.ReadToEnd();

            JsonConvert.PopulateObject(json, this); ;


        }

    }
}