using System;

namespace Airborn.web.Models
{
    public class SR22_G2 : SR22
    {

        public SR22_G2()
        {
        }

        public override string JsonFileName_LowerWeight()
        {
            return "../Data/SR22_G2_2900.json";
        }

        public override string JsonFileName_HigherWeight()
        {
            return "../Data/SR22_G2_3400.json";
        }

        public override int GetLowerWeight()
        {
            return 2900;
        }

        public override int GetHigherWeight()
        {
            return 3400;
        }        
    }
}