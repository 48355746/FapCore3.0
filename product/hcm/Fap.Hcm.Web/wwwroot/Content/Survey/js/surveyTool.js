var surveyTool = function () {
    function e(e, u, n, o) {
        $.post(basePath + "survey/home/createSurveyActionLog", {
            name: u,
            type: n,
            value: o,
            page_name: e,
            page_uuid: window.page_uuid
        },
        function () { })
    }
    return {
        createSurveyActionLog: e
    }
}();