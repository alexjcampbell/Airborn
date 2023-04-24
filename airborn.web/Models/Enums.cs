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

    public enum RunwaySurface
    {
        Paved,
        DryGrass
    }

    public enum AircraftType
    {
        [Display(Name = "SR22 G2")]
        SR22_G2,

        [Display(Name = "SR22T G5")]
        SR22T_G5,

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
}