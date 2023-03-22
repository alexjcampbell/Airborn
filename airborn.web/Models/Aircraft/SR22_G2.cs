using System;

namespace Airborn.web.Models
{
    public class SR22_G2 : SR22
    {

        public SR22_G2()
        {
        }

        public override string JsonFileName()
        {
            return "../Data/SR22_G2_3400.json";
        }
    }
}