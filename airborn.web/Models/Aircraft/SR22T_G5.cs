using System;

namespace Airborn.web.Models
{
    public class SR22T_G5 : SR22
    {

        public SR22T_G5()
        {
        }

        public override string JsonFileName()
        {
            return "../Data/SR22T_G5_3600.json";
        }
    }
}