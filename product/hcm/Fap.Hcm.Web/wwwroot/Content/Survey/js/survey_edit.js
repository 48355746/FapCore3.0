function checkUploadImgItem() {
    var e = !0;
    return $(".survey-question-upload-img-wrap").each(function () {
        var t = $(this).siblings(".question-choice").children().length;
        if (0 >= t) {
            var i = $(this).siblings(".question-title").find(".question-id").text();
            return showAlert(i + "请上传至少1张图片!"),
            $("html,body").animate({
                scrollTop: $(this).parents(".topic-type-content").offset().top - 8 + "px"
            },
            500),
            e = !1
        }
    }),
    e
}

//预览
function previewSurvey(fid) {
    edit.saveSurvey().then(function () {
        var e = basePath + "/System/Survey/Preview/"+fid;
        $("body").append('<a href="" id="goto_preview" target="_blank"></a>'),
        $("#goto_preview").attr("href", e),
        $("#goto_preview").get(0).click()
    })
}

function popSuggestionSelector(e, t) {
    require.async(["home:static/js/quit-task-dialog.js"],
    function (i) {
        i.show(e, t).then(function () {
            location.href = basePath + "/Survey/Survey/ManageSurvey"
        })
    })
}

//发布
//function publishSurvey(fid) {
//    $.get(basePath + "api/Survey/SubmitSurvey/" + fid, function (rv) {
//        if (rv.success) {
//            bootbox.alert("发布成功");
//        }
//    })
//}
function createQuitInfo(e, t) {
    var i = {};
    i.url = window.location.href,
    i.screenShotUrl = e,
    i.from = t;
    var o = basePath + "quitTask/createQuitInfo";
    $.post(o, i,
    function () {
        location.href = basePath + "/Survey/Survey/ManageSurvey"
    })
}
$(document).ready(function () {
    $(".survey-nav-tab").click(function () {
        var e = $(this).find(".survey-nav-icon").html();
        if ("2" === e && checkSurveyTitle(surveyTitle)) if ($(".topic-type-question").length > 0) {
            edit.saveSurvey(1);
            var t = 0 === parseInt(templateType.id, 10) ? "edit" : "edit" + templateType.id;
            surveyTool.createSurveyActionLog(t, "nextStep", "imgClick", survey_init.survey_id),
            location.href = basePath + "/Survey/Survey/SurveyFilter?survey_id=" + survey_init.survey_id
        } else require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (e) {
            e.show("请完成问卷信息编辑", "确定", null)
        })
    }),
    survey_init.content && $.each(survey_init.content,
    function (e, t) {
        $(".question-box-init").remove();
        var i = {};
        i.question_content = t,
        selectEditedQuestion(i)
    });
    var e = $.Deferred();
    $("body").on("blur", ".edit-title",
    function () {
        var e = $(this).html();
        e = $.trim(e);
        var t = getQuoteIndex(e);
        if (t) if (checkIndexes(t)) $(this).parent().parent().find(".question-id").attr("title-quote", "Y"),
        $(this).parent().parent().children().remove(".title-error-tip");
        else {
            0 == $(this).parent().parent().find(".title-error-tip").length && $(this).parent().parent().append("<div class='title-error-tip required' style='margin-top: 5px'>不能设置矩阵单选题，请删除</div>");
            var i = $(this);
            setTimeout(function () {
                i.focus()
            },
            100)
        } else $(this).parent().parent().find(".question-id").attr("title-quote", "N"),
        $(this).parent().parent().children().remove(".title-error-tip")
    }),
    $(".title-content").trigger("blur"),
    $(".title-content").focus(function () {
        e = $.Deferred(),
        $(".survey-title .error-tips").removeClass("error"),
        $(".survey-title .error-tips").text("")
    }),
    $(".common-questions ul").hide(),
    $(".common-questions-title, .select-question-title").click(function () {
        selectQuestionGroup($(this))
    }),
    $(".common-questions").on("click", ".common_question",
    function () {
        $(".question-box-init").remove();
        var e = $(this).attr("index");
        selectEditedQuestion(common_question[e]),
        edit.scrollBox(),
        setUpdateStatus(),
        elementInit()
    }),
    $("#question-box").sortable({
        connectWith: "#question-box",
        handle: ".drag-area",
        start: function (e, t) {
            var i = t.item,
                o = i.find(".question-id").attr("absolute_id");
            (edit.checkQuestionLogic(o) && edit.checkQuestionLogic(o).length>0) ? require.async(["home:static/js/survey/widget/sortable_popup.js"],
            function (e) {
                e.show("该题有关联的逻辑规则，移动题目会导致规则失效，确认移动？", "确定", "取消").then(function () {
                    edit.sortQuestions(),
                    edit.removeLogicCondition(o, i),
                    edit.saveSurvey()
                },
                function () {
                    $("#question-box").sortable("cancel"),
                    edit.sortQuestions()
                })
            }) : $("#question-box").sortable({
                stop: function () {
                    edit.sortQuestions(),
                    edit.updateChoiceShowLogic(),
                    edit.updateByChoiceQuote(),
                    edit.saveSurvey()
                }
            })
        }
    }),
    $(".module").on("click",
    function () {
        $(".question-box-init").remove();
        var e = $(this).attr("name");
        selectQuestion(e),
        setUpdateStatus()
    }),
    $(window).bind("scroll",
    function () {
        $(window).scrollTop() > 250 ? ($(".sur-sidebar").addClass("fixed-area"), $(window).scrollTop() + 150 > $("#edit-survey-content").height() ? $(".fixed-area").css("top", "-" + ($(window).scrollTop() + 150 - $("#edit-survey-content").height()) + "px") : $(".fixed-area").css("top", "0")) : $(".sur-sidebar").removeClass("fixed-area")
    }),
    $(".op-next").click(function () {
        if (checkSurveyTitle(surveyTitle)) if ($(".topic-type-question").length > 0) {
            edit.saveSurvey(1);
            var e = 0 === parseInt(templateType.id, 10) ? "edit" : "edit" + templateType.id;
            surveyTool.createSurveyActionLog(e, "nextStep", "buttonClick", survey_init.survey_id)
        } else require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (e) {
            e.show("请完成问卷信息编辑", "确定", null)
        })
    }),
    $(".op-prev").click(function () {
        if (checkSurveyTitle(surveyTitle)) if ($(".topic-type-question").length > 0) {
            edit.saveSurvey(1, 1);
            var e = 0 === parseInt(templateType.id, 10) ? "edit" : "edit" + templateType.id;
            surveyTool.createSurveyActionLog(e, "prevStep", "buttonClick", survey_init.survey_id)
        } else require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (e) {
            e.show("请完成问卷信息编辑", "确定", null)
        })
    }),
    $(".attach-layer, #survey-tail").click(function () {
        $(".choice .edit-area").css({
            display: "inline-block"
        }),
        $(".edit-img").hide(),
        $(".edit-area").css({
            background: "#fff",
            border: "none"
        }),
        $(".title-content").css({
            border: "1px solid #dbdbdb"
        }),
        t != $(".edit-area-active").html() && (t = $(".edit-area-active").html()),
        selectFinish()
    }),
    elementInit();
    var t = $(".edit-area-active").html();
    $(".edit-area").find("img").each(function () {
        new ImgEditSize($(this))
    }),
    $("#quit-edit-survey").click(function () {
        var e = "";
        html2canvas(document.body, {
            allowTaint: !0,
            taintTest: !1,
            onrendered: function (t) {
                t.id = "mycanvas",
                e = t.toDataURL(),
                popSuggestionSelector(e, "问卷调研")
            }
        })
    }),
    window.setInterval(function () {
        1 === updateStatus && (edit.saveSurvey(), updateStatus = 0)
    },
    5e3)
});
var checkSurveyTitle = function (e) {
    return "" === e ? (showAlert("请正确填写问卷名称"), $("html,body").animate({
        scrollTop: $(".title-content").offset().top - 50 + "px"
    },
    500), !1) : !0
},
saveSurvey = function (e, t) {
    if (t) {
        if (createLocked === !1) {
            createLocked = !0,
            $("#save-button").addClass("save-button-locked");
            var i = "欢迎参加调查！答卷数据仅用于统计分析，请放心填写。题目选项无对错之分，按照实际情况选择即可。感谢您的帮助！",
            o = 0,
            n = basePath + "home/createSurveyProject",
            r = {
                pro_name: t,
                test_content: i,
                poster: o,
                YII_CSRF_TOKEN: CA.yiiCsrfToken
            };
            $http.post(n, r).success(function (t) {
                0 === t.error_code ? window.location.href = e + t.survey_id : (createLocked = !1, showAlert("保存问卷不成功，请重新提交！"))
            })
        }
    } else showAlert("请正确填写问卷名!")
};