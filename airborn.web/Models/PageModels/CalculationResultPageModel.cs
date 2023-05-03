using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{
    /// <summary>
    /// The model for each runway's calculation result, which is displayed on the Calculate page
    /// </summary>
    public class CalculationResultForRunwayPageModel
    {
        private CalculationResultForRunwayPageModel()
        {
        }

        public CalculationResultForRunwayPageModel(CalculationResultForRunway result, Runway runway)
        {
            Result = result;
            Runway = runway;
        }

        public CalculationResultForRunway Result
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

        public double? CrosswindComponent
        {
            get
            {
                return Result?.CrosswindComponent;
            }
        }

        public double? CrosswindComponentAbs
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

        private string CrosswindComponent_RightOrLeft
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

        public string CrosswindComponent_Formatted
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

        private string HeadwindComponent_TailwindWhenNegative
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
        public double? HeadwindComponent
        {
            get
            {
                return Result?.HeadwindComponent;
            }

        }

        public double? HeadwindComponentAbs
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

        public string HeadwindComponent_Formatted
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

        public string Wind_Formatted
        {
            get
            {
                return HeadwindComponent_Formatted + HeadwindComponent_TailwindWhenNegative + ", " + CrosswindComponent_Formatted + CrosswindComponent_RightOrLeft;

            }
        }

        public double? Takeoff_GroundRoll
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


        public double? Takeoff_50FtClearance
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

        public double? Landing_GroundRoll
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

        public double? Landing_50FtClearance
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



        public double? Takeoff_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Takeoff_GroundRoll / Runway.RunwayLength;
            }
        }

        public double? Landing_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Landing_GroundRoll / Runway.LandingAvailableLength.TotalFeet;
            }
        }

        public double? Takeoff_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Takeoff_50FtClearance / Runway.RunwayLength;
            }
        }

        public double? Landing_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Landing_50FtClearance / Runway.LandingAvailableLength.TotalFeet;
            }
        }

        public PerformanceCalculationLogItem LogItem
        {
            get
            {
                return Result?.LogItem;
            }

        }

        public double? Slope
        {
            get
            {
                return Runway?.Slope;
            }
        }

        public string Slope_Formatted
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