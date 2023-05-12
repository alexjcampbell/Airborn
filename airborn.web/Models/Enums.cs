using System;
using System.ComponentModel.DataAnnotations;

namespace Airborn.web.Models
{
    public enum TemperatureType
    {
        [Display(Name = "°C")]
        C,
        [Display(Name = "°F")]
        F
    }

    public enum AltimeterSettingType
    {
        [Display(Name = "\"Hg")]
        HG,
        [Display(Name = "mb")]
        MB
    }

    public enum AircraftType
    {
        [Display(Name = "Cirrus SR22 G2")]
        SR22_G2,

        [Display(Name = "Cirrus SR22T G5")]
        SR22T_G5,

        [Display(Name = "Cirrus SR22T G6")]
        SR22T_G6,

        [Display(Name = "Cessna 182T")]
        C182_T,

        [Display(Name = "Cessna 172SP")]
        C172_SP
    }

    public enum Scenario
    {
        Takeoff,
        Landing
    }

    public enum ScenarioMode

    {
        Takeoff_GroundRoll,
        Takeoff_50FtClearance,
        Landing_GroundRoll,
        Landing_50FtClearance
    }

    public enum RunwaySurface
    {
        Paved,
        Grass,
        Unknown
    }

    public enum AirconOptions
    {
        [Display(Name = "Aircon off")]
        NoAircon,
        [Display(Name = "Aircon on")]
        Aircon
    }

    public enum AirportType
    {
        [Display(Name = "Balloon port")]
        balloonport,
        [Display(Name = "Closed")]
        closed,
        [Display(Name = "Heliport")]
        heliport,
        [Display(Name = "Large airport")]
        large_airport,
        [Display(Name = "Medium airport")]
        medium_airport,
        [Display(Name = "Seaplane base")]
        seaplane_base,
        [Display(Name = "Small airport")]
        small_airport
    }

}