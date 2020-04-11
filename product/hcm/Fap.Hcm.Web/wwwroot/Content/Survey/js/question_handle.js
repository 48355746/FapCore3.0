define("home:static/js/survey/widget/question_handle.js",
function (e, t) {
    "use strict";
    var i = "<style>.handle-dialog{font-size:14px;font-family:微软雅黑;position:fixed;left:50%;top:10%;width:500px;min-height:220px;margin-left:-250px;background:#fff;border-radius:4px;border:1px solid #f6f9fe;z-index:1003;padding-bottom:30px}.handle-dialog-title{height:40px;line-height:40px;padding-left:16px;color:#2c4a93;background:#f1f5fd;border-bottom:1px solid #b0c0e7}.handle-dialog-content{margin:15px;color:#666;min-height: 80px;}.handle-dialog-button{text-align:center;height:20px;line-height:20px;padding-top:0px}.survey-mask{position:fixed;height:100%;width:100%;background:#aaa;z-index:1002;top:0;left:0;opacity:.5;filter:alpha(opacity:50)}.button{width:64px;height:30px;line-height:20px;border-radius:3px;background:#f1f5fd;border:1px solid #b0c0e7;color:#2c4a93;cursor:pointer}.button:hover{background:#f1f5fd;border:1px solid #6583cc;color:#2c4a93}.button-confirm{background:#249cfa;border:1px solid #249cfa;color:#fff}.button-confirm:hover{background:#48aefe;border:1px solid #48aefe;color:#fff}.close{font-size:18px;line-height:40px!important;font-weight:700;color:#666;float:right;padding-right:16px;cursor:pointer}</style>",
    n = '<div class=question-handle-dialog> <div class=handle-dialog> <div class=handle-dialog-title>题目设置<span class=close>&times;</span></div> <div class=handle-dialog-content></div> <div class=handle-dialog-button> <input type=button class="dialog-button-survey button-confirm" id=survey-confirm-y value="确定">&nbsp; <input type=button class="dialog-button-survey button-cancel" id=survey-confirm-n value="取消"> </div> </div> <div class=survey-mask></div></div>';
    $("body").append(i),
    t.show = function (e, t, i, o, c) {
        function l(e, t) {
            if (document.selection) {
                var i = document.selection.createRange();
                i.text = t
            } else if ("number" == typeof e.selectionStart && "number" == typeof e.selectionEnd) {
                var n = e.selectionStart,
                o = e.selectionEnd,
                c = n,
                l = e.value;
                e.value = l.substring(0, n) + t + l.substring(o, l.length),
                c += t.length,
                e.selectionStart = e.selectionEnd = c
            } else e.value += t
        }
        function s(e) {
            if (e) {
                var t = e.split(",");
                p.find(".exclusive-choice-select").val(t[0]),
                t.length > 1 && (t.splice(0, 1), $.each(t,
                function (e, t) {
                    a(t)
                }))
            }
        }
        function a(t) {
            d = '<li><label>选项<select class="exclusive-choice-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="exclusive-option" value="0">请选择选项</option>',
            $(e).parents(".topic-type-content").find(".choice").length > 0 && $(e).parents(".topic-type-content").find(".choice").each(function () {
                var e = $(this).find(".edit-area").html(),
                t = e.length > 15 ? e.substring(0, 15) + "..." : e;
                d += '<option class="exclusive-choice-option" value="' + $(this).attr("choice_absolute_id") + '">' + t + "</option>"
            }),
            d += '</select>与其他选项互斥<span class="exclusive-delete-button"></span></label></li>',
            p.find(".exclusive-choice-select").last().parent().parent().after(d),
            t && p.find(".exclusive-choice-select").last().val(t)
        }
        var r = $.Deferred();
        $("body").append(n);
        var p = $(".question-handle-dialog"),
        d = '<ul class="handle-area" question-order="' + i + '">';
        switch (t) {
            case "6":
                d += '<li><label><input type="checkbox" name="type-change" class="type-change" question-type="' + t + '"/>  变为多选题</label></li>',
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>',
                d += '<li style="margin-top: 20px"><label>选项引用 <select class="choice-quote-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="choice-quote-option" value="0">请选择来源题目</option>',
                $(e).parents(".topic-type-content").prevAll(".topic-type-content").length > 0 && $(e).parents(".topic-type-content").prevAll(".topic-type-content").each(function () {
                    if ("8" === $(this).find(".question-title").attr("type") && "Y" === $(this).find(".question-id").attr("question-required")) {
                        var e = $(this).find(".qs-content").html().length > 15 ? $(this).find(".qs-content").html().substring(0, 15) + "..." : $(this).find(".qs-content").html();
                        d += '<option class="choice-quote-option" value="' + $(this).find(".question-id").attr("absolute_id") + '">' + $(this).find(".question-id").html() + " " + e + "</option>"
                    }
                }),
                d += "</select>  的答案</label></li>";
                break;
            case "14":
                d += '<li><label><input type="checkbox" name="type-change" class="type-change" question-type="' + t + '"/>  变为图片多选题</label></li>',
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>';
                break;
            case "8":
                d += '<li><label><input type="checkbox" name="type-change" class="type-change" question-type="' + t + '"/>  变为单选题</label></li>',
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>',
                d += '<li style="margin-top: 15px"><label>最少选<input type="text" name="min-checkbox" class="min-checkbox">项&nbsp;&nbsp;最多选<input type="text" name="max-checkbox" class="max-checkbox">项<label></li>',
                d += '<li style="margin-top: 15px"><label>选项互斥：选中某个选项，则其他选择均取消。<label></li>',
                d += '<li><label>选项<select class="exclusive-choice-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="exclusive-option" value="0">请选择选项</option>',
                $(e).parents(".topic-type-content").find(".choice").length > 0 && $(e).parents(".topic-type-content").find(".choice").each(function () {
                    var e = $(this).find(".edit-area").html(),
                    t = e.length > 15 ? e.substring(0, 15) + "..." : e;
                    d += '<option class="exclusive-choice-option" value="' + $(this).attr("choice_absolute_id") + '">' + t + "</option>"
                }),
                d += '</select>与其他选项互斥<span class="exclusive-add-button"></span></label></li>',
                d += '<li style="margin-top: 20px"><label>选项引用 <select class="choice-quote-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="choice-quote-option" value="0">请选择来源题目</option>',
                $(e).parents(".topic-type-content").prevAll(".topic-type-content").length > 0 && $(e).parents(".topic-type-content").prevAll(".topic-type-content").each(function () {
                    if ("8" === $(this).find(".question-title").attr("type") && "Y" === $(this).find(".question-id").attr("question-required")) {
                        var e = $(this).find(".qs-content").html().length > 15 ? $(this).find(".qs-content").html().substring(0, 15) + "..." : $(this).find(".qs-content").html();
                        d += '<option class="choice-quote-option" value="' + $(this).find(".question-id").attr("absolute_id") + '">' + $(this).find(".question-id").html() + " " + e + "</option>"
                    }
                }),
                d += "</select>  的答案</label></li>";
                break;
            case "15":
                d += '<li><label><input type="checkbox" name="type-change" class="type-change" question-type="' + t + '"/>  变为图片单选题</label></li>',
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>',
                d += '<li style="margin-top: 15px"><label>最少选<input type="text" name="min-checkbox" class="min-checkbox">项&nbsp;&nbsp;最多选<input type="text" name="max-checkbox" class="max-checkbox">项<label></li>';
                break;
            case "7":
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>',
                d += '<li style="margin-top: 20px"><label>选项引用 <select class="choice-quote-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="choice-quote-option" value="0">请选择来源题目</option>',
                $(e).parents(".topic-type-content").prevAll(".topic-type-content").length > 0 && $(e).parents(".topic-type-content").prevAll(".topic-type-content").each(function () {
                    if ("8" === $(this).find(".question-title").attr("type") && "Y" === $(this).find(".question-id").attr("question-required")) {
                        var e = $(this).find(".qs-content").html().length > 15 ? $(this).find(".qs-content").html().substring(0, 15) + "..." : $(this).find(".qs-content").html();
                        d += '<option class="choice-quote-option" value="' + $(this).find(".question-id").attr("absolute_id") + '">' + $(this).find(".question-id").html() + " " + e + "</option>"
                    }
                }),
                d += "</select>  的答案</label></li>";
                break;
            default:
                d += '<li><label><input type="checkbox" name="question-required" class="question-required no-required"/>  该题可跳过不答</label></li>'
        }
        if (d += '<li style="margin-top: 20px"><label>标题引用 <select class="title-quote-select" style="height: 30px;line-height: 30px;font-size: 12px;margin: 0 5px;"><option class="title-quote-option" value="0">请选择来源题目</option>', $(e).parents(".topic-type-content").prevAll(".topic-type-content").length > 0 && $(e).parents(".topic-type-content").prevAll(".topic-type-content").each(function () {
            if (inArray($(this).find(".question-title").attr("type"), ["1", "2", "6", "7", "8"])) {
                var e = $(this).find(".qs-content").html().length > 15 ? $(this).find(".qs-content").html().substring(0, 15) + "..." : $(this).find(".qs-content").html();
                d += '<option class="title-quote-option" value=' + $(this).find(".question-id").attr("index") + ">" + $(this).find(".question-id").html() + " " + e + "</option>"
        }
        }), d += '</select>  的答案<span class="title-quote-button">引用</span></label></li><textarea class="title-quote-text" id="title-quote-text" style="width: 436px;resize: none;height: 94px;margin-top: 5px">', d += $(e).parents(".topic-type-content").find(".qs-content").html(), d += "</textarea>", d += '</ul><div style="margin-left: 16px"><span class="required handle-area-error"></span></div>', p.find(".handle-dialog-content").html(d), "Y" == $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("question-required") ? $(".question-required").attr("checked", !1) : $(".question-required").attr("checked", !0), "8" == t || "15" == t) {
            var u = $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("min");
            "" !== u && "undefined" !== u && null !== u && void 0 !== u && $(".min-checkbox").val(u);
            var h = $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("max");
            if ("" !== h && "undefined" !== h && null !== h && void 0 !== h && $(".max-checkbox").val(h), "8" == t) {
                var f = $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("exclusive-options");
                "" !== f && "undefined" !== f && null !== f && void 0 !== f && s(f)
            }
        }
        if ("6" == t || "7" == t || "8" == t) {
            var q = $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("choice-quote");
            "" !== q && "undefined" !== q && null !== q && void 0 !== q && p.find(".choice-quote-select").val(q)
        }
        var b = getQuoteIndex($(e).parents(".topic-type-content").find(".qs-content").html());
        return b && p.find(".title-quote-select").val(b[0]),
        "Confirm" != o && (null === o ? p.find("#survey-confirm-y").hide() : p.find("#survey-confirm-y").val(o)),
        "Cancel" != c && (null === c ? p.find("#survey-confirm-n").hide() : p.find("#survey-confirm-n").val(c)),
        p.find("#survey-confirm-y").click(function () {
            var n = i - 1,
            o = !0,
            c = $(".min-checkbox").val(),
            l = $(".max-checkbox").val();
            if ($(".topic-type-content:eq(" + n + ")").find(".question-title").find(".question-id").attr({
                min: ""
            }), $(".topic-type-content:eq(" + n + ")").find(".question-title").find(".question-id").attr({
                max: ""
            }), "8" === t || "15" == t) {
                ($(".question-required").is(":checked") && "" !== c || $(".question-required").is(":checked") && "" !== l) && ($(".handle-area-error").html("该题是非必答题，不应设置最多最少选项"), o = !1),
                    "" !== c && (isNaN(c) ? ($(".handle-area-error").html("请填写数字"), o = !1) : 1 > c && ($(".handle-area-error").html("不能填写0或负数"), o = !1), o && $(".topic-type-content:eq(" + n + ")").find(".question-title").find(".question-id").attr({
                        min: $(".min-checkbox").val()
                    }));
                var s = $(".topic-type-content:eq(" + n + ")").find(".choice").length;
                "" !== l && (isNaN(l) ? ($(".handle-area-error").html("请填写数字"), o = !1) : 1 > l ? ($(".handle-area-error").html("不能填写0或负数"), o = !1) : l > s ? ($(".handle-area-error").html("最多选择数不能超过选项数量"), o = !1) : l < $(".min-checkbox").val() && ($(".handle-area-error").html("最多选择数不能小于最少选择数量"), o = !1), o && $(".topic-type-content:eq(" + n + ")").find(".question-title").find(".question-id").attr({
                    max: $(".max-checkbox").val()
                }))
            }
            var a = getQuoteIndex($("#title-quote-text").val());
            if (a && (checkIndexes(a) || ($(".handle-area-error").html("不能引用矩阵题"), o = !1)), o) {
                if ($(".type-change").is(":checked")) switch (t) {
                    case "6":
                        $(".topic-type-content:eq(" + n + ")").find(".question-title").attr({
                            type: "8",
                            name: "checkbox-question"
                        }),
                        $(".topic-type-content:eq(" + n + ")").find(".choice").each(function () {
                            $(this).find('input[name="radio"]').attr({
                                type: "checkbox",
                                name: "checkbox"
                            })
                        });
                        break;
                    case "14":
                        $(".topic-type-content:eq(" + n + ")").find(".question-title").attr({
                            type: "15",
                            name: "checkbox-question"
                        }),
                        $(".topic-type-content:eq(" + n + ")").find(".choice").each(function () {
                            var e = $(this).attr("class"),
                            t = e.replace(/radio/g, "checkbox");
                            $(this).attr("class", t),
                            $(this).find('input[name="radio"]').attr({
                                type: "checkbox",
                                name: "checkbox"
                            }),
                            $(this).find("[class*='radio']").each(function () {
                                var e = $(this).attr("class"),
                                t = e.replace(/radio/g, "checkbox");
                                $(this).attr("class", t)
                            })
                        });
                        break;
                    case "8":
                        $(".topic-type-content:eq(" + n + ")").find(".question-title").attr({
                            type: "6",
                            name: "radio-question"
                        }),
                        $(".topic-type-content:eq(" + n + ")").find(".exclusive-option-tip").remove(),
                        $(".topic-type-content:eq(" + n + ")").find(".choice").each(function () {
                            $(this).find('input[name="checkbox"]').attr({
                                type: "radio",
                                name: "radio"
                            })
                        });
                        break;
                    case "15":
                        $(".topic-type-content:eq(" + n + ")").find(".question-title").attr({
                            type: "14",
                            name: "radio-question"
                        }),
                        $(".topic-type-content:eq(" + n + ")").find(".exclusive-option-tip").remove(),
                        $(".topic-type-content:eq(" + n + ")").find(".choice").each(function () {
                            var e = $(this).attr("class"),
                            t = e.replace(/checkbox/g, "radio");
                            $(this).attr("class", t),
                            $(this).find('input[name="checkbox"]').attr({
                                type: "radio",
                                name: "radio"
                            }),
                            $(this).find("[class*='checkbox']").each(function () {
                                var e = $(this).attr("class"),
                                t = e.replace(/checkbox/g, "radio");
                                $(this).attr("class", t)
                            })
                        })
                }
                $(".question-required").is(":checked") ? ($(".topic-type-content:eq(" + n + ")").find(".question-id").attr("question-required", "N"), $(".topic-type-content:eq(" + n + ")").find(".required").remove()) : ($(".topic-type-content:eq(" + n + ")").find(".question-id").attr("question-required", "Y"), 0 == $(".topic-type-content:eq(" + n + ")").find(".required").length && $(".topic-type-content:eq(" + n + ")").find(".question-id").before('<span class="required">*</span>')),
                "0" !== $(".choice-quote-select").val() ? $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("choice-quote", $(".choice-quote-select").val()) : $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("choice-quote", "0"),
                a ? ($(e).parents(".topic-type-content").find(".qs-content").html($("#title-quote-text").val()), $(".topic-type-content:eq(" + n + ")").find(".question-id").attr("title-quote", "Y")) : $(".topic-type-content:eq(" + n + ")").find(".question-id").attr("title-quote", "N");
                var d = [];
                $(".exclusive-choice-select").each(function () {
                    var e = $(this).val();
                    "0" !== e && -1 == $.inArray(e, d) && d.push(e)
                }),
                d.length > 0 ? $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("exclusive-options", d.join(",")) : $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("exclusive-options", ""),
                edit.updateQuestionType(),
                edit.showExclusiveOptions(),
                edit.updateByChoiceQuote(),
                edit.saveSurvey(),
                p.remove(),
                r.resolve()
            }
        }),
        p.find(".title-quote-button").click(function () {
            var e = p.find(".title-quote-select").val();
            0 !== parseInt(e, 10) && l(document.getElementById("title-quote-text"), "[Q" + e + "]")
        }),
        p.on("click", ".exclusive-delete-button",
        function () {
            $(this).parent().parent().remove()
        }),
        p.find(".exclusive-add-button").click(function () {
            a()
        }),
        p.find("#survey-confirm-n").click(function () {
            p.remove(),
            r.reject()
        }),
        p.find(".close").click(function () {
            p.remove(),
            r.reject()
        }),
        r.promise()
    }
});