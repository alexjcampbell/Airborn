using System;

namespace Airborn.web.Models
{

    public class MetarData
    {
        public string StationId { get; set; }
        public DateTime ObservationTime { get; set; }
        public float Temperature { get; set; }
        public float WindSpeed { get; set; }

        public float WindDirection { get; set; }

        public float WindDirectionMagnetic
        {
            get
            {
                double windDirectionMagnetic =
                    Direction.ConvertTrueToMagnetic(
                        WindDirection,
                        AirportDbContext.GetAirport(StationId).MagneticVariation.GetValueOrDefault()
                        );

                return
                    (float)windDirectionMagnetic;
            }
        }

        public float AltimeterSetting { get; set; }

        public AirportDbContext AirportDbContext
        {
            get;
            set;
        }

    }
}