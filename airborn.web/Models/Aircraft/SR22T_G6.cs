using System;

namespace Airborn.web.Models
{
    public class SR22T_G6 : SR22
    {

        public SR22T_G6()
        {
        }

        public override string JsonFileName_LowerWeight()
        {
            return "../Data/SR22T_G6_2900.json";
        }

        public override string JsonFileName_HigherWeight()
        {
            return "../Data/SR22T_G6_3600.json";
        }

        public override int GetLowerWeight()
        {
            return 2900;
        }

        public override int GetHigherWeight()
        {
            return 3600;
        }

        public override string GetAircraftTypeString()
        {
            return "Cirrus SR22T G6";
        }
    }
}