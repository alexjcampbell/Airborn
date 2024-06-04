$(document).ready(function () {
    var airportCode = $('#AirportIdentifier').val();
    if (airportCode) {
        getWeather(airportCode);
        mixpanel.track("Airport code entered", { "AirportCode": airportCode });
    }

    var aircraftType = $('#AircraftType').val();
    if (aircraftType) {
        getAircraftMaxGrossWeight(aircraftType);
        getAircraftIsAirconditioned(aircraftType);
        mixpanel.track("Aircraft type entered", { "AircraftType": aircraftType });
    }
});

$(document).ready(function () {
    $('#AirportIdentifier').autocomplete({
        source: '/PlanFlight/AutocompleteAirportIdentifier',
        minLength: 2,
        select: function (event, ui) {
            $('#AirportIdentifier').val(ui.item.value);
            $('#AirportIdentifier').trigger('change');

            var airportCode = ui.item.value;

            getWeather(airportCode);
            mixpanel.track("Airport code selected", { "AirportCode": airportCode });
        }
    });
    $('#AircraftType').on('change', function () {
        var aircraftType = $(this).val();

        getAircraftMaxGrossWeight(aircraftType);
        getAircraftIsAirconditioned(aircraftType);
        mixpanel.track("Aircraft type changed", { "AircraftType": aircraftType });

    });
    $('#AirportIdentifier').on('change', function () {
        var airportCode = $('#AirportIdentifier').val();
        getWeather(airportCode);
        mixpanel.track("Airport code changed", { "AirportCode": airportCode });
    });
});

function getWeather(airportCode) {
    $.ajax({
        url: "/Weather/GetMetarForAirport",
        type: "GET",
        data: { airportCode: airportCode },
        success: function (data) {
            // Read the fields from the JSON response and do something with them
            // Read the fields from the JSON response and do something with them
            var isError = data.isError;
            var stationId = data.stationId;
            var temperature = data.temperature;
            var windSpeed = data.windSpeed;
            var windDirection = data.windDirection;
            var windDirectionMagnetic = data.windDirectionMagnetic;
            var altimeterSetting = data.altimeterSetting;
            if (isError) {
                $('#StationIdFailed').text(stationId);
                $('#METARFailedToast').toast('show');
                // Set the textboxes and dropdowns with the METAR data
                $("#Temperature").val("");
                $("#TemperatureType option:first").prop("selected", true);
                $("#WindStrength").val("");
                $("#WindDirectionMagnetic").val("");
                $("#AltimeterSetting").val("");
                $("#AltimeterSettingType option:first").prop("selected", true);
                return;
            }
            // Set the textboxes and dropdowns with the METAR data
            $("#Temperature").val(temperature.toFixed(0));
            $("#TemperatureType option:first").prop("selected", true);
            $("#WindStrength").val(windSpeed.toFixed(0));
            $("#WindDirectionMagnetic").val(windDirectionMagnetic.toFixed(0));
            $("#AltimeterSetting").val(altimeterSetting.toFixed(2));
            $("#AltimeterSettingType option:first").prop("selected", true);
            // Set the text and show the toast
            $('#StationId').text(stationId);
            $('#TemperatureToast').text(temperature.toFixed(0));
            $('#WindSpeedToast').text(windSpeed.toFixed(0));
            $('#WindDirectionMagneticToast').text(windDirectionMagnetic.toFixed(0));
            $('#WindDirectionTrueToast').text(windDirection);
            $('#AltimeterSettingToast').text(altimeterSetting.toFixed(2));
            $('#METARToast').toast('show');

            mixpanel.track("Weather data received", { "AirportCode": airportCode, "IsError": isError, "StationId": stationId, "Temperature": temperature, "WindSpeed": windSpeed, "WindDirection": windDirection });
            console.log("Latest METAR for " + stationId + ": temperature " + temperature + "°C, wind speed " + windSpeed + " knots, wind direction magnetic " + windDirectionMagnetic + " degrees, wind direction true " + windDirection + ",  altimeter " + altimeterSetting.toFixed(2) + " inHg");

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log("Error: " + textStatus + " - " + errorThrown);
            if (jqXHR.status === 400) {
                console.log(jqXHR.responseText);
                $('#StationIdFailed').text(stationId);
                $('#METARFailedToast').toast('show');
            }
        }
    });
}

function getAircraftMaxGrossWeight(aircraftType) {
    $.ajax({
        url: '/PlanFlight/GetAircraftMaxGrossWeight',
        data: { aircraftType: aircraftType },
        type: 'GET',
        success: function (data) {
            $('#AircraftWeight').val(data);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function getAircraftIsAirconditioned(aircraftType) {
    $.ajax({
        url: '/PlanFlight/IsAirconditioned',
        data: { aircraftType: aircraftType },
        type: 'GET',
        success: function (data) {
            if (data == true) {
                $('#AirconOption').val(1);
                $('#AirconOption').prop('disabled', false);
            }
            else {
                $('#AirconOption').val(0);
                $('#AirconOption').prop('disabled', true);
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}