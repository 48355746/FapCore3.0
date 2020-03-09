define("home:static/js/survey/widget/choice_operate_dialog.js",
function (e, i) {
    "use strict";
    var t = "<style>.survey-choice-confirm{font-size:14px;font-family:微软雅黑;position:fixed;left:50%;top:40%;width:500px;height:220px;margin-left:-250px;background:#fff;border-radius:4px;border:1px solid #f6f9fe;z-index:1003}.survey-choice-confirm-title{height:40px;line-height:40px;padding-left:16px;color:#2c4a93;background:#f1f5fd;border-bottom:1px solid #b0c0e7}.survey-choice-confirm-content{margin:15px;color:#666;min-height:80px}.survey-choice-confirm-content ul{padding:0}.survey-choice-confirm-content ul li{list-style:none}.survey-choice-confirm-button{text-align:center;height:20px;line-height:20px}.survey-choice-mask{position:fixed;height:100%;width:100%;background:#aaa;z-index:1002;top:0;left:0;opacity:.5;filter:alpha(opacity:50)}.button{width:64px;height:30px;line-height:20px;border-radius:3px;background:#f1f5fd;border:1px solid #b0c0e7;color:#2c4a93;cursor:pointer}.button:hover{background:#f1f5fd;border:1px solid #6583cc;color:#2c4a93}.button-confirm{background:#249cfa;border:1px solid #249cfa;color:#fff}.button-confirm:hover{background:#48aefe;border:1px solid #48aefe;color:#fff}.close{font-size:18px;line-height:40px!important;font-weight:700;color:#666;float:right;padding-right:16px;cursor:pointer}.add-other{margin:0!important}</style>",
    r = '<div class=operate-popup><div class=survey-choice-confirm> <div class=survey-choice-confirm-title>选项设置<span class=close>&times;</span></div> <div class=survey-choice-confirm-content> <ul> <li> <label> <input type=checkbox name=add-other class=add-other> <span>&nbsp;选项后增加填空框</span> </label>  <li> <label> <input type=checkbox name=choice-no-required class=choice-no-required> <span>该选项可跳过不答</span> </label>  </ul> </div> <div class=survey-choice-confirm-button> <input type=button class="dialog-button-survey button-confirm" id=survey-choice-confirm-y value="确定">&nbsp; <input type=button class="dialog-button-survey button-cancel" id=survey-choice-confirm-n value="取消"> </div> </div> <div class=survey-choice-mask></div></div>';
    $("body").append(t),
    i.show = function (e, i) {
        var t = $.Deferred();
        $("body").append(r);
        var c, o, a = $(".operate-popup");
        return $(".edit-area-active").hasClass("matrix-choice") ? (c = $(".edit-area-active").attr("has_other"), o = $(".edit-area-active").attr("choice_required")) : (c = $(".edit-area-active").parents(".choice").attr("has_other"), o = $(".edit-area-active").parents(".choice").attr("choice_required")),
        $(".add-other").change(function () {
            $(".add-other").is(":checked") ? $(".choice-no-required").parents("li").show() : $(".choice-no-required").parents("li").hide(),
            $(".choice-no-required").prop("checked", !0)
        }),
        "Y" == c ? $(".add-other").prop("checked", !0) : ($(".add-other").prop("checked", !1), $(".choice-no-required").parents("li").hide()),
        0 === parseInt(o, 10) ? $(".choice-no-required").prop("checked", !0) : $(".choice-no-required").prop("checked", !1),
        "Confirm" != e && (null === e ? a.find("#survey-choice-confirm-y").hide() : a.find("#survey-choice-confirm-y").val(e)),
        "Cancel" != i && (null === i ? a.find("#survey-choice-confirm-n").hide() : a.find("#survey-choice-confirm-n").val(i)),
        a.find("#survey-choice-confirm-y").click(function () {
            var e;
            e = $(".edit-area-active").hasClass("matrix-choice") ? $(".edit-area-active") : $(".edit-area-active").parents(".choice");
            var i = "",
            r = '<input type="text" class="other-content"  style="width: 120px;height: 30px; vertical-align: middle;">';
            if ($(".add-other").is(":checked")) if (e.attr("has_other", "Y"), $(".choice-no-required").is(":checked") ? (i = "", e.attr("choice_required", 0)) : (i = '<span class="required-content">(必填)</span>', e.attr("choice_required", 1)), $(".edit-area-active").hasClass("matrix-choice")) {
                $(".edit-area-active").parents("td").find(".other-content").remove();
                var c = $(".edit-area-active").parents("td").index();
                $(".edit-area-active").parents("tr").siblings().each(function () {
                    $(this).find("td:eq(" + c + ")").find(".other-content").remove(),
                    $(this).find("td:eq(" + c + ")").find(".required-content").remove(),
                    $(this).find("td:eq(" + c + ")").append(r + i)
                })
            } else $(".edit-area-active").parents(".choice").find(".other-content").remove(),
            $(".edit-area-active").parents(".choice").find(".required-content").remove(),
            $(".edit-area-active").parents(".choice").find(".position-relative").after(r + i);
            else if (e.attr("has_other", "N"), e.attr("choice_required", 0), $(".edit-area-active").hasClass("matrix-choice")) {
                var c = $(".edit-area-active").parents("td").index();
                $(".edit-area-active").parents("tr").siblings().each(function () {
                    $(this).find("td:eq(" + c + ")").find(".other-content").remove(),
                    $(this).find("td:eq(" + c + ")").find(".required-content").remove()
                })
            } else $(".edit-area-active").parents(".choice").find(".other-content").remove(),
            $(".edit-area-active").parents(".choice").find(".required-content").remove();
            var o = 0;
            $(".edit-area-active").parents(".question-choice").find(".choice").each(function () {
                "Y" == $(this).attr("has_other") && (o += 1)
            }),
            o > 0 ? $(".edit-area-active").parents(".question-choice").siblings(".question-title").find(".question-id").attr("has_other", "Y") : $(".edit-area-active").parents(".question-choice").siblings(".question-title").find(".question-id").attr("has_other", "N"),
            edit.saveSurvey(),
            a.remove(),
            t.resolve()
        }),
        a.find("#survey-choice-confirm-n").click(function () {
            a.remove(),
            t.reject()
        }),
        a.find(".close").click(function () {
            a.remove(),
            t.reject()
        }),
        t.promise()
    }
});