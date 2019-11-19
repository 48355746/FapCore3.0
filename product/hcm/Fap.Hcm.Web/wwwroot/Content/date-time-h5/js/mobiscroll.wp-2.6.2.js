(function ($) {

    var anim;

    $.mobiscroll.themes.wp = {
        defaults: {
            width: 70,
            height: 76,
            accent: 'none',
            dateOrder: 'mmMMddDDyy',
            showLabel: false,
            onAnimStart: function (dw, i, time) {
                $('.dwwl' + i, dw).addClass('wpam');
                clearTimeout(anim[i]);
                anim[i] = setTimeout(function () {
                    $('.dwwl' + i, dw).removeClass('wpam');
                }, time * 1000 + 100);
            }
        },
        init: function (elm, inst) {
            var click,
                active;

            anim = {};

            $('.dw', elm).addClass('wp-' + inst.settings.accent);

            //$('.dwwl', elm).on('touchstart mousedown DOMMouseScroll mousewheel', function () {
            $('.dwwl', elm).on('touchstart mousedown DOMMouseScroll mousewheel', '.dw-sel', function () {
                click = true;
                active = $(this).closest('.dwwl').hasClass('wpa');
                $('.dwwl', elm).removeClass('wpa');
                $(this).closest('.dwwl').addClass('wpa');
            }).on('touchmove mousemove', function () {
                click = false;
            }).on('touchend mouseup', function () {
                if (click && active) {
                    $(this).closest('.dwwl').removeClass('wpa');
                }
            });
        }
    };

    $.mobiscroll.themes['wp light'] = $.mobiscroll.themes.wp;

})(jQuery);


