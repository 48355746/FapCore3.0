var surveyView = angular.module("surveyView", []);
surveyView.controller("surveyViewController", ["$scope", "$sce", "$timeout",
function (e, t, i) {
    function o() {
        var t = [];
        //return 1 === parseInt(e.surveyTemplate.ip_filter, 10) && t.push("同IP限答一次"),
        //1 === parseInt(e.surveyTemplate.telephone_filter, 10) && t.push("同用户限答一次"),
        //t
    }
    var n = function (t) {
        e.tab = t,
            $(".tab").removeClass("tab_active"),
            $(".navigation-tab li:eq(" + (t - 1) + ")").addClass("tab_active"),
            project.status >= 4 ?           
                (e.survey_url = surveyLink, $("#survey-url").val(surveyLink), $("#survey_qr_code").empty(), $("#survey_qr_code").qrcode({
                render: "canvas",
                width: 120,
                height: 120,
                text: surveyLink
            })) : (e.survey_url = "问卷发布后显示地址", $("#survey-url").addClass("survey-url-disabled"))
    };
    //e.validSurveyCount = "已收集有效问卷：" + valid_survey_count + "份",
    //e.valid_survey_count = valid_survey_count,
    e.template = template,
    e.pageCount = 1,
    //e.base_questions = base_questions,
    e.scene = scene,
    e.surveyType = surveyType,
    e.loaded = 0,
    e.filterUserType = filterUserType,
    //e.filterUserName = {
    //    0: "全部样本",
    //    1: "按人口属性",
    //    2: ""
    //},
    e.spssShowName = {
        0: "否",
        1: "是"
    },
    //e.userAttrConfig = userAttrConfig,
    //e.surveyTemplate = surveyTemplate,
    e.qualityArr = o(),
    e.absluteIdOrderMap = {},
    1 == tabIndex || 2 == tabIndex ? n(tabIndex) : e.tab = 1;
    for (var r = 0; r < additional_questions.length; r++) {
        for (var a = 0,
        l = 0; l < additional_questions[r].choice.length; l++) additional_questions[r].choice[l].selected > 0 && a++;
        additional_questions[r].selected = a
    }
    e.additional_questions = additional_questions,
    e.additional_questions_length = additional_questions.length,
    angular.forEach(e.template,
    function (t) {
        "11" == t.type_id && e.pageCount++
    }),
    //e.collection_target = collection_target,
    //e.logic_condition_relation = logic_condition && logic_condition.logic_condition_relation ? logic_condition.logic_condition_relation : {},
    //e.redirect_relation = redirect_relation ? redirect_relation : {},
    //e.poster = t.trustAsHtml(project.poster ? '<img class="project-img" src="' + CA.baseUrlModule + "file/showImage/fileid/" + project.poster + '">' : '<img class="project-img" src="/static/home/img/survey/question.jpg">'),
    //e.pro_name = project.pro_name,
    //e.test_content = t.trustAsHtml(project.test_content);
    //var c = function (e, t, i) {
    //    var o, n;
    //    return 1 == i ? 647 >= e && 164 >= t ? (o = t, n = e) : 647 >= e ? (o = 164, n = parseFloat(e / t) * o, n > 647 && (n = 647, o = parseFloat(t / e) * n)) : 164 >= t ? (n = 647, o = parseFloat(t / e) * n, o > 164 && (o = 164, n = parseFloat(e / t) * o)) : (n = 647, o = parseFloat(t / e) * n, o > 164 && (o = 164, n = parseFloat(e / t) * o)) : 340 >= e && 600 >= t ? (o = t, n = e) : 340 >= e ? (o = 600, n = parseFloat(e / t) * o, n > 340 && (n = 340, o = parseFloat(t / e) * n)) : 600 >= t ? (n = 340, o = parseFloat(t / e) * n, o > 600 && (o = 600, n = parseFloat(e / t) * o)) : (n = 340, o = parseFloat(t / e) * n, o > 600 && (o = 600, n = parseFloat(e / t) * o)),
    //    {
    //        width: n,
    //        height: o
    //    }
    //};
    angular.forEach(e.template,
    function (o) {
        if (o.vote_type_id = parseInt(o.vote_type_id, 10), "11" == o.type_id) o.content = t.trustAsHtml("页码：" + o.page + "/" + e.pageCount);
        else {
            if (5 != o.vote_type_id ? ("Y" == o.required ? o.content = '<span class="important_spns">*</span>Q' + o.index + "&nbsp" + o.content : "11" != o.type_id && "10" != o.type_id && (o.content = "Q" + o.index + "&nbsp" + o.content), o.content = t.trustAsHtml(o.content)) : o.content = o.index + ". " + o.content, 8 == o.vote_type_id) {
                var n = 0;
                o.vertical = 0;
                var r = 0;
                e.loaded = 0,
                angular.forEach(o.choice,
                function (t) {
                    var a = new Image;
                    a.src = t.content,
                    a.onload = function () {
                        r++,
                        a.width > a.height && (i(function () {
                            o.vertical = 1
                        }), n = 1),
                        t.imgHeight = a.height,
                        t.imgWidth = a.width,
                        r == o.choice.length && (angular.forEach(o.choice,
                        function (e) {
                            var t = c(e.imgWidth, e.imgHeight, n);
                            e.content = "<img src='" + e.content + "' style='width:" + t.width + "px;height:" + t.height + "px;'/>"
                        }), i(function () {
                            e.loaded = 1
                        }))
                    }
                })
            } else i(function () {
                e.loaded = 1
            });
            o.choice_quote = parseInt(o.choice_quote, 10) > 0 ? parseInt(o.choice_quote, 10) : 0,
            e.absluteIdOrderMap[o.absolute_id] = o.order
        } ("8" == o.type_id || "6" == o.type_id || "14" == o.type_id || "15" == o.type_id) && angular.forEach(o.choice,
        function (t) {
            if (e.logic_condition_relation && e.logic_condition_relation[o.absolute_id] && e.logic_condition_relation[o.absolute_id][t.choice_absolute_id]) {
                var i = "显示",
                n = [];
                $.each(e.logic_condition_relation[o.absolute_id][t.choice_absolute_id],
                function (t, i) {
                    for (var o in e.template) if (e.template[o].absolute_id == t && i > 0) {
                        n.push(e.template[o].index);
                        break
                    }
                }),
                n.sort(),
                $.each(n,
                function (e, t) {
                    i += e === n.length - 1 ? "Q" + t : "Q" + t + ",&nbsp"
                }),
                t.logic_show = i
            } else t.logic_show = ""
        }),
        e.redirect_relation && e.redirect_relation[o.absolute_id] && (e.redirect_relation[o.absolute_id][0] ? $.each(e.redirect_relation[o.absolute_id][0],
        function (t, i) {
            switch (parseInt(t, 10)) {
                case -1: o.logic_show = "选中/填写任意项,提前结束(计入结果)";
                    break;
                case -2: o.logic_show = "选中/填写任意项,提前结束(不计入结果)";
                    break;
                default:
                    for (var n in e.template) e.template[n].absolute_id == t && i > 0 && (o.logic_show = "选中/填写任意项,跳转至 Q" + e.template[n].index)
            }
        }) : angular.forEach(o.choice,
        function (t) {
            e.redirect_relation[o.absolute_id][t.choice_absolute_id] && $.each(e.redirect_relation[o.absolute_id][t.choice_absolute_id],
            function (i, o) {
                switch (parseInt(i, 10)) {
                    case -1: t.logic_show = "提前结束(计入结果)";
                        break;
                    case -2: t.logic_show = "提前结束(不计入结果)";
                        break;
                    default:
                        for (var n in e.template) e.template[n].absolute_id == i && o > 0 && (t.logic_show = "跳转至 Q" + e.template[n].index)
                }
            })
        })),
        5 == o.vote_type_id && (o.contentArr = o.content.split("#**#"), o.contentArr[0] += " ?"),
        (14 == o.type_id || 15 == o.type_id) && angular.forEach(o.choice,
        function (e) {
            e.contentArr = e.content.split("#**#")
        })
    }),
    e.selectTab = function (e) {
        n(e)
    },
    condition = {},
    filter && filter.filter_content ? (e.filter = filter.filter_content.content, e.filter.filter ? ($(".no-rule").remove(), angular.forEach(e.filter.filter,
    function (e, i) {
        e.r_index = "规则" + (i + 1),
        e.rule1_condition = e.rule[0].condition,
        e.rule1_content = t.trustAsHtml(e.rule[0].content),
        e.rule1_choice_content = t.trustAsHtml(e.rule[0].choice_content),
        2 == e.rule_type && (e.rule2_content = t.trustAsHtml(e.rule[1].content), e.rule2_condition = e.rule[1].condition, e.rule2_choice_content = t.trustAsHtml(e.rule[1].choice_content))
    })) : $(".filter-rule-title").append('<span class="no-rule">无</span>')) : $(".filter-rule-title").append('<span class="no-rule">无</span>'),
    e.trustAsHtml = function (e) {
        return t.trustAsHtml(e)
    },
    e.checkExclusive = function (e, t) {
        if (e.exclusive_options) {
            var i = e.exclusive_options.split(",");
            if (t.id = t.id ? t.id : t.choice_absolute_id, inArray(t.id.toString(), i)) return !0
        }
        return !1
    },
    e.getQuoteTip = function (t) {
        var i = "";
        if (t.choice_quote > 0) {
            var o = "";
            switch (t.type_id) {
                case "6":
                    o = "单选题";
                    break;
                case "7":
                    o = "下拉选择题";
                    break;
                case "8":
                    o = "多选题"
            }
            i = "&lt;" + o + "&gt此题目选项来自于Q" + e.absluteIdOrderMap[t.choice_quote] + "中的选项"
        }
        return i
    },
    e.copySurveyUrl = function () {
        var e = new Clipboard("#copy-survey-url-button");
        e.on("success",
        function (e) {
            $("#msg").remove(),
            $("<span id='msg' style='margin-left: 10px;color: #FF9900;'/>").insertAfter($("#copy-survey-url-button")).text("复制成功"),
            e.clearSelection()
        }),
        e.on("error",
        function () {
            $("#msg").remove(),
            $("<span id='msg' style='margin-left: 10px;color: #FF9900;'/>").insertAfter($("#copy-survey-url-button")).text("复制失败")
        })
    },
    e.questionTypeClass = function (e) {
        var t, i = e.vote_type_id;
        return 6 == i ? t = "input-type" : 7 == i ? t = "text-type" : 8 == i && (t = 1 == e.vertical ? "image-type vertical-type" : "image-type horizon-type"),
        t
    }
}]),
surveyView.filter("unsafe", ["$sce",
function (e) {
    return function (t) {
        return e.trustAsHtml(t)
    }
}]);