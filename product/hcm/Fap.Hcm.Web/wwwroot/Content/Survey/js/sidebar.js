define("common:widget/sidebar/sidebar.js",
function(e, i) {
    i.init = $(function() {
        var e = {
            versions: function() {
                var e = navigator.userAgent,
                i = navigator.appVersion;
                return {
                    trident: e.indexOf("Trident") > -1,
                    presto: e.indexOf("Presto") > -1,
                    webkit: e.indexOf("AppleWebKit") > -1,
                    gecko: e.indexOf("Gecko") > -1 && -1 == e.indexOf("KHTML"),
                    mobile: !!e.match(/AppleWebKit.*Mobile.*/),
                    ios: !!e.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/),
                    android: e.indexOf("Android") > -1 || e.indexOf("Linux") > -1,
                    iphone: e.indexOf("iPhone") > -1,
                    mac: e.indexOf("Mac") > -1,
                    ipad: e.indexOf("iPad") > -1,
                    webapp: -1 == e.indexOf("Safari"),
                    weixin: e.indexOf("MicroMessenger") > -1,
                    baidubox: e.indexOf("baiduboxapp") > -1,
                    google: e.indexOf("Chrome") > -1,
                    version: i
                }
            } (),
            language: (navigator.browserLanguage || navigator.language).toLowerCase()
        }; (e.versions.mobile || e.versions.ios || e.versions.android || e.versions.iphone || e.versions.ipad || e.versions.weixin || e.versions.baidubox) && ($("#slideBar .item-kefu-link").attr("href", "https://ikefu.baidu.com/wise/mtc-cs"), $("#slideBar .item-kefu-link").attr("data-mtc-trace-content", "https://ikefu.baidu.com/wise/mtc-cs"));
        var i = $("#goTop");
        $(window).scroll(function() {
            $(this).scrollTop() > 200 ? (i.fadeIn(400), $("#feedback").css("border-bottom-left-radius", 0).css("border-bottom-right-radius", 0)) : (i.fadeOut(400), $("#feedback").css("border-bottom-left-radius", 3).css("border-bottom-right-radius", 3))
        }),
        i.click(function() {
            $("html,body").animate({
                scrollTop: 0
            },
            "normal", "swing")
        })//,
        //"mtc" === window.siteChannelCode && $.getJSON("getfun.php?op=getUnreadCount",
        //function(e) {
        //    e={"status":0,"unread":0};
        //    e && e.unread && ($("#feedback").append('<div id="cornerIcon"><div>' + e.unread + "<div></div>"), $("#feedback").off("click.noUnread").on("click.hasUnread",
        //    function() {
        //        //window.open("http://f3.baidu.com/index.php/feedback/f/index")
        //    }))
        //})
    })
});