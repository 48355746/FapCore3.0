define("home:static/js/survey/widget/operate_popup.js",
function (i, e) {
    "use strict";
    var n = "<style>.survey-confirm{font-size:14px;font-family:微软雅黑;position:fixed;left:50%;top:20%;width:500px;margin-left:-250px;background:#fff;border-radius:4px;border:1px solid #f6f9fe;z-index:1003}.survey-confirm-title{height:40px;line-height:40px;padding-left:16px;color:#2c4a93;background:#f1f5fd;border-bottom:1px solid #b0c0e7}.survey-confirm-content{margin:40px 40px 20px 40px;text-align:center;color:#666}.survey-confirm-button{text-align:center;height:20px;line-height:20px;margin-bottom:40px;font-size:0}.survey-mask{position:fixed;height:100%;width:100%;background:#aaa;z-index:1002;top:0;left:0;opacity:.5;filter:alpha(opacity:50)}.survey-confirm .button{width:64px;height:30px;line-height:26px;border-radius:3px;background:#f1f5fd;border:1px solid #b0c0e7;color:#2c4a93;cursor:pointer;font-size:12px;}.survey-confirm .button:hover{background:#f1f5fd;border:1px solid #6583cc;color:#2c4a93}.survey-confirm .button-confirm{background:#249cfa;border:1px solid #249cfa;color:#fff;margin-right:6px}.survey-confirm .button-confirm:hover{background:#48aefe;border:1px solid #48aefe;color:#fff}.close{font-size:18px;line-height:40px!important;font-weight:700;color:#666;float:right;padding-right:16px;cursor:pointer}.survey-confirm-content span{vertical-align:text-bottom}.survey-content-text{display:inline-block;vertical-align:middle;text-align:left;margin-left:16px;max-width:362px;}</style>",
    o = '<div class="operate-popup">  <div class="survey-confirm"><div class="survey-confirm-title">操作提示<span class="close">&times;</span></div><div class="survey-confirm-content"><img src="/Content/Survey/images/warning_icon.png" style="vertical-align: middle;"><div class="survey-content-text">确定进入下一步吗？</div></div><div class="survey-confirm-button"><input type="button" class="button button-confirm" id="survey-confirm-y" value="确定">&nbsp;<input type="button" class="button button-cancel" id="survey-confirm-n" value="取消"></div></div><div class="survey-mask"></div></div>';
    $("body").append(n),
    e.show = function (i, e, n) {
        var t = $.Deferred();
        $("body").append(o);
        var r = $(".operate-popup");
        return r.find(".survey-confirm-content div").html(i),
        r.find(".survey-confirm-content img").attr("src", "/Content/Survey/images/warning_icon.png"),
        "Confirm" != e && (null === e ? r.find("#survey-confirm-y").hide() : r.find("#survey-confirm-y").val(e)),
        "Cancel" != n && (null === n ? r.find("#survey-confirm-n").hide() : r.find("#survey-confirm-n").val(n)),
        r.find("#survey-confirm-y").click(function () {
            r.remove(),
            t.resolve()
        }),
        r.find("#survey-confirm-n").click(function () {
            r.remove(),
            t.reject()
        }),
        r.find(".close").click(function () {
            r.remove(),
            t.reject()
        }),
        t.promise()
    }
});