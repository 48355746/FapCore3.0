define("home:static/js/survey/widget/edit_logic_pop.js",
function (i, e) {
    "use strict";
    var t = "<style>.logic-area{font-size:14px;font-family:微软雅黑;position:fixed;left:50%;top:10%;width:500px;height:450px;margin-left:-250px;background:#fff;border-radius:4px;border:1px solid #f6f9fe;z-index:1003}.logic-area-title{height:40px;line-height:40px;padding-left:16px;color:#2c4a93;background:#f1f5fd;border-bottom:1px solid #b0c0e7}.logic-area-content{margin:40px;color:#666;height:270px}.survey-mask{position:fixed;height:100%;width:100%;background:#aaa;z-index:1002;top:0;left:0;opacity:.5;filter:alpha(opacity:50)}.button{width:64px;height:30px;border-radius:3px;background:#f1f5fd;border:1px solid #b0c0e7;color:#2c4a93;cursor:pointer}.button:hover{background:#f1f5fd;border:1px solid #6583cc;color:#2c4a93}.button-confirm{background:#249cfa;border:1px solid #249cfa;color:#fff;text-align:center}.button-confirm:hover{background:#48aefe;border:1px solid #48aefe;color:#fff}.close{font-size:18px;line-height:40px!important;font-weight:700;color:#666;float:right;padding-right:16px;cursor:pointer}.logic-area-button{width:500px;height:30px;margin:30px 0 40px 0;text-align:center}.logic-area-condition{font-size: 12px;width:165px;height:240px;float:left;border:1px solid #b0c0e7;margin-right:5px;border-radius:4px;-webkit-border-radius:4px;-moz-border-radius:4px;padding:15px 0 15px 0;overflow-y:auto;overflow-x:hidden}.logic-area-question{font-size: 12px;width:230px;height:240px;float:left;border:1px solid #b0c0e7;border-radius:4px;-webkit-border-radius:4px;-moz-border-radius:4px;padding:15px 0 15px 15px;overflow-y:auto;overflow-x:hidden}.logic-area-condition-title{padding:0 0 0 10px}.condition-active{background-color:#dbdbdb}.condition-area{cursor:pointer}.logic-show-checkbox{margin-right:5px!important}.logic-show-question label{text-overflow:ellipsis;white-space:nowrap;overflow:hidden;width:215px;display:block}.condition_element{padding:0 0 0 15px;text-overflow:ellipsis;white-space:nowrap;overflow:hidden;display:block}</style>",
    o = '<div class=logic-popup><div class=logic-area> <div class=logic-area-title>逻辑设置<span class=close>&times;</span></div> <div class=logic-area-content> <div style="width:165px;margin-right:5px;float:left;"><span class="logic-area-condition-title">当前用户选择:</span><div class=logic-area-condition> <div class=condition-area> </div> </div></div> <div style="float:left;width:230px;padding-left:20px"><span class="logic-area-condition-title">则<span style="color: red;">跳转</span>以下题目</span> <div class=logic-area-question> <div class=logic-questions> </div> </div></div> </div> <div class=logic-area-button> <input type=button class="dialog-button-survey button-confirm" value="确定">&nbsp; <input type=button class="dialog-button-survey button-cancel" value="取消"> </div> </div> <div class=survey-mask></div></div>';
    $("body").append(t);
    var n = [{
        absolute_id: "-1",
        name: "提前结束(计入结果)"
    },
    {
        absolute_id: "-2",
        name: "提前结束(不计入结果)"
    }];
    e.show = function (i) {
        function e() {
            if ($(".condition-active").length > 0) {
                var e, t, o;
                e = parseInt($(".condition-active").attr("absolute_id")),
                t = parseInt($(".condition-active").attr("choice_absolute_id")),
                redirect_relation[e] || (redirect_relation[e] = {});
                var n = [],
                c = !1;
                0 === parseInt(t, 10) ? (n.push("0"), $(i).parents(".operate").siblings(".question-choice").find(".choice").each(function () {
                    n.push($(this).attr("choice_absolute_id"))
                }), c = !0) : n.push(t);
                var a = !1;
                $(".logic-show-question").each(function () {
                    $(this).find(".logic-show-checkbox").is(":checked") && (a = !0)
                }),
                $.each(n,
                function (i, t) {
                    if (c !== !0 || a !== !1 || 0 == t) {
                        var n = "";
                        $(".logic-show-question").each(function () {
                            if (o = parseInt($(this).attr("absolute_id")), redirect_relation[e][t] || (redirect_relation[e][t] = {}), $(this).find(".logic-show-checkbox").is(":checked")) {
                                redirect_relation[e][t] = {},
                                redirect_relation[e][t][o] = 1,
                                n = o > 0 ? '<span class="choice_show_logic_show_questions">跳转' : '<span class="choice_show_logic_show_questions">';
                                var i = $(this).find(".logic_show_question_index").html();
                                "" == i && (i = $(this).find(".logic_show_question_index").parent().text(), i = '<a class="choice_show_logic_show_questions_link" title="' + i + '">' + i + "</a>"),
                                n += i
                            } else redirect_relation[e][t] && redirect_relation[e][t][o] && delete redirect_relation[e][t][o],
                            $.isEmptyObject(redirect_relation[e][t]) && delete redirect_relation[e][t]
                        }),
                        n += "</span>",
                        $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find('[choice_absolute_id="' + t + '"]').find(".choice_show_logic_show_questions").remove(),
                        n && 7 != $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").attr("type") && $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find('[choice_absolute_id="' + t + '"]').find(".option-tips").append(n)
                    }
                })
            }
        }
        var t = $.Deferred();
        $("body").append(o);
        var c = $(".logic-popup"),
        a = "<ul> ",
        s = $(i).parents(".operate").siblings(".question-title").find(".question-id").attr("absolute_id"),
        l = $(i).parents(".operate").siblings(".question-title").attr("type");
        a += '<li class="condition_element" absolute_id=' + s + ' choice_absolute_id=0 title="任意选项">任意选项</li>',
        $(i).parents(".operate").siblings(".question-choice").find(".choice").each(function () {
            var i, e;
            "7" == l ? (i = $(this).text(), e = $(this).html()) : (i = $(this).find(".edit-area").text(), e = $(this).find(".edit-area").html()),
            "" == $.trim(i) && e.indexOf("img") >= 0 && (i = "【图片】"),
            a += '<li class="condition_element" absolute_id=' + s + " choice_absolute_id=" + $(this).attr("choice_absolute_id") + ' title="' + i + '">' + i + "</li>"
        }),
        a += "</ul>",
        c.find(".condition-area").html(a);
        var r = "<ul>";
        $(i).parents(".topic-type-content").nextAll(".topic-type-content").length > 0 && $(i).parents(".topic-type-content").nextAll(".topic-type-content").each(function () {
            if ("11" != $(this).find(".question-title").attr("type")) {
                var i, e;
                i = $(this).find(".edit-title").text(),
                e = $(this).find(".edit-title").html(),
                "" == $.trim(i) && e.indexOf("img") >= 0 && (i = "【图片】"),
                r += '<li class="logic-show-question"  absolute_id=' + $(this).find(".question-id").attr("absolute_id") + ' title="' + $(this).find(".question-id").text() + i + '"><label><input type="radio" name="redirect-radio" class="logic-show-checkbox logic-show-checkbox-' + $(this).find(".question-id").attr("absolute_id") + '" style="margin: 3px 0 0 1px;"><span class="logic_show_question_index">' + $(this).find(".question-id").text() + "</span>" + i + "</label></li>"
            }
        });
        for (var d in n) r += '<li class="logic-show-question"  absolute_id=' + n[d].absolute_id + ' title="' + n[d].name + '"><label><input type="radio" name="redirect-radio" class="logic-show-checkbox logic-show-checkbox-' + n[d].absolute_id + '" style="margin: 3px 0 0 1px;"><span class="logic_show_question_index"></span>' + n[d].name + "</label></li>";
        return r += '</ul><div class="dialog-button-survey cancel-redirect-button" style="text-align: center">取消选择</div>',
        c.find(".logic-questions").html(r),
        c.find(".button-confirm").click(function () {
            e(),
            edit.saveSurvey(),
            c.remove(),
            t.resolve()
        }),
        c.find(".button-cancel").click(function () {
            c.remove(),
            t.reject()
        }),
        c.find(".close").click(function () {
            c.remove(),
            t.reject()
        }),
        c.find(".cancel-redirect-button").click(function () {
            $(".logic-show-checkbox").prop("checked", !1)
        }),
        c.find(".condition_element").click(function () {
            e(),
            $(".logic-show-checkbox").attr("checked", !1),
            $(".condition_element").removeClass("condition-active"),
            $(this).addClass("condition-active");
            var i, t;
            i = parseInt($(".condition-active").attr("absolute_id")),
            t = parseInt($(".condition-active").attr("choice_absolute_id")),
            redirect_relation[i] && redirect_relation[i][t] && $.each(redirect_relation[i][t],
            function (i, e) {
                e > 0 && $(".logic-show-checkbox-" + i).prop("checked", !0)
            })
        }),
        $(".condition_element:eq(0)").trigger("click"),
        t.promise()
    }
});