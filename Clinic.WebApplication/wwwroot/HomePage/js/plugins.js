// Avoid `console` errors in browsers that lack a console.
(function () {
    var method;
    var noop = function () {};
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeline', 'timelineEnd', 'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

// Place any jQuery/helper plugins in here.

$('.smoth-scroll a[href*="#"]:not([href="#"])').click(function () {
    if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
        var target = $(this.hash);
        target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
        if (target.length) {
            $('html, body').animate({
                scrollTop: target.offset().top
            }, 1000);
            return false;
        }
    }
});

(function ($) {
    $.fn.textareaCounter = function (options) {
        // setting the defaults
        // $("textarea").textareaCounter({ limit: 100 });
        var defaults = {
            limit: 100
        };
        var options = $.extend(defaults, options);

        // and the plugin begins
        return this.each(function () {
            var obj, text, wordcount, limited;

            obj = $(this);
            obj.after('<span style="font-size: 11px; clear: both; margin-top: 3px; display: block;float: right" id="counter-text">Max. ' + options.limit + ' words</span>');

            obj.keyup(function () {
                text = obj.val();
                if (text === "") {
                    wordcount = 0;
                } else {
                    wordcount = $.trim(text).split(" ").length;
                }
                if (wordcount > options.limit) {
                    $("#counter-text").html('<span style="color: #DD0000;">0 words left</span>');
                    limited = $.trim(text).split(" ", options.limit);
                    limited = limited.join(" ");
                    $(this).val(limited);
                } else {
                    $("#counter-text").html((options.limit - wordcount) + ' words left');
                }
            });
        });
    };
    $("textarea").textareaCounter();
})(jQuery);