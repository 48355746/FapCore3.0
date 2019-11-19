surveyModule.controller("SurveyTipController", ["$scope", "$sce", "$http", "$q",
function (t) {
    function e() {
        var e = "";
        switch (t.project.type) {
            case 1:
                e = "当前为预览状态，发布后才能投票";
                break;
            default:
                e = "问卷当前为编辑状态，填写答案不会记录系统，请勿将页面分享他人填答"
        }
        return 5 == t.project.status && (e = "项目已关闭，调研链接已失效！"),
        e
    }
    t.init = function (r) {
        t.project = r,
        t.tip = e()
    }
}]);