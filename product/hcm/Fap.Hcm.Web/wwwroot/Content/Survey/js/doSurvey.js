/*! crowdtest */
"use strict";
crowdtestModule.controller("SurveyDetailController", ["$scope", "$sce", "$http", "$q", "$timeout", "$filter", "$location", "$anchorScroll", "FileUploader", "User",
function (a, b, c, d, e, f, g, h, i, j) {
    function k(a) {
        g.hash(a),
        h()
    }
    function l(a) {
        var b = !1;
        return angular.forEach(a.value,
        function (c, d) {
            c === !0 && n(a.choice[d].id.toString(), a.exclusive_options.split(",")) && (b = !0)
        }),
        b
    }
    function m() {
        var a = 0;
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) && (a = 1),
        a
    }
    function n(a, b) {
        var c = !1;
        return $.each(b,
        function (b, d) {
            if (d === a) return c = !0,
            !1
        }),
        c
    }
    function o(b) {
        var d = $.Deferred();
        a.isClosed = !0;
        var e = H();
        e.is_mobile = m(),
        e.save_type = 1,
        "object" == typeof b && ("verify_code" in b && (e.verify_code = b.verify_code), "telephone" in b && (e.telephone = b.telephone), "time" in b && (e.time = b.time));
        var f = base_url_module + "survey/saveSurveyResponse";
        return a.project.notLoginPassed && (f = base_url_module + "survey/saveSurveyResponseAgent"),
        c.post(f, e).success(function (b) {
            $(".verify-code-popup").remove(),
            0 === b.error_code ? location.href = base_url_module + b.msg : 3 === b.error_code ? (alert("提交失败！"), a.isClosed = !1) : j.currentUser.id > 0 ? location.href = base_url_module + "user/error" : location.href = base_url_module + "survey/closeProjectAgent/sur_id/" + a.surveyId + "/type/" + b.error_code,
            d.resolve()
        }),
        {
            promise: d.promise()
        }
    }
    function p(b) {
        a.isDisabled = !0;
        var d = H();
        d.is_mobile = m(),
        d.save_type = 1,
        d.token = a.project.token,
        d.referer = a.project.referer,
        d.time = a.startTime,
        "object" == typeof b && "telephone" in b && (d.telephone = b.telephone);
        var e = base_url_module + "api/Survey/SaveSelfCollectionSurveyResponse";
        c.post(e, d).success(function (b) {
            if (b.success) {
                window.location.href = basePath + "Survey/Survey/FinishSelfCollectionSurvey";
            } else {
                (seajs.use("popups/survey/operate_popup",
            function (a) {
                a.show("提交失败！", "确定", null).then(function () { })
            }), a.isDisabled = !1)
            }
            //0 === b.error_code ? 1 == a.project.vote_type && 1 == a.project.viewResult ? window.location.href = base_url_module + "survey/doSelfcollectionSurvey/sur_id/" + a.surveyId + "/token/" + a.project.token + "/isClosed/1" : window.location.href = base_url_module + "Survey/Survey/FinishSelfCollectionSurvey" : 1 === b.error_code || 2 === b.error_code ? window.location.href = base_url_module + "survey/finishSelfCollectionSurvey/sur_id/" + a.surveyId + "/token/" + a.project.token : 3 === b.error_code && (seajs.use("popups/survey/operate_popup",
            //function (a) {
            //    a.show("提交失败！", "确定", null).then(function () { })
            //}), a.isDisabled = !1)
        })
    }
    function q(a) {
        a.targetTime = 0,
        angular.forEach(a.list,
        function (b) {
            0 === b.logic_hide && (a.targetTime += 3)
        })
    }
    function r(b) {
        clearInterval(I),
        a.runningTime = 0,
        q(b),
        b.restTime = b.targetTime,
        I = setInterval(function () {
            a.runningTime += 1,
            t(b),
            a.$apply(),
            b.restTime <= 0 && clearInterval(I)
        },
        1e3)
    }
    function s(b) {
        clearInterval(I),
        I = setInterval(function () {
            a.runningTime += 1,
            t(b),
            a.$apply(),
            b.restTime <= 0 && clearInterval(I)
        },
        1e3)
    }
    function t(b) {
        b.restTime = b.targetTime - a.runningTime,
        b.restTime < 0 && (b.restTime = 0)
    }
    a.init = function (b, c, d) {
        a.startTime = f("date")(new Date, "yyyy-MM-dd HH:mm:ss"),
        k("survey-navigation"),
        a.selectedPage = 1,
        a.surveyId = b.survey_id,
        a.pages = b.pages,
        a.pageCount = b.page_count,
        a.logic = b.logic_condition && b.logic_condition.logic_condition_relation ? b.logic_condition.logic_condition_relation : null,
        angular.isArray(a.logic) && (a.logic = a.logic.length > 0 ? a.logic : null),
        a.redirectLogic = a.logic ? null : b.redirect_relation ? b.redirect_relation : null,
        a.redirectQuesArr = [],
        a.auto_filter = 0,
        a.autoFilterQuestionId = -2,
        a.scene = parseInt(b.scene, 10),
        a.type = parseInt(b.type, 10),
        a.isClosed = c,
        a.isDisabled = !1,
        a.project = d,
        a.questionMap = {},
        a.quoteAnswerArr = {},
        a.choiceQuoteRelation = {},
        a.absluteIdOrderMap = {},
        a.runningTime = 0,
        a.currentQuestion = null,
        a.loaded = 0,
        u(),
        w(),
        a.process = {
            current: 0,
            total: 0,
            percent: 0
        },
        a.updateProcess()
    },
    a.updateProcess = function () {
        var b = 0,
        c = 0;
        angular.forEach(a.pages,
        function (d) {
            angular.forEach(d.list,
            function (d) {
                if (0 === d.logic_hide && 10 != d.type_id && 11 != d.type) {
                    c++;
                    var e = a.checkQuestion(d);
                    e && b++
                }
            })
        }),
        a.process.current = b,
        a.process.total = c,
        a.process.percent = c > 0 ? parseInt(100 * b / c, 10) : 0
    },
    a.radioSelect = function (b, c, d) {
        a.isClosed || (b.value = c, a.shuffleLogicShow(b, d))
    },
    a.checkboxSelect = function (b, c, d) {
        a.isClosed || (b.value[c] = !b.value[c], a.shuffleLogicShow(b, d))
    },
    a.isChoiceSelect = function (a, b, c) {
        return 6 == a.type_id && a.value == b || 8 == a.type_id && 1 == a.value[b]
    },
    a.answerQuestion = function (b, c, d) {
        if (!a.isClosed) {
            switch (b.type_id) {
                case 6:
                case 14:
                    b.choice_quote > 0 && (c = parseInt(d.order, 10) - 1),
                    b.value = c,
                    a.shuffleLogicShow(b, d);
                    break;
                case 7:
                    a.shuffleLogicShow(b, d);
                    break;
                case 8:
                case 15:
                    b.choice_quote > 0 && (c = parseInt(d.order, 10) - 1),
                    b.value[c] = !b.value[c],
                    a.shuffleLogicShow(b, d),
                    b.value[c] === !0 && a.updateExclusive(b, c, d);
                    break;
                case 1:
                case 2:
                    a.shuffleLogicShow(b, []);
                    break;
                case 9:
                    b.value[d] = c,
                    a.shuffleLogicShow(b, []);
                    break;
                case 13:
                    b.value[d][c] = !b.value[d][c],
                    a.shuffleLogicShow(b, [])
            }
            var e = b.content.split(".");
            void 0 !== a.quoteAnswerArr[e[0]] && (a.quoteAnswerArr[e[0]] = y(b)),
            a.choiceQuoteRelation[b.id] && angular.forEach(a.choiceQuoteRelation[b.id],
            function (c, d) {
                a.pages[c.pageIndex].list[c.quesIndex].content_show = 0,
                angular.forEach(b.value,
                function (b, d) {
                    b === !0 ? (a.pages[c.pageIndex].list[c.quesIndex].content_show = 1, a.pages[c.pageIndex].list[c.quesIndex].choice[d].hide = 0) : a.pages[c.pageIndex].list[c.quesIndex].choice[d].hide = 1
                })
            }),
            a.currentQuestion = b,
            a.updateProcess()
        }
    },
    a.changeQuestion = function (b) {
        a.currentQuestion = b
    },
    a.stopPropagation = function (a) {
        a.stopPropagation()
    },
    a.fileUploader = new i({
        url: "/crowdtest/file/fileUpload",
        autoUpload: !0,
        removeAfterUpload: !0,
        queueLimit: 4,
        formData: [{
            YII_CSRF_TOKEN: window.yii_csrf_token_global
        }]
    }),
    a.fileUploader.onCompleteItem = function (b, c, d, e) {
        b.question.value.length < b.question.max && b.question.value.push({
            value: c.data.download_url,
            file_id: c.data.file_id,
            file_name: c.data.file_name
        }),
        a.updateProcess()
    },
    a.deleteQuestionItem = function (b, c, d) {
        c = c.splice(d, 1),
        a.answerQuestion(b)
    },
    a.replaceTitleQuote = function (b) {
        var c = /\[Q[0-9]*[1-9][0-9]*\]/g,
        d = b.match(c);
        if (null === d) return b;
        var e = b;
        return angular.forEach(d,
        function (b) {
            var c = b.substring(2, b.length - 1),
            d = new RegExp("\\[Q" + c + "\\]", "g");
            e = a.quoteAnswerArr[c] ? e.replace(d, a.quoteAnswerArr[c]) : e
        }),
        e
    };
    var u = function () {
        angular.forEach(a.pages,
        function (b, c) {
            b.restTime = 3,
            b.targetTime = 0,
            angular.forEach(b.list,
            function (b, d) {
                if (a.questionMap[b.id] = [c, d], b.passed = !0, b.type_id = parseInt(b.type_id, 10), b.vote_type_id = parseInt(b.vote_type_id, 10), b.logic_hide = parseInt(b.logic_hide, 10), 4 != a.project.status && 10 != b.type_id && b.index && (b.content = b.index + ". " + b.content), b.logic_hide > 0 && (b.logicParent = []), "N" === b.required && b.content.indexOf(". ") >= 0) {
                    var f = b.content.split(". "),
                    g = f[0];
                    f.splice(0, 1),
                    b.content = g + ". 【非必填】" + f.join(". ")
                }
                if ("Y" === b.title_quote) {
                    var h = x(b.content);
                    angular.forEach(h,
                    function (b) {
                        void 0 === a.quoteAnswerArr[b] && (a.quoteAnswerArr[b] = "")
                    })
                }
                if (parseInt(b.choice_quote, 10) > 0 ? (b.content_show = 0, a.choiceQuoteRelation[b.choice_quote] || (a.choiceQuoteRelation[b.choice_quote] = []), a.choiceQuoteRelation[b.choice_quote].push({
                    pageIndex: c,
                    quesIndex: d
                })) : b.content_show = 1, 7 === b.type_id && b.value && angular.forEach(b.choice,
                function (a) {
                    b.value.id === a.id && (b.value = a)
                }), 8 !== b.type_id && 15 !== b.type_id || (b.max = parseInt(b.max ? b.max : 0, 10), b.min = parseInt(b.min ? b.min : 0, 10), (b.max > 0 || b.min > 0) && (b.min === b.max ? b.tip = "选择" + b.max + "个选项" : 0 === b.max ? b.tip = "选择至少" + b.min + "个选项" : 0 === b.min ? b.tip = "选择至多" + b.max + "个选项" : b.tip = "选择" + b.min + "-" + b.max + "个选项", b.tip && 0 === b.vote_type_id && (b.tip = " (" + b.tip + ")"))), 12 === b.type_id && (b.max = parseInt(b.max, 10), b.min = "" == b.min ? 0 : parseInt(b.min, 10), (b.max > 0 || b.min > 0) && (b.min === b.max ? b.tip = "上传" + b.max + "张图片" : 0 === b.max ? b.tip = "最少上传" + b.min + "张图片" : 0 === b.min ? b.tip = "最多上传" + b.max + "张图片" : b.tip = "上传" + b.min + "-" + b.max + "张图片", b.tip && 0 === b.vote_type_id && (b.tip = " (" + b.tip + ")"))), 3 !== b.vote_type_id && 4 !== b.vote_type_id && 14 !== b.type_id && 15 !== b.type_id || angular.forEach(b.choice,
                function (a, b) {
                    a.contentArr = a.content.split("#**#")
                }), 8 === b.vote_type_id) {
                    var i = 0;
                    b.vertical = 0;
                    var j = 0;
                    a.loaded = 0,
                    angular.forEach(b.choice,
                    function (c, d) {
                        var f = new Image;
                        f.src = c.content,
                        f.onload = function () {
                            j++,
                            f.width > f.height && (e(function () {
                                b.vertical = 1
                            }), i = 1),
                            c.imgHeight = f.height,
                            c.imgWidth = f.width,
                            j == b.choice.length && (angular.forEach(b.choice,
                            function (a, b) {
                                var c = v(a.imgWidth, a.imgHeight, i);
                                1 == i ? a.content = "<img src='" + a.content + "' style='max-width: 100%;max-height:100%'/>" : a.content = "<img src='" + a.content + "' style='width:" + c.width + "px;height:" + c.height + "px;'/>"
                            }), a.loaded = 1)
                        }
                    })
                } else a.loaded = 1;
                var k = b.content.split(".");
                a.absluteIdOrderMap[b.id] = k[0]
            }),
            q(b),
            1 === b.index && r(b)
        })
    },
    v = function (a, b, c) {
        var d, e;
        return 1 == c ? a <= 849 && b <= 214 ? (d = b, e = a) : a <= 849 ? (d = 214, e = parseFloat(a / b) * d, e > 849 && (e = 849, d = parseFloat(b / a) * e)) : b <= 214 ? (e = 849, d = parseFloat(b / a) * e, d > 214 && (d = 214, e = parseFloat(a / b) * d)) : (e = 849, d = parseFloat(b / a) * e, d > 214 && (d = 214, e = parseFloat(a / b) * d)) : a <= 412 && b <= 600 ? (d = b, e = a) : a <= 412 ? (d = 600, e = parseFloat(a / b) * d, e > 412 && (e = 412, d = parseFloat(b / a) * e)) : b <= 600 ? (e = 412, d = parseFloat(b / a) * e, d > 600 && (d = 600, e = parseFloat(a / b) * d)) : (e = 412, d = parseFloat(b / a) * e, d > 600 && (d = 600, e = parseFloat(a / b) * d)),
        {
            width: e,
            height: d
        }
    },
    w = function () {
        null != a.logic ? angular.forEach(a.pages,
        function (b) {
            angular.forEach(b.list,
            function (b) {
                b.type_id > 5 && b.type_id < 9 && b.logic_condition && b.logic_condition.logic_show_list && angular.forEach(b.choice,
                function (c) {
                    c.selected === !0 && a.shuffleLogicShow(b, c)
                })
            })
        }) : null != a.redirectLogic && (angular.forEach(a.redirectLogic,
        function (b, c) {
            a.redirectQuesArr.push(c)
        }), a.redirectQuesArr.length > 0 && E(a.pages[a.questionMap[a.redirectQuesArr[0]][0]].list[a.questionMap[a.redirectQuesArr[0]][1]]))
    },
    x = function (a) {
        var b = /\[Q[0-9]*[1-9][0-9]*\]/g,
        c = a.match(b);
        if (null === c) return !1;
        var d = [];
        return angular.forEach(c,
        function (a) {
            var b = a.substring(2, a.length - 1);
            n(b, d) || d.push(b)
        }),
        d
    },
    y = function (a) {
        var b = "";
        if (1 !== a.type_id && 2 !== a.type_id || "" === a.value && null === a.value || (b = a.value), 6 === a.type_id && null !== a.value && (b = a.choice[a.value].content, "Y" === a.choice[a.value].is_other && a.choice[a.value].other_content && (b += a.choice[a.value].other_content)), 7 === a.type_id && null !== a.value && (b = a.value.content), 8 === a.type_id) {
            var c = [];
            for (var d in a.value) d < a.value.length && a.value[d] === !0 && c.push(d);
            if (c.length > 0) {
                var e = [];
                for (var d in c) if (d < a.value.length) {
                    var f = a.choice[c[d]].content;
                    "Y" === a.choice[c[d]].is_other && a.choice[c[d]].other_content && (f += a.choice[c[d]].other_content),
                    e.push(f)
                }
                b = e.join("、")
            }
        }
        return b
    };
    a.trustAsHtml = function (a) {
        return b.trustAsHtml(a)
    },
    a.toJsDate = function (a) {
        return a ? new Date(a) : null
    },
    a.formatDate = function (a) {
        if (!a) return null;
        var b = new Date(a),
        c = b.getFullYear() + "年" + (b.getMonth() + 1) + "月" + b.getDate() + "日";
        return c
    },
    a.selectPage = function (b) {
        a.selectedPage = b
    };
    var z = function (b, c) {
        if (a.logic && a.logic[b] && a.logic[b][c]) {
            var d = a.logic[b][c];
            for (var e in d) e = parseInt(e, 10),
            1 === parseInt(d[e], 10) && (a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]].logic_hide = 0, a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]].logicParent.indexOf(b) === -1 && a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]].logicParent.push(b))
        }
    },
    A = function (b, c) {
        if (a.logic && a.logic[b] && a.logic[b][c]) {
            var d = a.logic[b][c];
            for (var e in d) if (e = parseInt(e, 10), 1 === parseInt(d[e], 10)) {
                var f = a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]];
                0 === f.logic_hide && f.logicParent.indexOf(b) > -1 && f.logicParent.length < 2 && (a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]].logic_hide = 1, B(f)),
                f.logicParent.indexOf(b) > -1 && a.pages[a.questionMap[e][0]].list[a.questionMap[e][1]].logicParent.remove(b)
            }
        }
    },
    B = function (a) {
        var b = [];
        6 === a.type_id && null !== a.value && (b.push(a.choice[a.value].id), a.value = null),
        7 === a.type_id && null !== a.value && (b.push(a.value.id), a.value = null),
        8 === a.type_id && angular.forEach(a.value,
        function (c, d) {
            c === !0 && (b.push(a.choice[d].id), a.value[d] = null)
        }),
        angular.forEach(b,
        function (b) {
            A(a.id, b)
        })
    };
    a.shuffleLogicShow = function (b, c) {
        if (4 != a.project.status || b.logic_condition && b.logic_condition.logic_show_list) {
            for (var d in b.choice) (8 !== b.type_id || 8 === b.type_id && b.value[d] !== !0) && A(b.id, b.choice[d].id);
            if (6 === b.type_id && z(b.id, c.id), 7 === b.type_id && z(b.id, b.value.id), 8 === b.type_id) for (var d in b.value) b.value[d] === !0 && z(b.id, b.choice[d].id);
            a.updateRestTime(a.pages[a.selectedPage - 1])
        }
        if (!a.logic && a.redirectLogic && a.redirectLogic[b.id]) {
            for (var d in b.choice) (8 !== b.type_id && 15 !== b.type_id || (8 === b.type_id || 15 === b.type_id) && b.value[d] !== !0) && E(b);
            if (6 !== b.type_id && 14 !== b.type_id || F(b.id, c.id), 7 === b.type_id && F(b.id, b.value.id), 8 === b.type_id || 15 === b.type_id) {
                var e = C(b);
                e !== !1 && F(b.id, b.choice[e].id)
            }
            1 !== b.type_id && 2 !== b.type_id || ($.trim(b.value) ? F(b.id, 0) : E(b)),
            9 === b.type_id && F(b.id, 0),
            13 === b.type_id && D(b) && F(b.id, 0),
            a.updateRestTime(a.pages[a.selectedPage - 1])
        }
    };
    var C = function (b) {
        var c, d, e, f = !1,
        g = 0;
        for (var h in b.value) if (b.value[h] === !0) {
            f = !0,
            void 0 === c && (c = h);
            var i = b.choice[h].id;
            if (a.redirectLogic[b.id][i]) {
                var j = a.redirectLogic[b.id][i];
                angular.forEach(j,
                function (b, f) {
                    if (g === -1) parseInt(f, 10) === -2 && (c = h, g = -2);
                    else if (0 === g) if (parseInt(f, 10) > 0) {
                        var i = a.questionMap[f][0],
                        j = a.questionMap[f][1]; (void 0 === d || void 0 === e || i < d || i === d && j < e) && (d = i, e = j, c = h)
                    } else g = parseInt(f),
                    c = h
                })
            }
        }
        return f === !0 && c
    },
    D = function (a) {
        var b = !1;
        return angular.forEach(a.value,
        function (a, c) {
            b || angular.forEach(a,
            function (a, c) {
                a !== !0 || b || (b = !0)
            })
        }),
        b
    },
    E = function (b) {
        a.auto_filter = 0;
        var c = a.questionMap[b.id][0],
        d = a.questionMap[b.id][1];
        angular.forEach(a.pages,
        function (a, b) {
            angular.forEach(a.list,
            function (a, e) {
                (b > c || b === c && e > d) && (a.logic_hide = 1)
            })
        })
    },
    F = function (b, c) {
        if (a.redirectLogic && a.redirectLogic[b]) {
            var d, e, f = !1;
            if (a.redirectLogic[b][c]) {
                var g = a.redirectLogic[b][c];
                angular.forEach(g,
                function (b, c) {
                    parseInt(c, 10) > 0 ? (d = a.questionMap[c][0], e = a.questionMap[c][1], f = !0, a.auto_filter = 0) : (a.auto_filter = parseInt(c, 10) === a.autoFilterQuestionId ? 1 : 0, f = !1)
                })
            } else d = a.questionMap[b][0],
            e = a.questionMap[b][1] + 1,
            f = !0,
            a.auto_filter = 0;
            if (f) {
                var h = !1;
                angular.forEach(a.pages,
                function (b, c) {
                    angular.forEach(b.list,
                    function (b, f) {
                        (c > d || c === d && f >= e) && !h && (b.logic_hide = 0, a.redirectLogic[b.id] && (h = !0))
                    })
                })
            }
        }
    };
    a.updateExclusive = function (a, b, c) {
        if (a.exclusive_options) {
            var d = a.exclusive_options.split(",");
            n(c.id.toString(), d) ? (angular.forEach(a.value,
            function (b, c) {
                a.value[c] = !1
            }), a.value[b] = !0) : angular.forEach(d,
            function (b) {
                angular.forEach(a.choice,
                function (c, d) {
                    c.id.toString() === b && (a.value[d] = !1)
                })
            })
        }
    },
    a.getQuoteTip = function (b) {
        var c = "";
        if (b.choice_quote > 0) {
            var d = "";
            switch (parseInt(b.type_id, 10)) {
                case 6:
                    d = "单选题";
                    break;
                case 7:
                    d = "下拉选择题";
                    break;
                case 8:
                    d = "多选题"
            }
            c = "&lt;" + d + "&gt此题目选项来自于第" + a.absluteIdOrderMap[b.choice_quote] + "题中的选项"
        }
        return c
    },
    a.filterHide = function (a) {
        var b = [];
        return angular.forEach(a,
        function (a, c) {
            a.hide && 0 !== a.hide || b.push(a)
        }),
        b
    };
    var G = function (a) {
        var b = !0,
        c = !0;
        return angular.forEach(a,
        function (a) {
            if (a.passed = !0, "Y" === a.required && 0 === a.logic_hide) {
                if (1 !== a.type_id && 2 !== a.type_id || null !== a.value && "" !== a.value || (b = !1, a.passed = !1), 6 !== a.type_id && 7 !== a.type_id && 14 !== a.type_id || null !== a.value || (b = !1, a.passed = !1), 8 === a.type_id || 15 === a.type_id) {
                    var d = !1,
                    e = 0;
                    angular.forEach(a.value,
                    function (a) {
                        a === !0 && (e += 1, d = !0)
                    }),
                    d === !1 && (b = !1, a.passed = !1),
                    parseInt(a.max, 10) > 0 && e > parseInt(a.max, 10) && (b = !1, a.passed = !1),
                    parseInt(a.min, 10) > 0 && e < parseInt(a.min, 10) && !l(a) && (b = !1, a.passed = !1)
                }
                if (9 === a.type_id && angular.forEach(a.value,
                function (c, d) {
                    a.radio_array_title[d].passed = !0,
                    null === c && (b = !1, a.passed = !1, a.radio_array_title[d].passed = !1)
                }), 13 === a.type_id && angular.forEach(a.value,
                function (c, d) {
                    var f = !1;
                    a.checkbox_array_title[d].passed = !0,
                    angular.forEach(c,
                    function (a) {
                        a === !0 && (e += 1, f = !0)
                }),
                    f === !1 && (b = !1, a.passed = !1, a.checkbox_array_title[d].passed = !1)
                }), 12 === a.type_id) {
                    var e = a.value.length;
                    e <= 0 && (b = !1, a.passed = !1),
                    parseInt(a.max, 10) > 0 && e > parseInt(a.max, 10) && (b = !1, a.passed = !1),
                    parseInt(a.min, 10) > 0 && e < parseInt(a.min, 10) && (b = !1, a.passed = !1)
                }
            }
            if (0 === a.logic_hide && a.passed && (6 == a.type_id && angular.forEach(a.choice,
            function (c, d) {
                a.value == d && "Y" == c.is_other && 1 == c.required && (null !== c.other_content && "" !== $.trim(c.other_content) || (b = !1, a.passed = !1))
            }), 8 == a.type_id && angular.forEach(a.choice,
            function (c, d) {
                a.value[d] && "Y" == c.is_other && 1 == c.required && (null !== c.other_content && "" !== $.trim(c.other_content) || (b = !1, a.passed = !1))
            }), 9 === a.type_id && angular.forEach(a.value,
            function (c, d) {
                if (null !== c && c in a.choice) {
                    var e = a.choice[c];
                    if ("Y" == e.is_other && 1 == e.required) {
                        var f = a.radio_array_title[d].other_content[c];
                        null !== f && "" !== $.trim(f) || (b = !1, a.passed = !1, a.radio_array_title[d].passed = !1)
            }
            }
            }), 13 === a.type_id && angular.forEach(a.value,
            function (c, d) {
                angular.forEach(a.choice,
                function (e, f) {
                    if (c[f] && "Y" == e.is_other && 1 == e.required) {
                        var g = a.checkbox_array_title[d].other_content[f];
                        null !== g && "" !== $.trim(g) || (b = !1, a.passed = !1, a.checkbox_array_title[d].passed = !1)
            }
            })
            })), c && !a.passed) {
                c = !1;
                var f = "survey-question-" + a.page + "-" + a.order;
                k(f)
            }
        }),
        b
    };
    a.checkQuestion = function (a) {
        var b = !0;
        if (0 === a.logic_hide) {
            if (1 !== a.type_id && 2 !== a.type_id && 3 !== a.type_id || null !== a.value && "" !== a.value || (b = !1), 6 !== a.type_id && 7 !== a.type_id && 14 !== a.type_id || null !== a.value || (b = !1), 8 === a.type_id || 15 === a.type_id) {
                var c = !1,
                d = 0;
                angular.forEach(a.value,
                function (a) {
                    a === !0 && (d += 1, c = !0)
                }),
                c === !1 && (b = !1),
                parseInt(a.max, 10) > 0 && d > parseInt(a.max, 10) && (b = !1),
                parseInt(a.min, 10) > 0 && d < parseInt(a.min, 10) && !l(a) && (b = !1)
            }
            if (9 === a.type_id && angular.forEach(a.value,
            function (a) {
                null === a && (b = !1)
            }), 13 === a.type_id && angular.forEach(a.value,
            function (a, c) {
                var e = !1;
                angular.forEach(a,
                function (a) {
                    a === !0 && (d += 1, e = !0)
            }),
                e === !1 && (b = !1)
            }), 12 === a.type_id) {
                var d = a.value.length;
                d <= 0 && (b = !1),
                parseInt(a.max, 10) > 0 && d > parseInt(a.max, 10) && (b = !1),
                parseInt(a.min, 10) > 0 && d < parseInt(a.min, 10) && (b = !1)
            }
        }
        return b
    },
    a.checkNextPage = function (b) {
        var c = G(b);
        c === !0 && (a.selectPage(a.selectedPage + 1), r(a.pages[a.selectedPage - 1]), k("survey-navigation"))
    };
    var H = function () {
        var b = {};
        return b.survey_id = a.surveyId,
        b.auto_filter = a.auto_filter,
        b.sample_result = {},
        b.extra = {},
        angular.forEach(a.pages,
        function (a) {
            angular.forEach(a.list,
            function (a) {
                if (1 !== a.logic_hide) {
                    if (1 !== a.type_id && 2 !== a.type_id && 3 !== a.type_id || "" === a.value && null === a.value || (b["response_" + a.order] = a.value), 5 === a.type_id && null !== a.value && (a.value === !0 ? b["response_" + a.order] = "Y" : b["response_" + a.order] = "N"), 6 !== a.type_id && 14 !== a.type_id || null === a.value || (a.hasOwnProperty("sample_id") ? b.sample_result[a.sample_id] = a.choice[a.value].choice_id : (b["response_" + a.order] = a.choice[a.value].id, "Y" === a.choice[a.value].is_other && (b["other_" + a.order + "_" + a.choice[a.value].id] = a.choice[a.value].other_content))), 7 === a.type_id && null !== a.value && (b["response_" + a.order] = a.value.id), 8 === a.type_id || 15 === a.type_id) {
                        var c = [];
                        for (var d in a.value) d < a.value.length && a.value[d] === !0 && c.push(d);
                        if (c.length > 0) {
                            var e = [];
                            for (var d in c) d < a.value.length && (e.push(a.choice[c[d]].id), "Y" === a.choice[c[d]].is_other && (b["other_" + a.order + "_" + a.choice[[c[d]]].id] = a.choice[c[d]].other_content));
                            b["response_" + a.order] = e
                        }
                    }
                    if (9 === a.type_id) for (var d in a.value) d < a.value.length && null !== a.value[d] && (a.radio_array_title[d].hasOwnProperty("sample_id") ? b.sample_result[a.radio_array_title[d].sample_id] = a.choice[a.value[d]].order : (b["response_" + a.order + "_" + a.radio_array_title[d].id] = a.choice[a.value[d]].id, "Y" === a.choice[a.value[d]].is_other && (b["other_" + a.order + "_" + a.radio_array_title[d].id + "_" + a.choice[a.value[d]].id] = a.radio_array_title[d].other_content[a.value[d]])));
                    if (13 === a.type_id) for (var d in a.value) if (d < a.value.length && 0 != a.value[d].length) {
                        var c = [];
                        for (var f in a.value[d]) f < a.value[d].length && a.value[d][f] === !0 && c.push(f);
                        if (c.length > 0) {
                            var e = [];
                            for (var f in c) f < a.value[d].length && (e.push(a.choice[c[f]].id), "Y" === a.choice[c[f]].is_other && (b["other_" + a.order + "_" + a.checkbox_array_title[d].id + "_" + a.choice[c[f]].id] = a.checkbox_array_title[d].other_content[c[f]]));
                            b["response_" + a.order + "_" + a.checkbox_array_title[d].id] = e
                        }
                    }
                    12 === a.type_id && a.value.length > 0 && (b["response_" + a.order] = a.value),
                    5 === a.vote_type_id && (b.extra[a.order] = a.extra)
                }
            })
        }),
        b
    };
    a.submit = function (b) {
        var d = $.Deferred(),
        e = G(b);
        return e === !0 && (a.project.notLoginPassed ? seajs.use("popups/survey/phone",
        function (b) {
            b.init({
                color: "#e93630"
            }).promise.then(function (b) {
                var e = base_url_module + "n/survey/isTelephoneUnique",
                f = {
                    survey_id: a.project.id,
                    telephone: b.telephone
                };
                c.post(e, f).success(function (b) {
                    if (0 === parseInt(b.status, 10)) {
                        var c = {
                            time: a.startTime,
                            telephone: f.telephone
                        };
                        o(c).promise.then(function () {
                            d.resolve()
                        })
                    } else window.location.href = base_url_module + "survey/closeProjectAgent/sur_id/" + a.surveyId + "/type/20"
                })
            })
        }) : seajs.use("popups/survey/vcodePop",
        function (a) {
            a.show(base_url_module).promise.then(function (a) {
                o({
                    verify_code: a
                }).promise.then(function () {
                    d.resolve()
                })
            },
            function (a) {
                d.resolve()
            })
        })),
        {
            promise: d.promise()
        }
    },
    a.submitSelf = function (b) {
        var d = G(b);
        d === !0 && (4 == a.project.status ? 1 == a.project.telephone_filter ? seajs.use("popups/survey/phone",
        function (b) {
            b.init({
                color: "#6573cc"
            }).promise.then(function (b) {
                var d = base_url_module + "n/survey/isTelephoneUnique",
                e = {
                    survey_id: a.project.id,
                    telephone: b.telephone
                };
                c.post(d, e).success(function (c) {
                    0 === parseInt(c.status, 10) ? p(b) : window.location.href = base_url_module + "survey/finishSelfCollectionSurvey/sur_id/" + a.surveyId + "/token/" + a.project.token
                })
            })
        }) : p() : window.location.href = base_url_module + "survey/finishSelfCollectionSurvey/sur_id/" + a.surveyId + "/token/" + a.project.token)
    },
    a.$on("submit",
    function (b, c) {
        a.submit(c)
    }),
    a.$on("submitSelf",
    function (b, c) {
        a.submitSelf(c)
    }),
    a.updateRestTime = function (b) {
        q(b),
        b.restTime <= 0 && b.targetTime > a.runningTime && s(b)
    };
    var I = 0;
    a.initPageTimer = function (a) {
        r(a)
    };
    a.questionTypeClass = function (a) {
        var b, c = a.vote_type_id;
        return 6 == c ? b = "input-type" : 7 == c ? b = "text-type" : 8 == c && (b = 1 == a.vertical ? "image-type vertical-type" : "image-type horizon-type"),
        b
    }
}]);