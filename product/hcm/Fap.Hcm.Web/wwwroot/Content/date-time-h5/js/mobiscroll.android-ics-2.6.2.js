(function ($) {
    var theme = {
        defaults: {
            dateOrder: 'Mddyy',
            mode: 'mixed',
            rows: 5,
            width: 70,
            height: 36,
            showLabel: false,
            useShortLabels: true
        }
    };

    $.mobiscroll.themes['android-ics'] = theme;
    $.mobiscroll.themes['android-ics light'] = theme;

})(jQuery);

