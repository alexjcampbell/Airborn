using System;

namespace Airborn.web.Models
{
    public class AirportNotFoundException : ArgumentOutOfRangeException
    {

        public AirportNotFoundException(string airport) :
            base(String.Format($"Airport not found."))
        {
            AirportName = airport;
        }

        public string AirportName
        {
            get; set;
        }

    }
}