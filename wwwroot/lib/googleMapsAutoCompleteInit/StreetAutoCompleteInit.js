

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

