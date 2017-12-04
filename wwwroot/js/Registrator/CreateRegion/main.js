var autocomplete = new google.maps.places.Autocomplete(document.getElementsByClassName("streetInput")[0], {
    language: 'ru',
    componentRestrictions: { country: 'by' },
    types:['geocode']
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