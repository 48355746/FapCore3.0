$(document).ready(function () {
    Array.prototype.indexOf = function (e) {
        for (var i = 0; i < this.length; i++) if (this[i] == e) return i;
        return -1
    },
    Array.prototype.remove = function (e) {
        var i = this.indexOf(e);
        i > -1 && this.splice(i, 1)
    }
}),
surveyModule.controller("SurveyDetailController", ["$scope", "$sce", "$http", "$q", "$timeout", "$location", "$anchorScroll", "FileUploader",
function (e, i, t, n, a, o, r, c) {
    function u(e) {
        o.hash(e),
        r()
    }
    function l(e) {
        var i = !1;
        return angular.forEach(e.value,
        function (t, n) {
            t === !0 && inArray(e.choice[n].id.toString(), e.exclusive_options.split(",")) && (i = !0)
        }),
        i
    }
    function s(e) {
        e.targetTime = 0,
        angular.forEach(e.list,
        function (i) {
            0 === i.logic_hide && (e.targetTime += 3)
        })
    }
    function p(i) {
        clearInterval(M),
        e.runningTime = 0,
        s(i),
        i.restTime = i.targetTime,
        M = setInterval(function () {
            e.runningTime += 1,
            f(i),
            e.$apply(),
            i.restTime <= 0 && clearInterval(M)
        },
        1e3)
    }
    function d(i) {
        clearInterval(M),
        M = setInterval(function () {
            e.runningTime += 1,
            f(i),
            e.$apply(),
            i.restTime <= 0 && clearInterval(M)
        },
        1e3)
    }
    function f(i) {
        i.restTime = i.targetTime - e.runningTime,
        i.restTime < 0 && (i.restTime = 0)
    }
    e.init = function (i, t, n) {
        u("survey-navigation"),
        e.selectedPage = 1,
        e.surveyId = i.survey_id,
        e.pages = i.pages,
        e.pageCount = i.page_count,
        e.logic = i.logic_condition && i.logic_condition.logic_condition_relation ? i.logic_condition.logic_condition_relation : null,
        angular.isArray(e.logic) && (e.logic = e.logic.length > 0 ? e.logic : null),
        e.redirectLogic = e.logic ? null : i.redirect_relation ? i.redirect_relation : null,
        e.redirectQuesArr = [],
        e.stopType = 0,
        e.scene = parseInt(i.scene, 10),
        e.type = parseInt(i.type, 10),
        e.isClosed = t,
        e.isDisabled = !1,
        e.project = n,
        e.questionMap = {},
        e.quoteAnswerArr = {},
        e.choiceQuoteRelation = {},
        e.absluteIdOrderMap = {},
        e.runningTime = 0,
        e.currentQuestion = null,
        e.loaded = 0,
        h(),
        v(),
        e.process = {
            current: 0,
            total: 0,
            percent: 0
        },
        e.updateProcess()
    },
    e.updateProcess = function () {
        var i = 0,
        t = 0;
        angular.forEach(e.pages,
        function (n) {
            angular.forEach(n.list,
            function (n) {
                if (0 === n.logic_hide && 10 != n.type_id && 11 != n.type) {
                    t++;
                    var a = e.checkQuestion(n);
                    a && i++
                }
            })
        }),
        e.process.current = i,
        e.process.total = t,
        e.process.percent = t > 0 ? parseInt(100 * i / t, 10) : 0
    },
    e.radioSelect = function (i, t, n) {
        e.isClosed || (i.value = t, e.shuffleLogicShow(i, n))
    },
    e.checkboxSelect = function (i, t, n) {
        e.isClosed || (i.value[t] = !i.value[t], e.shuffleLogicShow(i, n))
    },
    e.isChoiceSelect = function (e, i) {
        return 6 == e.type_id && e.value == i ? !0 : 8 == e.type_id && 1 == e.value[i] ? !0 : !1
    },
    e.answerQuestion = function (i, t, n) {
        if (!e.isClosed) {
            switch (i.type_id) {
                case 6:
                case 14:
                    i.choice_quote > 0 && (t = parseInt(n.order, 10) - 1),
                    i.value = t,
                    e.shuffleLogicShow(i, n);
                    break;
                case 7:
                    e.shuffleLogicShow(i, n);
                    break;
                case 8:
                case 15:
                    i.choice_quote > 0 && (t = parseInt(n.order, 10) - 1),
                    i.value[t] = !i.value[t],
                    e.shuffleLogicShow(i, n),
                    i.value[t] === !0 && e.updateExclusive(i, t, n);
                    break;
                case 1:
                case 2:
                    e.shuffleLogicShow(i, []);
                    break;
                case 9:
                    i.value[n] = t,
                    e.shuffleLogicShow(i, []);
                    break;
                case 13:
                    i.value[n][t] = !i.value[n][t],
                    e.shuffleLogicShow(i, [])
            }
            var a = i.content.split(".");
            void 0 !== e.quoteAnswerArr[a[0]] && (e.quoteAnswerArr[a[0]] = y(i)),
            e.choiceQuoteRelation[i.id] && angular.forEach(e.choiceQuoteRelation[i.id],
            function (t) {
                e.pages[t.pageIndex].list[t.quesIndex].content_show = 0,
                angular.forEach(i.value,
                function (i, n) {
                    i === !0 ? (e.pages[t.pageIndex].list[t.quesIndex].content_show = 1, e.pages[t.pageIndex].list[t.quesIndex].choice[n].hide = 0) : e.pages[t.pageIndex].list[t.quesIndex].choice[n].hide = 1
                })
            }),
            e.currentQuestion = i,
            e.updateProcess()
        }
    },
    e.changeQuestion = function (i) {
        e.currentQuestion = i
    },
    e.stopPropagation = function (e) {
        e.stopPropagation()
    },
    e.fileUploader = new c({
        url: basePath + "/api/Survey/uploadfile/",
        autoUpload: !0,
        removeAfterUpload: !0,
        alias: "upload_img",
        queueLimit: 4
    }),
    e.fileUploader.onCompleteItem = function (i, t) {
        i.question.value.length < i.question.max && i.question.value.push({
            value: t.img_url
        }),
        e.updateProcess()
    },
    e.deleteQuestionItem = function (i, t, n) {
        t = t.splice(n, 1),
        e.answerQuestion(i)
    },
    e.replaceTitleQuote = function (i) {
        var t = /\[Q[0-9]*[1-9][0-9]*\]/g,
        n = i.match(t);
        if (null === n) return i;
        var a = i;
        return angular.forEach(n,
        function (i) {
            var t = i.substring(2, i.length - 1),
            n = new RegExp("\\[Q" + t + "\\]", "g");
            a = e.quoteAnswerArr[t] ? a.replace(n, e.quoteAnswerArr[t]) : a
        }),
        a
    };
    var h = function () {
        angular.forEach(e.pages,
        function (i, t) {
            i.restTime = 3,
            i.targetTime = 0,
            angular.forEach(i.list,
            function (i, n) {
                if (e.questionMap[i.id] = [t, n], i.passed = !0, i.type_id = parseInt(i.type_id, 10), i.vote_type_id = parseInt(i.vote_type_id, 10), i.logic_hide = parseInt(i.logic_hide, 10), 4 != e.project.status && 10 != i.type_id && i.index && (i.content = i.index + ". " + i.content), i.logic_hide > 0 && (i.logicParent = []), "N" === i.required && i.content.indexOf(". ") >= 0) {
                    var o = i.content.split(". "),
                    r = o[0];
                    o.splice(0, 1),
                    i.content = r + ". 【非必填】" + o.join(". ")
                }
                if ("Y" === i.title_quote) {
                    var c = _(i.content);
                    angular.forEach(c,
                    function (i) {
                        void 0 === e.quoteAnswerArr[i] && (e.quoteAnswerArr[i] = "")
                    })
                }
                if (parseInt(i.choice_quote, 10) > 0 ? (i.content_show = 0, e.choiceQuoteRelation[i.choice_quote] || (e.choiceQuoteRelation[i.choice_quote] = []), e.choiceQuoteRelation[i.choice_quote].push({
                    pageIndex: t,
                    quesIndex: n
                })) : i.content_show = 1, 7 === i.type_id && i.value && angular.forEach(i.choice,
                function (e) {
                    i.value.id === e.id && (i.value = e)
                }), (8 === i.type_id || 15 === i.type_id) && (i.max = parseInt(i.max ? i.max : 0, 10), i.min = parseInt(i.min ? i.min : 0, 10), (i.max > 0 || i.min > 0) && (i.tip = i.min === i.max ? "选择" + i.max + "个选项" : 0 === i.max ? "选择至少" + i.min + "个选项" : 0 === i.min ? "选择至多" + i.max + "个选项" : "选择" + i.min + "-" + i.max + "个选项", i.tip && 0 === i.vote_type_id && (i.tip = " (" + i.tip + ")"))), 12 === i.type_id && (i.max = parseInt(i.max, 10), i.min = "" == i.min ? 0 : parseInt(i.min, 10), (i.max > 0 || i.min > 0) && (i.tip = i.min === i.max ? "上传" + i.max + "张图片" : 0 === i.max ? "最少上传" + i.min + "张图片" : 0 === i.min ? "最多上传" + i.max + "张图片" : "上传" + i.min + "-" + i.max + "张图片", i.tip && 0 === i.vote_type_id && (i.tip = " (" + i.tip + ")"))), (3 === i.vote_type_id || 4 === i.vote_type_id || 14 === i.type_id || 15 === i.type_id) && angular.forEach(i.choice,
                function (e) {
                    e.contentArr = e.content.split("#**#")
                }), 8 === i.vote_type_id) {
                    var u = 0;
                    i.vertical = 0;
                    var l = 0;
                    e.loaded = 0,
                    angular.forEach(i.choice,
                    function (t) {
                        var n = new Image;
                        n.src = t.content,
                        n.onload = function () {
                            l++,
                            n.width > n.height && (a(function () {
                                i.vertical = 1
                            }), u = 1),
                            t.imgHeight = n.height,
                            t.imgWidth = n.width,
                            l == i.choice.length && (angular.forEach(i.choice,
                            function (e) {
                                var i = g(e.imgWidth, e.imgHeight, u);
                                e.content = 1 == u ? "<img src='" + e.content + "' style='max-width: 100%;max-height:100%'/>" : "<img src='" + e.content + "' style='width:" + i.width + "px;height:" + i.height + "px;'/>"
                            }), e.loaded = 1)
                        }
                    })
                } else e.loaded = 1;
                var s = i.content.split(".");
                e.absluteIdOrderMap[i.id] = s[0]
            }),
            s(i),
            1 === i.index && p(i)
        })
    },
    g = function (e, i, t) {
        var n, a;
        return 1 == t ? 849 >= e && 214 >= i ? (n = i, a = e) : 849 >= e ? (n = 214, a = parseFloat(e / i) * n, a > 849 && (a = 849, n = parseFloat(i / e) * a)) : 214 >= i ? (a = 849, n = parseFloat(i / e) * a, n > 214 && (n = 214, a = parseFloat(e / i) * n)) : (a = 849, n = parseFloat(i / e) * a, n > 214 && (n = 214, a = parseFloat(e / i) * n)) : 412 >= e && 600 >= i ? (n = i, a = e) : 412 >= e ? (n = 600, a = parseFloat(e / i) * n, a > 412 && (a = 412, n = parseFloat(i / e) * a)) : 600 >= i ? (a = 412, n = parseFloat(i / e) * a, n > 600 && (n = 600, a = parseFloat(e / i) * n)) : (a = 412, n = parseFloat(i / e) * a, n > 600 && (n = 600, a = parseFloat(e / i) * n)),
        {
            width: a,
            height: n
        }
    },
    v = function () {
        null != e.logic ? angular.forEach(e.pages,
        function (i) {
            angular.forEach(i.list,
            function (i) {
                i.type_id > 5 && i.type_id < 9 && i.logic_condition && i.logic_condition.logic_show_list && angular.forEach(i.choice,
                function (t) {
                    t.selected === !0 && e.shuffleLogicShow(i, t)
                })
            })
        }) : null != e.redirectLogic && (angular.forEach(e.redirectLogic,
        function (i, t) {
            e.redirectQuesArr.push(t)
        }), e.redirectQuesArr.length > 0 && w(e.pages[e.questionMap[e.redirectQuesArr[0]][0]].list[e.questionMap[e.redirectQuesArr[0]][1]]))
    },
    _ = function (e) {
        var i = /\[Q[0-9]*[1-9][0-9]*\]/g,
        t = e.match(i);
        if (null === t) return !1;
        var n = [];
        return angular.forEach(t,
        function (e) {
            var i = e.substring(2, e.length - 1);
            inArray(i, n) || n.push(i)
        }),
        n
    },
    y = function (e) {
        var i = "";
        if (1 !== e.type_id && 2 !== e.type_id || "" === e.value && null === e.value || (i = e.value), 6 === e.type_id && null !== e.value && (i = e.choice[e.value].content, "Y" === e.choice[e.value].is_other && e.choice[e.value].other_content && (i += e.choice[e.value].other_content)), 7 === e.type_id && null !== e.value && (i = e.value.content), 8 === e.type_id) {
            var t = [];
            for (var n in e.value) n < e.value.length && e.value[n] === !0 && t.push(n);
            if (t.length > 0) {
                var a = [];
                for (var n in t) if (n < e.value.length) {
                    var o = e.choice[t[n]].content;
                    "Y" === e.choice[t[n]].is_other && e.choice[t[n]].other_content && (o += e.choice[t[n]].other_content),
                    a.push(o)
                }
                i = a.join("、")
            }
        }
        return i
    };
    e.trustAsHtml = function (e) {
        return i.trustAsHtml(e)
    },
    e.toJsDate = function (e) {
        return e ? new Date(e) : null
    },
    e.formatDate = function (e) {
        if (!e) return null;
        var i = new Date(e),
        t = i.getFullYear() + "年" + (i.getMonth() + 1) + "月" + i.getDate() + "日";
        return t
    },
    e.selectPage = function (i) {
        e.selectedPage = i
    };
    var m = function (i, t) {
        if (e.logic && e.logic[i] && e.logic[i][t]) {
            var n = e.logic[i][t];
            for (var a in n) a = parseInt(a, 10),
            1 === parseInt(n[a], 10) && (e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]].logic_hide = 0, -1 === e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]].logicParent.indexOf(i) && e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]].logicParent.push(i))
        }
    },
    I = function (i, t) {
        if (e.logic && e.logic[i] && e.logic[i][t]) {
            var n = e.logic[i][t];
            for (var a in n) if (a = parseInt(a, 10), 1 === parseInt(n[a], 10)) {
                var o = e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]];
                0 === o.logic_hide && o.logicParent.indexOf(i) > -1 && o.logicParent.length < 2 && (e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]].logic_hide = 1, x(o)),
                o.logicParent.indexOf(i) > -1 && e.pages[e.questionMap[a][0]].list[e.questionMap[a][1]].logicParent.remove(i)
            }
        }
    },
    x = function (e) {
        var i = [];
        6 === e.type_id && null !== e.value && (i.push(e.choice[e.value].id), e.value = null),
        7 === e.type_id && null !== e.value && (i.push(e.value.id), e.value = null),
        8 === e.type_id && angular.forEach(e.value,
        function (t, n) {
            t === !0 && (i.push(e.choice[n].id), e.value[n] = null)
        }),
        angular.forEach(i,
        function (i) {
            I(e.id, i)
        })
    };
    e.shuffleLogicShow = function (i, t) {
        if (e.project.status < 4 || i.logic_condition && i.logic_condition.logic_show_list) {
            for (var n in i.choice) (8 !== i.type_id || 8 === i.type_id && i.value[n] !== !0) && I(i.id, i.choice[n].id);
            if (6 === i.type_id && m(i.id, t.id), 7 === i.type_id && m(i.id, i.value.id), 8 === i.type_id) for (var n in i.value) i.value[n] === !0 && m(i.id, i.choice[n].id);
            e.updateRestTime(e.pages[e.selectedPage - 1])
        }
        if (!e.logic && e.redirectLogic && e.redirectLogic[i.id]) {
            for (var n in i.choice) (8 !== i.type_id && 15 !== i.type_id || (8 === i.type_id || 15 === i.type_id) && i.value[n] !== !0) && w(i);
            if ((6 === i.type_id || 14 === i.type_id) && A(i.id, t.id), 7 === i.type_id && A(i.id, i.value.id), 8 === i.type_id || 15 === i.type_id) {
                var a = q(i);
                a !== !1 && A(i.id, i.choice[a].id)
            } (1 === i.type_id || 2 === i.type_id) && ($.trim(i.value) ? A(i.id, 0) : w(i)),
            9 === i.type_id && A(i.id, 0),
            13 === i.type_id && E(i) && A(i.id, 0),
            e.updateRestTime(e.pages[e.selectedPage - 1])
        }
    };
    var q = function (i) {
        var t, n, a, o = !1,
        r = 0;
        for (var c in i.value) if (i.value[c] === !0) {
            o = !0,
            void 0 === t && (t = c);
            var u = i.choice[c].id;
            if (e.redirectLogic[i.id][u]) {
                var l = e.redirectLogic[i.id][u];
                angular.forEach(l,
                function (i, o) {
                    if (-1 === r) -2 === parseInt(o, 10) && (t = c, r = -2);
                    else if (0 === r) if (parseInt(o, 10) > 0) {
                        var u = e.questionMap[o][0],
                        l = e.questionMap[o][1]; (void 0 === n || void 0 === a || n > u || u === n && a > l) && (n = u, a = l, t = c)
                    } else r = parseInt(o),
                    t = c
                })
            }
        }
        return o === !0 ? t : !1
    },
    E = function (e) {
        var i = !1;
        return angular.forEach(e.value,
        function (e) {
            i || angular.forEach(e,
            function (e) {
                e !== !0 || i || (i = !0)
            })
        }),
        i
    },
    w = function (i) {
        var t = e.questionMap[i.id][0],
        n = e.questionMap[i.id][1];
        angular.forEach(e.pages,
        function (e, i) {
            angular.forEach(e.list,
            function (e, a) {
                (i > t || i === t && a > n) && (e.logic_hide = 1)
            })
        })
    },
    A = function (i, t) {
        if (e.redirectLogic && e.redirectLogic[i]) {
            var n, a, o = !1;
            if (e.redirectLogic[i][t]) {
                var r = e.redirectLogic[i][t];
                angular.forEach(r,
                function (i, t) {
                    parseInt(t, 10) > 0 ? (n = e.questionMap[t][0], a = e.questionMap[t][1], o = !0, e.stopType = 0) : (e.stopType = t, o = !1)
                })
            } else n = e.questionMap[i][0],
            a = e.questionMap[i][1] + 1,
            o = !0,
            e.stopType = 0;
            if (o) {
                var c = !1;
                angular.forEach(e.pages,
                function (i, t) {
                    angular.forEach(i.list,
                    function (i, o) {
                        (t > n || t === n && o >= a) && !c && (i.logic_hide = 0, e.redirectLogic[i.id] && (c = !0))
                    })
                })
            }
        }
    };
    e.updateExclusive = function (e, i, t) {
        if (e.exclusive_options) {
            var n = e.exclusive_options.split(",");
            inArray(t.id.toString(), n) ? (angular.forEach(e.value,
            function (i, t) {
                e.value[t] = !1
            }), e.value[i] = !0) : angular.forEach(n,
            function (i) {
                angular.forEach(e.choice,
                function (t, n) {
                    t.id.toString() === i && (e.value[n] = !1)
                })
            })
        }
    },
    e.getQuoteTip = function (i) {
        var t = "";
        if (parseInt(i.choice_quote, 10) > 0) {
            var n = "";
            switch (parseInt(i.type_id, 10)) {
                case 6:
                    n = "单选题";
                    break;
                case 7:
                    n = "下拉选择题";
                    break;
                case 8:
                    n = "多选题"
            }
            t = "&lt;" + n + "&gt此题目选项来自于第" + e.absluteIdOrderMap[i.choice_quote] + "题中的选项"
        }
        return t
    },
    e.filterHide = function (e) {
        var i = [];
        return angular.forEach(e,
        function (e) {
            e.hide && 0 !== e.hide || i.push(e)
        }),
        i
    };
    var T = function (e) {
        var i = !0,
        t = !0;
        return angular.forEach(e,
        function (e) {
            if (e.passed = !0, "Y" === e.required && 0 === e.logic_hide) {
                if (1 !== e.type_id && 2 !== e.type_id || null !== e.value && "" !== e.value || (i = !1, e.passed = !1), 6 !== e.type_id && 7 !== e.type_id && 14 !== e.type_id || null !== e.value || (i = !1, e.passed = !1), 8 === e.type_id || 15 === e.type_id) {
                    var n = !1,
                    a = 0;
                    angular.forEach(e.value,
                    function (e) {
                        e === !0 && (a += 1, n = !0)
                    }),
                    n === !1 && (i = !1, e.passed = !1),
                    parseInt(e.max, 10) > 0 && a > parseInt(e.max, 10) && (i = !1, e.passed = !1),
                    parseInt(e.min, 10) > 0 && a < parseInt(e.min, 10) && !l(e) && (i = !1, e.passed = !1)
                }
                if (9 === e.type_id && angular.forEach(e.value,
                function (t, n) {
                    e.radio_array_title[n].passed = !0,
                    null === t && (i = !1, e.passed = !1, e.radio_array_title[n].passed = !1)
                }), 13 === e.type_id && angular.forEach(e.value,
                function (t, n) {
                    e.checkbox_array_title[n].passed = !0;
                    var o = !1;
                    angular.forEach(t,
                    function (e) {
                        e === !0 && (a += 1, o = !0)
                }),
                    o === !1 && (i = !1, e.passed = !1, e.checkbox_array_title[n].passed = !1)
                }), 12 === e.type_id) {
                    var a = e.value.length;
                    0 >= a && (i = !1, e.passed = !1),
                    parseInt(e.max, 10) > 0 && a > parseInt(e.max, 10) && (i = !1, e.passed = !1),
                    parseInt(e.min, 10) > 0 && a < parseInt(e.min, 10) && (i = !1, e.passed = !1)
                }
            }
            if (0 === e.logic_hide && e.passed && (6 == e.type_id && angular.forEach(e.choice,
            function (t, n) {
                e.value == n && "Y" == t.is_other && 1 == t.required && (null === t.other_content || "" === $.trim(t.other_content)) && (i = !1, e.passed = !1)
            }), 8 == e.type_id && angular.forEach(e.choice,
            function (t, n) {
                e.value[n] && "Y" == t.is_other && 1 == t.required && (null === t.other_content || "" === $.trim(t.other_content)) && (i = !1, e.passed = !1)
            }), 9 === e.type_id && angular.forEach(e.value,
            function (t, n) {
                if (null !== t && t in e.choice) {
                    var a = e.choice[t];
                    if ("Y" == a.is_other && 1 == a.required) {
                        var o = e.radio_array_title[n].other_content[t]; (null === o || "" === $.trim(o)) && (i = !1, e.passed = !1, e.radio_array_title[n].passed = !1)
            }
            }
            }), 13 === e.type_id && angular.forEach(e.value,
            function (t, n) {
                angular.forEach(e.choice,
                function (a, o) {
                    if (t[o] && "Y" == a.is_other && 1 == a.required) {
                        var r = e.checkbox_array_title[n].other_content[o]; (null === r || "" === $.trim(r)) && (i = !1, e.passed = !1, e.checkbox_array_title[n].passed = !1)
            }
            })
            })), t && !e.passed) {
                t = !1;
                var o = "survey-question-" + e.page + "-" + e.order;
                u(o)
            }
        }),
        i
    };
    e.checkQuestion = function (e) {
        var i = !0;
        if (0 === e.logic_hide) {
            if (1 !== e.type_id && 2 !== e.type_id && 3 !== e.type_id || null !== e.value && "" !== e.value || (i = !1), 6 !== e.type_id && 7 !== e.type_id && 14 !== e.type_id || null !== e.value || (i = !1), 8 === e.type_id || 15 === e.type_id) {
                var t = !1,
                n = 0;
                angular.forEach(e.value,
                function (e) {
                    e === !0 && (n += 1, t = !0)
                }),
                t === !1 && (i = !1),
                parseInt(e.max, 10) > 0 && n > parseInt(e.max, 10) && (i = !1),
                parseInt(e.min, 10) > 0 && n < parseInt(e.min, 10) && !l(e) && (i = !1)
            }
            if (9 === e.type_id && angular.forEach(e.value,
            function (e) {
                null === e && (i = !1)
            }), 13 === e.type_id && angular.forEach(e.value,
            function (e) {
                var t = !1;
                angular.forEach(e,
                function (e) {
                    e === !0 && (n += 1, t = !0)
            }),
                t === !1 && (i = !1)
            }), 12 === e.type_id) {
                var n = e.value.length;
                0 >= n && (i = !1),
                parseInt(e.max, 10) > 0 && n > parseInt(e.max, 10) && (i = !1),
                parseInt(e.min, 10) > 0 && n < parseInt(e.min, 10) && (i = !1)
            }
        }
        return i
    },
    e.checkNextPage = function (i) {
        var t = T(i);
        t === !0 && (e.selectPage(e.selectedPage + 1), p(e.pages[e.selectedPage - 1]), u("survey-navigation"))
    },
     e.submit = function (e) {
         var i = A(e);
         i === !0 && (location.href = basePath + "Survey/Survey/FinishSelfCollectionSurvey")
     },
    e.submitSelf = function (e) {
        var i = A(e);
        i === !0 && (location.href = basePath + "Survey/Survey/FinishSelfCollectionSurvey")
    },
    e.updateRestTime = function (i) {
        s(i),
        i.restTime <= 0 && i.targetTime > e.runningTime && d(i)
    };
    var M = 0;
    e.initPageTimer = function (e) {
        p(e)
    };
    e.questionTypeClass = function (e) {
        var i, t = e.vote_type_id;
        return 6 == t ? i = "input-type" : 7 == t ? i = "text-type" : 8 == t && (i = 1 == e.vertical ? "image-type vertical-type" : "image-type horizon-type"),
        i
    }
}]);