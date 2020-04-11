var response_survey_module = angular.module("response_survey", ["chieffancypants.loadingBar"]);
response_survey_module.controller("response_survey_controller", ["$http", "$scope", "$sce",
function (e, s, r) {
    s.res_survey = response_survey,
    s.absoluteIdIndexMap = {},
    angular.forEach(s.res_survey.question,
    function (e, r) {
        e.type_id = parseInt(e.type_id, 10),
        e.vote_type_id = parseInt(e.vote_type_id, 10),
        5 === e.vote_type_id && (e.contentArr = e.content.split("#**#")),
        (14 === e.type_id || 15 === e.type_id) && angular.forEach(e.choice,
        function (e) {
            e.contentArr = e.content.split("#**#")
        }),
        s.absoluteIdIndexMap[e.id] = r
    }),
    s.trustAsHtml = function (e) {
        return r.trustAsHtml(e)
    },
    angular.forEach(s.res_survey.question,
    function (e) {
        parseInt(e.choice_quote, 10) > 0 && angular.forEach(s.res_survey.question[s.absoluteIdIndexMap[e.choice_quote]].choice,
        function (s, r) {
            s.selected === !1 && (e.choice[r].hide = 1)
        })
    }),
    s.info_user = "答卷人：" + response_survey.info.user,
    s.info_time = "提交时间：" + response_survey.info.time,
    s.prevResponse = function () {
        var r = basePath + "api/Survey/getSurveyResponsePrev?res_id=" + response_survey.info.id + "&survey_id=" + response_survey.info.survey_id + "&status=2&res_order=" + response_survey.info.res_order;
        e.get(r).success(function (e) {
            e && e.info && e.question && (response_survey = e, s.res_survey = e, angular.forEach(s.res_survey.question,
    function (e, r) {
        e.type_id = parseInt(e.type_id, 10),
        e.vote_type_id = parseInt(e.vote_type_id, 10),
        5 === e.vote_type_id && (e.contentArr = e.content.split("#**#")),
        (14 === e.type_id || 15 === e.type_id) && angular.forEach(e.choice,
        function (e) {
            e.contentArr = e.content.split("#**#")
        }),
        s.absoluteIdIndexMap[e.id] = r
    }),
    s.trustAsHtml = function (e) {
        return r.trustAsHtml(e)
    },
    angular.forEach(s.res_survey.question,
    function (e) {
        parseInt(e.choice_quote, 10) > 0 && angular.forEach(s.res_survey.question[s.absoluteIdIndexMap[e.choice_quote]].choice,
        function (s, r) {
            s.selected === !1 && (e.choice[r].hide = 1)
        })
    }), s.info_user = "答卷人：" + e.info.user, s.info_time = "提交时间：" + e.info.time)
        })
    },
    s.nextResponse = function () {
        var r = basePath + "api/Survey/getSurveyResponseNext?res_id=" + response_survey.info.id + "&survey_id=" + response_survey.info.survey_id + "&status=2&res_order=" + response_survey.info.res_order;
        e.get(r).success(function (e) {
            e && e.info && e.question && (response_survey = e, s.res_survey = e, angular.forEach(s.res_survey.question,
    function (e, r) {
        e.type_id = parseInt(e.type_id, 10),
        e.vote_type_id = parseInt(e.vote_type_id, 10),
        5 === e.vote_type_id && (e.contentArr = e.content.split("#**#")),
        (14 === e.type_id || 15 === e.type_id) && angular.forEach(e.choice,
        function (e) {
            e.contentArr = e.content.split("#**#")
        }),
        s.absoluteIdIndexMap[e.id] = r
    }),
    s.trustAsHtml = function (e) {
        return r.trustAsHtml(e)
    },
    angular.forEach(s.res_survey.question,
    function (e) {
        parseInt(e.choice_quote, 10) > 0 && angular.forEach(s.res_survey.question[s.absoluteIdIndexMap[e.choice_quote]].choice,
        function (s, r) {
            s.selected === !1 && (e.choice[r].hide = 1)
        })
    }), s.info_user = "答卷人：" + e.info.user, s.info_time = "提交时间：" + e.info.time)
        })
    }
}]),
response_survey_module.filter("unsafe", ["$sce",
function (e) {
    return function (s) {
        return e.trustAsHtml(s)
    }
}]);