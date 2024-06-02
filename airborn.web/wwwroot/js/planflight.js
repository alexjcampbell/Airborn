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
            var isError = data.isError;
            var stationId = data.stationId;
            var temperature = data.temperature;
            var windSpeed = data.windSpeed;
            var windDirection = data.windDirection;

            mixpanel.track("Weather data received", { "AirportCode": airportCode, "IsError": isError, "StationId": stationId, "Temperature": temperature, "WindSpeed": windSpeed, "WindDirection": windDirection });
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