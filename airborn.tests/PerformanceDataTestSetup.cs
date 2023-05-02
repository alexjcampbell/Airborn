using System;
using Airborn.web.Models;

namespace Airborn.Tests
{
    public class BookPerformanceDataTestSetup
    {

        public const int MinimumTemperature = 0;
        public const int MaximumTemperature = 40;

        public const int TemperatureInterval = 10;

        public const int MinimumPressure = 0;
        public const int MaximumPressure = 10000;

        public const int PressureInterval = 1000;

        public const int AircraftWeightInterval = 50;

        public static BookPerformanceDataList GenerateSampleData(AircraftType aircraftType)
        {
            int minimumAircraftWeight = Aircraft.GetAircraftFromAircraftType(aircraftType).GetLowerWeight();
            int maximumAircraftWeight = Aircraft.GetAircraftFromAircraftType(aircraftType).GetHigherWeight();

            BookPerformanceDataList sampleData = new BookPerformanceDataList(minimumAircraftWeight, maximumAircraftWeight);

            for (int temperature = MinimumTemperature; temperature <= MaximumTemperature; temperature += TemperatureInterval)
            {
                for (int pressure = MinimumPressure; pressure <= MaximumPressure; pressure += PressureInterval)
                {
                    for (int weight = minimumAircraftWeight; weight <= maximumAircraftWeight; weight += AircraftWeightInterval)
                    {
                        Distance groundRoll = Distance.FromFeet(pressure + temperature + (0.1m * weight));
                        Distance distanceToClear50Ft = Distance.FromFeet(groundRoll.TotalFeet * 1.2m);

                        BookPerformanceData takeoffData = new BookPerformanceData(
                                Scenario.Takeoff,
                                Distance.FromFeet(pressure),
                                temperature,
                                weight,
                                groundRoll,
                                distanceToClear50Ft
                                );

                        sampleData.Add(takeoffData);

                        BookPerformanceData landingData = new BookPerformanceData(
                                Scenario.Landing,
                                Distance.FromFeet(pressure),
                                temperature,
                                weight,
                                Distance.FromFeet(groundRoll.TotalFeet * 1.1m),
                                Distance.FromFeet(distanceToClear50Ft.TotalFeet * 1.1m)
                                );

                        sampleData.Add(landingData);
                    }
                }

            }
            return sampleData;

        }

    }
}