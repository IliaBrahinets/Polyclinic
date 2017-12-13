SetDeleteListener();

$("#addStreetField").click(function () {

    var streetField = $("#CreateRegionForm .streetField").first();

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

        StreetField.remove();
    });

}

//to pass a collection of streets
$("#CreateRegionForm").submit(function () {
    event.preventDefault();

    var data = $(this).serializeArray();

    var SendObj = {};
    //-1 because of a RequestToken
    for (var i = 0; i < data.length - 1; i += 2) {

        var SendObjNumb = i / 2;

        SendObj["[" + SendObjNumb + "]." + data[i].name] = data[i].value;
        SendObj["[" + SendObjNumb + "]." + data[i + 1].name] = data[i + 1].value;
    }

    SendObj[data[data.length - 1].name] = data[data.length - 1].value;

    $.ajax("/Registrator/CreateRegion",
        {
            method: "POST", 
            url: "/Registrator/CreateRegion",
            data: SendObj,
            success: function () {
                location.href = "/Registrator/Regions"
            }
        });
});


function googleMapsAutoCompleteInit(placeSearch) {

    try {

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
    } catch (RerenceError) {
        
        console.error("probably google is not defined!");
    }
}

googleMapsAutoCompleteInit(document.getElementsByClassName("streetInput")[0]);
