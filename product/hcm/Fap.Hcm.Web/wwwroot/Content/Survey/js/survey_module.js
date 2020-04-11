angular.module("ie7support", []).config(function (e) {
    e.enabled(!1)
});
var surveyModule = angular.module("survey", ["angularFileUpload", "ng.ueditor"]);
surveyModule.config(["$httpProvider",
function (e) {
    e.defaults.xsrfCookieName = "YII_CSRF_TOKEN",
    e.defaults.xsrfHeaderName = "X-YII-CSRF-TOKEN",
    e.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest"
}]),
surveyModule.directive("paginationList",
function () {
    return {
        restrict: "EA",
        require: "?ngModel",
        scope: {
            totalItems: "=",
            firstText: "@",
            prevText: "@",
            nextText: "@",
            lastText: "@"
        },
        templateUrl: "pagination_list.html",
        link: function (e, t, a, s) {
            function i(e, t) {
                var a = 5,
                s = Math.max(e - Math.floor((a - 1) / 2), 1),
                i = s + a - 1;
                i > t && (i = t, s = Math.max(i - a + 1, 1));
                for (var n = [], r = s; i >= r; r++) n.push(r);
                return n
            }
            s.$render = function () {
                e.currentPage = parseInt(s.$viewValue, 10) || 1,
                e.pages = i(e.currentPage, e.totalPage)
            };
            var n = {
                itemsPerPage: 10,
                currentPage: 1,
                isFirstLast: !0,
                isPrevNext: !0,
                firstText: "<<",
                prevText: "<",
                nextText: ">",
                lastText: ">>"
            };
            e.isFirstLast = angular.isDefined(a.isFirstLast) ? a.isFirstLast : n.isFirstLast,
            e.isPrevNext = angular.isDefined(a.isPrevNext) ? a.isPrevNext : n.isPrevNext,
            e.$watch("totalItems",
            function () {
                e.itemsPerPage = angular.isDefined(a.itemsPerPage) ? parseInt(a.itemsPerPage, 10) : n.itemsPerPage,
                e.totalPage = Math.ceil(e.totalItems / e.itemsPerPage),
                e.setPage(1)
            }),
            e.getText = function (e) {
                return a[e + "Text"] || n[e + "Text"]
            },
            e.setPage = function (t) {
                t > e.totalPage && (t = e.totalPage),
                1 > t && (t = 1),
                s.$setViewValue(t),
                s.$render()
            },
            e.isFirstPage = function () {
                return 1 === e.currentPage
            },
            e.isLastPage = function () {
                return e.currentPage === e.totalPage
            },
            e.isActive = function (t) {
                return e.currentPage === t
            }
        }
    }
}),
surveyModule.run(["$templateCache",
function (e) {
    var t = '<ul class=pagination-list> <li ng-if=isFirstLast><a ng-class="{disabled: isFirstPage()}" ng-bind="getText(\'first\')" ng-click="setPage(1)"></a> <li ng-if=isPrevNext><a ng-class="{disabled: isFirstPage()}" ng-bind="getText(\'prev\')" ng-click="setPage(currentPage - 1)"></a> <li ng-repeat="page in pages"><a ng-class="{current: isActive(page)}" ng-bind=page ng-click="setPage(page)"></a> <li ng-if=isPrevNext><a ng-class="{disabled: isLastPage()}" ng-bind="getText(\'next\')" ng-click="setPage(currentPage + 1)"></a> <li ng-if=isFirstLast><a ng-class="{disabled: isLastPage()}" ng-bind="getText(\'last\')" ng-click="setPage(totalPage)"></a></ul>';
    e.put("pagination_list.html", t)
}]),
surveyModule.directive("star",
function () {
    return {
        restrict: "EA",
        require: "?ngModel",
        scope: {
            readonly: "=",
            hints: "@",
            max: "@"
        },
        template: '<ul class=star-list> <li ng-repeat="star in stars"> <a ng-click="changeScore($index)" ng-attr-title="{{star.hint}}" class=star ng-class="{\'selected\': star.selected}"></a> </ul>',
        link: function (e, t, a, s) {
            function i() {
                e.stars = [],
                e.hints = angular.isDefined(a.hints) ? e.$eval(a.hints) : [];
                for (var t = 0; t < e.max; t++) {
                    var s = {
                        selected: t < e.score,
                        hint: void 0 == e.hints[t] ? "" : e.hints[t]
                    };
                    e.stars.push(s)
                }
            }
            e.max = angular.isDefined(a.max) ? a.max : 5,
            e.changeScore = function (t) {
                !e.readonly && t >= 0 && t < e.stars.length && (s.$setViewValue(t + 1), s.$render())
            },
            s.$render = function () {
                e.score = parseInt(s.$viewValue, 10) || 0,
                i()
            }
        }
    }
});