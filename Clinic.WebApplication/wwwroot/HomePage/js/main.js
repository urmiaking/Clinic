(function ($) {

    "use strict";


    $(".carousel-inner .item:first-child").addClass("active");


    /* ------------------------------
    Mobile menu click then remove
    --------------------------------*/
    $(".mainmenu-area #mainmenu li a").on("click", function () {
        $(".navbar-collapse").removeClass("in");
    });


    /*---------------
    Scroll to top
    -----------------*/
    $.scrollUp({
        scrollText: '<i class="fa fa-angle-up"></i>',
        easingType: 'linear',
        scrollSpeed: 900,
        animation: 'fade'
    });


    /*---------------------------
    Team Slider Activation Code
    ----------------------------*/
    $('.team-member').owlCarousel({
        loop: true,
        margin: 0,
        responsiveClass: true,
        nav: true,
        autoplay: true,
        autoplayTimeout: 4000,
        smartSpeed: 1000,
        navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right" ></i>'],
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            }
        }
    });


    /*----------------------------------
    Testimonials Slider Activation Code
    -----------------------------------*/
    $('.testimonials').owlCarousel({
        loop: true,
        margin: 0,
        responsiveClass: true,
        nav: false,
        autoplay: true,
        autoplayTimeout: 4000,
        smartSpeed: 1000,
        navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right" ></i>'],
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 1
            },
            1000: {
                items: 1
            }
        }
    });


    /*--------------------------
    Blog Slider Activation Code
    ---------------------------*/
    $('.blog-post').owlCarousel({
        loop: true,
        margin: 0,
        responsiveClass: true,
        nav: true,
        autoplay: true,
        autoplayTimeout: 4000,
        smartSpeed: 1000,
        navText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right" ></i>'],
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            }
        }
    });
    // Revolution slider
    var revapi;
    revapi = jQuery('.tp-banner').revolution({
        delay: 9000,
        startwidth: 1170,
        startheight: 700,
        hideThumbs: 200,
        fullWidth: "on",
        forceFullWidth: "on"
    });

    $(window).on("load", function () {
        /*----------------------
        Preloader Fade Out For
        ------------------------*/
        $('.poreloader').fadeOut(500);
        /*----------------------
        Wow js activation code
        -----------------------*/
        new WOW().init({
            mobile: true,
        });
    });
})(jQuery);