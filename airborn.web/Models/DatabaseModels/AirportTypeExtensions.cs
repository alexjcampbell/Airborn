using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Airborn.web.Models;

public static class AirportTypeExtensions
{
    public static string GetDisplayName(this AirportType airportType)
    {
        var memberInfo = typeof(AirportType).GetMember(airportType.ToString())[0];
        var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.Name ?? airportType.ToString();
    }
}