$(document).ready(function () { });
var surveyModule = angular.module("survey", []);
surveyModule.config(["$httpProvider",
function (e) {
    e.defaults.xsrfCookieName = "YII_CSRF_TOKEN",
    e.defaults.xsrfHeaderName = "X-YII-CSRF-TOKEN",
    e.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest"
}]),
surveyModule.controller("SurveyResponse", ["$http", "$scope", "$q",
function (e, t) {
    function a(e) {
        var t = basePath + "/System/Api/Survey/ExportUserReport?" + $.param(e);
        return angular.copy(t)
    }
    var r = {
        4: "发布中",
        5: "已关闭"
    },
    n = [];
    if (t.responseList = [], t.itemsPerPage = 10, t.currentPage = 0, t.itemCount = 0, t.emptyList = !0, t.surveyId = header.survey_id, t.surveyName = header.surveyName, t.onlineTime = header.onlineTime, t.status = header.status, t.target = parseInt(header.target, 10), t.type = parseInt(header.type, 10), t.valid = parseInt(header.valid, 10), 0 == t.type || 4 == t.type) {
        var s = {
            survey_id: t.surveyId,
            sheet: 1
        };
        t.exportUrl = a(s);
        var s = {
            survey_id: t.surveyId,
            sheet: 2
        };
        t.exportSpssUrl = a(s)
    } else t.exportUrl = basePath + "home/getSurveyResData/survey_id/" + t.surveyId + "/type/0/platform/1";
    t.downloadUrl = basePath + "home/getSurveyDataDownload/survey_id/" + t.surveyId,
    t.exportStatus = 0;
    var i = function () {
        var a = basePath + "/System/Api/Survey/Tester/"+t.surveyId; //location.href.replace(/userSurveyList/, "getResponseList");
        e.get(a).success(function (e) {
            if (0 === e.error_code) {
                t.count = parseInt(e.count, 10),
                t.target <= t.count && (t.status = 5),
                t.statusName = r[t.status],
                n = e.list;
                var a = 1;
                angular.forEach(n,
                function (e) {
                    e.index = a,
                    a += 1,
                        e.openUrl = basePath + "/System/Survey/TesterReview?resuid=" + e.res_id + "&suruid=" + t.surveyId + "&resorder=" + e.index
                }),
                t.responseList = n,
                t.itemCount = n.length,
                t.emptyList = !1
            }
        })
    };
    i(),
    t.exportData = function () {
        surveyTool.createSurveyActionLog("userSurveyList", "export", "buttonClick", t.surveyId),
        require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (r) {
            r.show("导出数据耗时较长，请【不要】跳转或关闭页面！", "知道啦", null).then(function () {
                //location.href = t.exportUrl
                e.get(t.exportUrl).success(function (e) {
                    0 === e.error_code ? (t.exportStatus = 1, $("#export-data").val("导出统计报告").removeAttr("disabled").removeClass("disabled"), location.href = basePath + "/" + e.fn) : require.async(["home:static/js/survey/widget/operate_popup.js"],
                    function (e) {
                        e.show("导出数据请求失败，请重新导出！", "确定", null)
                    })
                })
            })
        })
    },
    t.exportSpssData = function () {
        surveyTool.createSurveyActionLog("userSurveyList", "exportSpss", "buttonClick", t.surveyId),
        require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (r) {
            r.show("导出数据耗时较长，请【不要】跳转或关闭页面！", "知道啦", null).then(function () {
                //location.href = t.exportSpssUrl
                e.get(t.exportSpssUrl).success(function (e) {
                    0 === e.error_code ? (t.exportStatus = 1, $("#export-data").val("导出统计报告").removeAttr("disabled").removeClass("disabled"), location.href = basePath + "/" + e.fn) : require.async(["home:static/js/survey/widget/operate_popup.js"],
                    function (e) {
                        e.show("导出数据请求失败，请重新导出！", "确定", null)
                    })
                })
            })
        })
    }
}]),
surveyModule.filter("offset",
function () {
    return function (e, t) {
        return t = parseInt(t, 10),
        e.slice(t)
    }
}),
surveyModule.directive("surveyPagination",
function () {
    return {
        restrict: "EA",
        replace: !0,
        templateUrl: "pagination.html",
        link: function (e) {
            e.pages = [0],
            e.setPage = function (t) {
                e.currentPage = t,
                e.pages = [];
                var a = e.pageCount(),
                r = t - 2 - (t + 3 > a ? t + 3 - a : 0);
                0 > r && (r = 0);
                var n = r + 4;
                n >= a && (n = a - 1);
                for (var s = r; n >= s; s += 1) e.pages.push(s)
            },
            e.prevPage = function () {
                e.currentPage > 0 && e.setPage(e.currentPage - 1)
            },
            e.prevPageDisabled = function () {
                return 0 === e.currentPage ? "disabled" : ""
            },
            e.pageCount = function () {
                return Math.ceil(e.itemCount / e.itemsPerPage)
            },
            e.nextPage = function () {
                e.currentPage < e.pageCount() - 1 && e.setPage(e.currentPage + 1)
            },
            e.nextPageDisabled = function () {
                return e.currentPage === e.pageCount() - 1 ? "disabled" : ""
            },
            e.$watch("itemCount",
            function () {
                e.setPage(0)
            },
            !0),
            e.setPage(0)
        }
    }
}),
surveyModule.run(["$templateCache",
function (e) {
    var t = '<div class="mtc-pagination" style="float: left;width: 100%;"><ul class="mtc-pagination-wrap"><li ng-class="prevPageDisabled()"><a class="pagination-nav-btn" href="" ng-click="setPage(0)">&lt;&lt;</a></li><li ng-class="prevPageDisabled()"><a class="pagination-nav-btn" href="" ng-click="prevPage()">&lt;</a></li><li ng-repeat="n in pages" ng-class="{active: n == currentPage}" ng-click="setPage(n)"><a href="" ng-bind="{{n+1}}"></a></li><li ng-class="nextPageDisabled()"><a class="pagination-nav-btn" href="" ng-click="nextPage()">&gt;</a><li ng-class="nextPageDisabled()"><a class="pagination-nav-btn last-pagination-nav-btn" href="" ng-click="setPage(pageCount()-1)" >&gt;&gt;</a></li></li></ul></div>';
    e.put("pagination.html", t)
}]);