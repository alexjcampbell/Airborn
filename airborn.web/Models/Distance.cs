using System;

namespace Airborn.web.Models
{
    public struct Distance : IEquatable<Distance>, IComparable<Distance>
    {
        private static readonly decimal MetersPerKilometer = 1000.0m;
        private static readonly decimal CentimetersPerMeter = 100.0m;
        private static readonly decimal CentimetersPerInch = 2.54m;
        private static readonly decimal InchesPerFoot = 12.0m;
        private static readonly decimal FeetPerYard = 3.0m;
        private static readonly decimal FeetPerMeter = CentimetersPerMeter / (CentimetersPerInch * InchesPerFoot);
        private static readonly decimal InchesPerMeter = CentimetersPerMeter / CentimetersPerInch;

        private readonly decimal _meters;

        public Distance(decimal meters)
        {
            this._meters = meters;
        }

        public decimal TotalKilometers
        {
            get
            {
                return _meters / MetersPerKilometer;
            }
        }

        public decimal TotalMeters
        {
            get
            {
                return _meters;
            }
        }

        public decimal TotalCentimeters
        {
            get
            {
                return _meters * CentimetersPerMeter;
            }
        }

        public decimal TotalYards
        {
            get
            {
                return _meters * FeetPerMeter / FeetPerYard;
            }
        }

        public decimal TotalFeet
        {
            get
            {
                return _meters * FeetPerMeter;
            }
        }

        public decimal TotalInches
        {
            get
            {
                return _meters * InchesPerMeter;
            }
        }

        public static Distance FromKilometers(decimal value)
        {
            return new Distance(value * MetersPerKilometer);
        }

        public static Distance FromMeters(decimal value)
        {
            return new Distance(value);
        }

        public static Distance FromCentimeters(decimal value)
        {
            return new Distance(value / CentimetersPerMeter);
        }

        public static Distance FromYards(decimal value)
        {
            return new Distance(value * FeetPerYard / FeetPerMeter);
        }

        public static Distance FromFeet(decimal value)
        {
            return new Distance(value / FeetPerMeter);
        }

        public static Distance FromInches(decimal value)
        {
            return new Distance(value / InchesPerMeter);
        }

        public static Distance operator +(Distance a, Distance b)
        {
            return new Distance(a._meters + b._meters);
        }

        public static Distance operator -(Distance a, Distance b)
        {
            return new Distance(a._meters - b._meters);
        }

        public static Distance operator -(Distance a)
        {
            return new Distance(-a._meters);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Distance))
                return false;

            return Equals((Distance)obj);
        }

        public bool Equals(Distance other)
        {
            return this._meters == other._meters;
        }

        public int CompareTo(Distance other)
        {
            return this._meters.CompareTo(other._meters);
        }

        public override int GetHashCode()
        {
            return _meters.GetHashCode();
        }

        public override string ToString()
        {
            // default to ft, until we come back and support output in metres
            return TotalFeet.ToString("N0") + " ft";
        }
    }
}
