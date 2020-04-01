$(function () {

    function initMap() {

        var location = new google.maps.LatLng(-6.180841, 106.843210);

        var mapCanvas = document.getElementById('map');
        var mapOptions = {
            center: location,
            zoom: 10,
            panControl: false,
            scrollwheel: false,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        }
        var map = new google.maps.Map(mapCanvas, mapOptions);

        var markerImage = 'images/map-marker.png';

        var marker = new google.maps.Marker({
            position: location,
            map: map,
            icon: markerImage
        });

        var contentString = '<div class="info-window">' +
            '<div class="info-content">' +
            '<div class="space-15"></div><p class="text-center show"><img src="images/map-logo.png" alt=""> <strong>Healt Care</strong></p><hr><p class="padding-10"><strong>Sudirman Street #42 Building 213 <br> Jakarta, Indonesia, 40619</strong></p>' +
            '</div>' +
            '</div>';

        var infowindow = new google.maps.InfoWindow({
            content: contentString,
            maxWidth: 400
        });

        marker.addListener('click', function () {
            infowindow.open(map, marker);
        });

        var styles = [
            {
                "featureType": "landscape",
                "elementType": "geometry",
                "stylers": [
                    {
                        "saturation": "-100"
            }
        ]
    },
            {
                "featureType": "poi",
                "elementType": "labels",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "poi",
                "elementType": "labels.text.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "road",
                "elementType": "labels.text",
                "stylers": [
                    {
                        "color": "#545454"
            }
        ]
    },
            {
                "featureType": "road",
                "elementType": "labels.text.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "road.highway",
                "elementType": "geometry.fill",
                "stylers": [
                    {
                        "saturation": "-87"
            },
                    {
                        "lightness": "-40"
            },
                    {
                        "color": "#ffffff"
            }
        ]
    },
            {
                "featureType": "road.highway",
                "elementType": "geometry.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "road.highway.controlled_access",
                "elementType": "geometry.fill",
                "stylers": [
                    {
                        "color": "#f0f0f0"
            },
                    {
                        "saturation": "-22"
            },
                    {
                        "lightness": "-16"
            }
        ]
    },
            {
                "featureType": "road.highway.controlled_access",
                "elementType": "geometry.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "road.highway.controlled_access",
                "elementType": "labels.icon",
                "stylers": [
                    {
                        "visibility": "on"
            }
        ]
    },
            {
                "featureType": "road.arterial",
                "elementType": "geometry.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "road.local",
                "elementType": "geometry.stroke",
                "stylers": [
                    {
                        "visibility": "off"
            }
        ]
    },
            {
                "featureType": "water",
                "elementType": "geometry.fill",
                "stylers": [
                    {
                        "saturation": "-52"
            },
                    {
                        "hue": "#00e4ff"
            },
                    {
                        "lightness": "-16"
            }
        ]
    }
];

        map.set('styles', styles);


    }

    google.maps.event.addDomListener(window, 'load', initMap);
});
