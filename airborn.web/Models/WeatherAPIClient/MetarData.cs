using System;

namespace Airborn.web.Models
{

    public class MetarData
    {
        public bool IsError { get; set; }

        public string StationId { get; set; }
        public DateTime ObservationTime { get; set; }
        public float Temperature { get; set; }
        public float WindSpeed { get; set; }

        public float WindDirection { get; set; }

        public float? WindDirectionMagnetic
        {
            get
            {
                if (IsError)
                {
                    return null;
                }

                Airport airport = DbContext.GetAirport(StationId);

                double windDirectionMagnetic =
                    Direction.ConvertTrueToMagnetic(
                        WindDirection,
                        airport.MagneticVariation.GetValueOrDefault()
                        );

                return
                    (float)windDirectionMagnetic;
            }
        }

        public float AltimeterSetting { get; set; }

        public AirbornDbContext DbContext
        {
            get;
            set;
        }

    }
}