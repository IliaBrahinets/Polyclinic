function googleMapsAutoCompleteInit(placeSearch) {

    var autocomplete = new google.maps.places.Autocomplete(placeSearch, {
        language: 'ru',
        componentRestrictions: { country: 'by' },
        types: ['geocode']
    });

    function geolocate(autocomplete) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var geolocation = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };
                var circle = new google.maps.Circle({
                    center: geolocation,
                    radius: position.coords.accuracy
                });
                autocomplete.setBounds(circle.getBounds());
            });
        }
    }

    geolocate(autocomplete);

    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();
        var street = place.address_components[0];

        console.log(place.address_components);

        if (street.types.indexOf("route") != -1)
            $(placeSearch).val(place.address_components[0]["long_name"]);
        else
            $(placeSearch).val('');
    });
}

googleMapsAutoCompleteInit(document.getElementsByClassName("streetInput")[0]);
SetDeleteListener();

$("#addStreetField").click(function () {

    var streetField = $("#CreateRegionForm .streetField").first();

    console.log(streetField);

    var newstreetField = $(streetField).clone();

    googleMapsAutoCompleteInit(newstreetField.find(".streetInput")[0]);

    $(newstreetField).appendTo(".streetFieldsContainer");

    //to work the delete button of the new streetField
    SetDeleteListener();
});

function SetDeleteListener() {

    $(".deleteStreetField").click(function () {

        var StreetField = $(this).closest(".streetField");

        if (StreetField.parent().children().length <= 1) return;

        console.log(StreetField);

        StreetField.remove();
    });

}

//to pass a collection of streets
$("#CreateRegionForm").submit(function () {
    event.preventDefault();

    var data = $(this).serializeArray();

    var SendObj = [];
    //-1 because of a ValidateToken
    for (var i = 0; i < data.length - 1; i+=2) {

        var curr = {};
        curr[data[i].name] = data[i].value;
        curr[data[i + 1].name] = data[i + 1].value;
        
        SendObj.push(curr);
    }

    $.ajax("/Registrator/CreateRegion",
        {
            method: "POST",
            url: "/Registrator/CreateRegion",
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(SendObj),
            success: function () {
                location.href = "/Registrator/Regions"
            }
        });
});
