using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{
    /// <summary>
    /// The model for each runway's calculation result, which is displayed on the Calculate page
    /// </summary>
    public class CalculationResultPageModel
    {
        private CalculationResultPageModel()
        {
        }

        public CalculationResultPageModel(PerformanceCalculationResultForRunway result, Runway runway)
        {
            Result = result;
            Runway = runway;
        }

        public PerformanceCalculationResultForRunway Result
        {
            get;
            set;
        }

        public Runway Runway
        {
            get;
            set;
        }

        public bool IsBestWind
        {
            get;
            set;
        }

        public decimal? CrosswindComponent
        {
            get
            {
                return Result?.CrosswindComponent;
            }
        }

        public decimal? CrosswindComponentAbs
        {
            get
            {

                var crosswindComponent = Result?.CrosswindComponent;

                if (crosswindComponent < 0)
                {
                    crosswindComponent = crosswindComponent * -1;
                }

                return crosswindComponent;
            }
        }

        public string CrosswindComponentText
        {
            get
            {
                if (Result?.CrosswindComponent > 0)
                {
                    return " right";
                }
                else if (Result?.CrosswindComponent < 0)
                {
                    return " left";
                }
                return "";
            }
        }

        public string CrosswindComponentOutput
        {
            get
            {
                if (CrosswindComponentAbs > 0)
                {
                    return CrosswindComponentAbs?.ToString(",##0") + " kts";
                }
                return "0 kts x-wind";
            }
        }

        public string HeadwindComponentText
        {
            get
            {
                if (Result?.HeadwindComponent > 0)
                {
                    return " headwind";
                }
                else if (Result?.HeadwindComponent < 0)
                {
                    return " tailwind";
                }
                return " headwind";
            }
        }
        public decimal? HeadwindComponent
        {
            get
            {
                return Result?.HeadwindComponent;
            }

        }

        public decimal? HeadwindComponentAbs
        {
            get
            {

                // if it's a tailwind we'll describe it as such, and the page caller needs an absolute value
                // of the wind component we calculated to show the tailwind amount

                var headwindComponent = Result?.HeadwindComponent;

                if (headwindComponent < 0)
                {
                    headwindComponent = headwindComponent * -1;
                }

                return headwindComponent;
            }
        }

        public string HeadwindComponentOutput
        {
            get
            {
                if (HeadwindComponentAbs > 0)
                {
                    return HeadwindComponentAbs?.ToString(",##0") + " kts";
                }
                return "0 kts";
            }
        }

        public string WindFormatted
        {
            get
            {
                return HeadwindComponentOutput + HeadwindComponentText + ", " + CrosswindComponentOutput + CrosswindComponentText;

            }
        }

        public decimal? Takeoff_GroundRoll
        {
            get
            {
                return Result?.Takeoff_GroundRoll.TotalFeet;
            }
        }


        public string Takeoff_Groundroll_Formatted
        {
            get
            {
                if (Result != null)
                {

                    string result = Result?.Takeoff_GroundRoll.ToString();

                    if (Takeoff_PercentageRunwayUsed_GroundRoll.HasValue)
                    {
                        result += " (" + Takeoff_PercentageRunwayUsed_GroundRoll?.ToString("P0") + ")";
                    }

                    return result;
                }

                return "";
            }
        }


        public decimal? Takeoff_50FtClearance
        {
            get
            {
                return Result?.Takeoff_50FtClearance.TotalFeet;
            }
        }

        public string Takeoff_50FtClearance_Formatted
        {
            get
            {
                if (Result != null)
                {
                    string result = Result?.Takeoff_50FtClearance.ToString();

                    if (Takeoff_PercentageRunwayUsed_DistanceToClear50Ft.HasValue)
                    {
                        result += " (" + Takeoff_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0") + ")";
                    }


                    return result;
                }

                return "";
            }
        }

        public decimal? Landing_GroundRoll
        {
            get
            {
                return Result?.Landing_GroundRoll.TotalFeet;
            }
        }

        public string Landing_Groundroll_Formatted
        {
            get
            {
                if (Result != null)
                {

                    string result = Result?.Landing_GroundRoll.ToString();

                    if (Landing_PercentageRunwayUsed_GroundRoll.HasValue)
                    {
                        result += " (" + Landing_PercentageRunwayUsed_GroundRoll?.ToString("P0") + ")";
                    }

                    return result;

                }

                return "";

            }
        }

        public decimal? Landing_50FtClearance
        {
            get
            {
                return Result?.Landing_50FtClearance.TotalFeet;
            }
        }

        public string Landing_50FtClearance_Formatted
        {
            get
            {
                if (Result != null)
                {
                    string result = Result?.Landing_50FtClearance.ToString();

                    if (Landing_PercentageRunwayUsed_DistanceToClear50Ft.HasValue)
                    {
                        result += " (" + Landing_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0") + ")";
                    }

                    return result;
                }
                return "";
            }
        }



        public decimal? Takeoff_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Takeoff_GroundRoll / Runway.RunwayLengthConverted;
            }
        }

        public decimal? Landing_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Landing_GroundRoll / Runway.RunwayLengthConverted;
            }
        }

        public decimal? Takeoff_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Takeoff_50FtClearance / Runway.RunwayLengthConverted;
            }
        }

        public decimal? Landing_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Landing_50FtClearance / Runway.RunwayLengthConverted;
            }
        }

        public PerformanceCalculationLogItem LogItem
        {
            get
            {
                return Result?.LogItem;
            }

        }

        public decimal? Slope
        {
            get
            {
                return Runway?.Slope;
            }
        }

        public string Slope_Friendly
        {
            get
            {
                if (Slope.HasValue)
                {
                    return Slope.Value.ToString("0.00") + "%";
                }
                return "Unknown";
            }
        }

    }
}