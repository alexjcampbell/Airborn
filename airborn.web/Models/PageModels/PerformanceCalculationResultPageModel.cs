using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Airborn.web.Models
{
    public class PerformanceCalculationResultPageModel
    {
        private PerformanceCalculationResultPageModel()
        {
        }

        public PerformanceCalculationResultPageModel(PerformanceCalculationResult result, Runway runway)
        {
            Result = result;
            Runway = runway;
        }

        public PerformanceCalculationResult Result
        {
            get;
            set;
        }

        public Runway Runway
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
                return "0 kts";
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

        public double? Takeoff_GroundRoll
        {
            get
            {
                return Result?.Takeoff_GroundRoll;
            }
        }


        public string Takeoff_Groundroll_Formatted
        {
            get
            {
                if (Result != null)
                {

                    return
                        Result?.Takeoff_GroundRoll.ToString("#,##")
                        +
                        " ft ("
                        +
                        Takeoff_PercentageRunwayUsed_GroundRoll?.ToString("P0")
                        + ")"
                        ;
                }

                return "";
            }
        }


        public double? Takeoff_50FtClearance
        {
            get
            {
                return Result?.Takeoff_50FtClearance;
            }
        }

        public string Takeoff_50FtClearance_Formatted
        {
            get
            {
                if (Result != null)
                {
                    return
                        Result?.Takeoff_50FtClearance.ToString("#,##")
                        +
                        " ft ("
                        +
                        Takeoff_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0")
                        + ")"
                        ;
                }

                return "";
            }
        }

        public double? Landing_GroundRoll
        {
            get
            {
                return Result?.Landing_GroundRoll;
            }
        }

        public string Landing_Groundroll_Formatted
        {
            get
            {
                if (Result != null)
                {
                    if (Runway.RunwayLength.Length > 0)
                    {
                        return
                            Result?.Landing_GroundRoll.ToString("#,##")
                            +
                            " ft ("
                            +
                            Landing_PercentageRunwayUsed_GroundRoll?.ToString("P0")
                            + ")"
                            ;
                    }
                }

                return "";

            }
        }

        public double? Landing_50FtClearance
        {
            get
            {
                return Result?.Landing_50FtClearance;
            }
        }

        public string Landing_50FtClearance_Formatted
        {
            get
            {
                if (Result != null)
                {
                    return
                        Result?.Landing_50FtClearance.ToString("#,##")
                        +
                        " ft ("
                        +
                        Landing_PercentageRunwayUsed_DistanceToClear50Ft?.ToString("P0")
                        + ")"
                        ;
                }
                return "";
            }
        }



        public double? Takeoff_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Takeoff_GroundRoll / Runway.RunwayLengthConverted;
            }
        }

        public double? Landing_PercentageRunwayUsed_GroundRoll
        {
            get
            {
                return Landing_GroundRoll / Runway.RunwayLengthConverted;
            }
        }

        public double? Takeoff_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Takeoff_50FtClearance / Runway.RunwayLengthConverted;
            }
        }

        public double? Landing_PercentageRunwayUsed_DistanceToClear50Ft
        {
            get
            {
                return Landing_50FtClearance / Runway.RunwayLengthConverted;
            }
        }
    }
}