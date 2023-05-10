using System;

namespace Airborn.web.Models
{
    public class SR22T_G5 : SR22
    {

        public SR22T_G5()
        {
        }

        public override string JsonFileName_LowestWeight
        {
            get
            {
                return "../Data/SR22T_G5_2900.json";
            }
        }

        public override string JsonFileName_HighestWeight
        {
            get
            {
                return "../Data/SR22T_G5_3600.json";
            }
        }

        public override int LowestPossibleWeight
        {
            get
            {
                return 2900;
            }
        }

        public override int HighestPossibleWeight
        {
            get
            {
                return 3600;
            }
        }

        public override int LowestPossibleTemperature
        {
            get
            {
                return 0;
            }
        }

        public override int HighestPossibleTemperature
        {
            get
            {
                return 50;
            }
        }

        public override string AircraftTypeString
        {
            get
            {
                return "Cirrus SR22T G5";
            }
        }
    }
}