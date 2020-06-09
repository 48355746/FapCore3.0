define("home:static/js/survey/widget/sortable_popup.js", function (e, i) {
    "use strict";
    var n = "<style>.survey-confirm{font-size:14px;font-family:微软雅黑;position:fixed;left:50%;top:40%;width:500px;height:220px;margin-left:-250px;background:#fff;border-radius:4px;border:1px solid #f6f9fe;z-index:1003}.survey-confirm-title{height:40px;line-height:40px;padding-left:16px;color:#2c4a93;background:#f1f5fd;border-bottom:1px solid #b0c0e7}.survey-confirm-content{margin:40px 0 30px;text-align:center;height:40px;line-height:40px;color:#666}.survey-confirm-button{text-align:center;height:20px;line-height:20px}.survey-mask{position:fixed;height:100%;width:100%;background:#aaa;z-index:1002;top:0;left:0;opacity:.5;filter:alpha(opacity:50)}.button-survey{width:64px!important;height:30px;line-height:26px;border-radius:3px;background:#f1f5fd;border:1px solid #b0c0e7;color:#2c4a93;cursor:pointer}.button-survey:hover{background:#f1f5fd;border:1px solid #6583cc;color:#2c4a93}.button-confirm{background:#249cfa;border:1px solid #249cfa;color:#fff}.button-confirm:hover{background:#48aefe;border:1px solid #48aefe;color:#fff}.close{font-size:18px;line-height:40px;font-weight:700;color:#666;float:right;padding-right:16px;cursor:pointer}.survey-confirm-content span{vertical-align:top;margin-left:10px}</style>"
        , o = '<div class="operate-popup">	<div class="survey-confirm"><div class="survey-confirm-title">操作提示<span class="close">&times;</span></div><div class="survey-confirm-content"><img src="/Content/Survey/images/warning_icon.png"><span></span></div><div class="survey-confirm-button"><input type="button" class="button-survey button-confirm" id="survey-confirm-y" value="确定">&nbsp;<input type="button" class="button-survey button-cancel" id="survey-confirm-n" value="取消"></div></div><div class="survey-mask"></div></div>';
    $("body").append(n),
        i.show = function (e, i, n) {
            var r = $.Deferred();
            $("body").append(o);
            var t = $(".operate-popup");
        return t.find(".survey-confirm-content img").attr("src", "/Content/Survey/images/warning_icon.png"),
                t.find(".survey-confirm-content span").text(e),
                "Confirm" != i && (null === i ? t.find("#survey-confirm-y").hide() : t.find("#survey-confirm-y").val(i)),
                "Cancel" != n && (null === n ? t.find("#survey-confirm-n").hide() : t.find("#survey-confirm-n").val(n)),
                t.find("#survey-confirm-y").click(function () {
                    t.remove(),
                        r.resolve()
                }),
                t.find("#survey-confirm-n").click(function () {
                    t.remove(),
                        r.reject()
                }),
                t.find(".close").click(function () {
                    t.remove(),
                        r.reject()
                }),
                r.promise()
        }
});