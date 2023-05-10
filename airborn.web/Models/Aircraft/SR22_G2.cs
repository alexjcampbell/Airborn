using System;

namespace Airborn.web.Models
{
    public class SR22_G2 : SR22
    {

        public SR22_G2()
        {
        }

        public override string JsonFileName_LowestWeight
        {
            get
            {
                return "../Data/SR22_G2_2900.json";
            }
        }

        public override string JsonFileName_HighestWeight
        {
            get
            {
                return "../Data/SR22_G2_3400.json";
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
                return 3400;
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
                return 40;
            }
        }

        public override string AircraftTypeString
        {
            get
            {
                return "Cirrus SR22 G2";
            }
        }
    }
}