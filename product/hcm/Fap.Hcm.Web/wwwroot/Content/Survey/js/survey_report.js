var surveyReport = angular.module("surveyReport", ["infinite-scroll"]);
surveyReport.controller("surveyReportController", ["$http", "$scope", "$q", "$timeout",
function (e, t, r, i) {
    var n = {
        4: "发布中",
        5: "已关闭",
        8: "暂停发布"
    };
    t.status = project.status,
    t.statusName = n[t.status],
    t.online_time = project.online_time,
    t.volid_count = project.volid_count,
    t.invalidUnsubmit = invalid_info.invalidUnsubmit,
    t.invalidQualification = invalid_info.invalidQualification,
    t.invalidTime = invalid_info.invalidTime,
    t.invalidFilter = invalid_info.invalidFilter,
    t.report_list = [],
    t.order = 0,
    t.report_list_busy = !1,
    t.chartArray = [{
        id: 1,
        title: "饼图"
    },
    {
        id: 2,
        title: "条形图"
    }],
    t.selectedChartArray = 1,
    t.itemsPerPage = 10,
    t.currentPage = 0,
    t.itemCount = 0,
    t.emptyList = !0,
    t.sortArray = [{
        id: 0,
        title: "默认排序"
    },
    {
        id: 1,
        title: "升序排列"
    },
    {
        id: 2,
        title: "降序排列"
    }],
    t.selectedSortArray = 0,
    t.filterAreaOnOff = !1,
    t.reportFilter = report_filter,
    t.filterListLength = t.reportFilter.length,
    t.surveyQuestions = [],
    t.conditions = [{
        id: "0",
        content: "请选择"
    },
    {
        id: "1",
        content: "选择了"
    },
    {
        id: "2",
        content: "没选择"
    }],
    t.choices = [],
    t.filterError = "",
    t.allQuestionsCount = 1,
    t.exportUrl = basePath + "/System/Api/Survey/ExportReport/" + project.id, //+ "/filter/1",
    t.downloadUrl = basePath + "api/Survey/getSurveyReportDataDownload/survey_id/" + project.id,
    t.exportStatus = 0;
    var s = function () {
        var e = {};
        e.id = 0,
        e.content = "请选择题目",
        t.surveyQuestions.push(e),
        $.each(survey_questions,
        function (e, r) {
            (6 == r.type_id || 7 == r.type_id || 8 == r.type_id || 14 == r.type_id || 15 == r.type_id) && t.surveyQuestions.push(r)
        }),
        $.each(t.reportFilter,
        function (e, r) {
            $.each(survey_questions,
            function (e, i) {
                i.id == r.question_id && (t.choices[i.id] = [], t.choices[i.id].push({
                    id: 0,
                    content: "请选择"
                }), t.choices[i.id] = t.choices[i.id].concat(i.choice))
            })
        })
    };
    s(),
    t.addFilter = function () {
        var e = t.filterComplete();
        if (1 == e) {
            var r = {};
            r.id = 9999,
            r.question_id = 0,
            r.condition_id = "0",
            r.choice_id = 0,
            r.title_id = 0,
            t.reportFilter.push(r),
            t.filterError = ""
        }
        t.filterListLength = t.reportFilter.length
    },
    t.choseFilterQuestion = function (e) {
        $.each(t.surveyQuestions,
        function (r, i) {
            i.id == e.question_id && (e.type_id = i.type_id, 9 == e.type_id && (e.title_id = i.radio_array_title_id), t.choices[i.id] = [], t.choices[i.id].push({
                id: 0,
                content: "请选择"
            }), t.choices[i.id] = t.choices[i.id].concat(i.choice))
        })
    },
    t.deleteReportFilter = function (r) {
        require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (i) {
            i.show("删除规则后将不可恢复，是否继续？", "确定", "取消").then(function () {
                var url = basePath + "/System/Api/Survey/DeleteReportFilter";
                $.post(url, r, function () {
                    for (var e = 0; e < t.reportFilter.length; e++) t.reportFilter[e].id == r.id && (t.reportFilter.splice(e, 1), t.filterListLength = t.reportFilter.length)
                })
                //e.post(i, r).success(function () {
                //    for (var e = 0; e < t.reportFilter.length; e++) t.reportFilter[e].id == r.id && (t.reportFilter.splice(e, 1), t.filterListLength = t.reportFilter.length)
                //})
            })
        })
    },
    t.setFilter = function () {
        1 == t.filterAreaOnOff ? t.filterCancel() : (0 === t.filterListLength && t.addFilter(), t.filterAreaOnOff = !0)
    },
    t.filterCancel = function () {
        var r = basePath + "/System/Api/Survey/ReportFilter/" + project.id;
        e.get(r).success(function (e) {
            t.reportFilter = e,
            t.filterAreaOnOff = !1,
            t.filterListLength = t.reportFilter.length
        })
    },
    t.filterSave = function () {
        var r = t.filterComplete();
        if (1 == r) {
            var url = basePath + "/System/Api/Survey/ReportFilter",
            n = {};
            n.survey_id = project.id,
                n.filters = JSON.stringify(t.reportFilter),
                n.YII_CSRF_TOKEN = FAP.surveyToken;
            $.post(url, n, function (e) {
                1 == e.error && (t.report_list = [], t.order = 0, t.filterAreaOnOff = !1, t.reportFilter = e.filters, t.filterListLength = t.reportFilter.length)
            })
            //    e.post(i, n).success(function (e) {
            //    1 == e.error && (t.report_list = [], t.order = 0, t.filterAreaOnOff = !1, t.reportFilter = e.filters, t.filterListLength = t.reportFilter.length)
            //})
        }
        $body = $(window.opera ? "CSS1Compat" == document.compatMode ? "html" : "body" : "html,body"),
        $body.animate({
            scrollTop: $(".survey_report_area").offset().top
        },
        1e3)
    },
    t.filterComplete = function () {
        var e = 1;
        $.each(t.reportFilter,
        function (t, r) {
            return r.question_id != 0 && r.condition_id > 0 && r.choice_id!= 0 || (e = 2),
            e > 1 ? !1 : void 0
        });
        var r = t.reportFilter.slice(0);
        r.sort(function (e, t) {
            return e.question_id < t.question_id ? 1 : e.question_id > t.question_id ? -1 : 0
        });
        for (var i = 0; i < r.length - 1; i++) 1 == e && r.length > 1 && r[i].question_id == r[i + 1].question_id && r[i].condition_id == r[i + 1].condition_id && r[i].choice_id == r[i + 1].choice_id && (e = 3);
        return 2 == e ? t.filterError = "筛选内容不可为空，请完整填写。" : 3 == e && (t.filterError = "出现重复筛选条件，请确认。"),
        e
    },
    t.chartChange = function (e, r) {
        e.selectedChartArray = r,
        1 === r ? e.selectedSortArray ? t.constructionPie(e, e.response_single, e.selectedSortArray) : t.constructionPie(e, e.response_single, t.selectedSortArray) : 2 === r && (e.selectedSortArray ? (o(e, e.response_single, e.selectedSortArray), t.constructionBar(e, e.response_single, e.selectedSortArray)) : (o(e, e.response_single, t.selectedSortArray), t.constructionBar(e, e.response_single, t.selectedSortArray)))
    },
    t.sortChange = function (e, r) {
        e.selectedSortArray = r;
        var i;
        i = e.selectedChartArray ? e.selectedChartArray : 1,
        2 == r ? 6 == e.type_id || 7 == e.type_id || 14 == e.type_id ? (e.response_single.choice.sort(function (e, t) {
            return e.selected < t.selected ? 1 : e.selected > t.selected ? -1 : 0
        }), 2 == i ? t.constructionBar(e, e.response_single, r) : 1 == i && t.constructionPie(e, e.response_single, r)) : (8 == e.type_id || 15 == e.type_id) && (o(e, e.response_multiple), e.response_multiple.choice.sort(function (e, t) {
            return e.selected < t.selected ? 1 : e.selected > t.selected ? -1 : 0
        }), t.constructionBar(e, e.response_multiple, r)) : 1 == r ? 6 == e.type_id || 7 == e.type_id || 14 == e.type_id ? (e.response_single.choice.sort(function (e, t) {
            return e.selected > t.selected ? 1 : e.selected < t.selected ? -1 : 0
        }), 2 == i ? t.constructionBar(e, e.response_single, r) : 1 == i && t.constructionPie(e, e.response_single, r)) : (8 == e.type_id || 15 == e.type_id) && (o(e, e.response_multiple), e.response_multiple.choice.sort(function (e, t) {
            return e.selected > t.selected ? 1 : e.selected < t.selected ? -1 : 0
        }), t.constructionBar(e, e.response_multiple, r)) : 6 == e.type_id || 7 == e.type_id || 14 == e.type_id ? (e.response_single.choice.sort(function (e, t) {
            return e.order < t.order ? 1 : e.order > t.order ? -1 : 0
        }), 2 == i ? t.constructionBar(e, e.response_single, r) : 1 == i && t.constructionPie(e, e.response_single, r)) : (8 == e.type_id || 15 == e.type_id) && (o(e, e.response_multiple), e.response_multiple.choice.sort(function (e, t) {
            return e.order < t.order ? 1 : e.order > t.order ? -1 : 0
        }), t.constructionBar(e, e.response_multiple, r))
    },
    t.exportData = function () {
        //surveyTool.createSurveyActionLog("surveyReport", "export", "buttonClick", project.id),
        require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (r) {
            r.show("导出数据耗时较长，请【不要】跳转或关闭页面！", "知道啦", null).then(function () {
                e.get(t.exportUrl).success(function (e) {
                    $("#export-data").val("正在导出").attr("disabled", "disabled").addClass("disabled"),
                    t.exportStatus = 0,
                    0 === e.error_code ? (t.exportStatus = 1, $("#export-data").val("导出统计报告").removeAttr("disabled").removeClass("disabled"), location.href = basePath + "/" + e.fn) : require.async(["home:static/js/survey/widget/operate_popup.js"],
                    function (e) {
                        e.show("导出数据请求失败，请重新导出！", "确定", null)
                    })
                })
            })
        })
    },
    t.getReportData = function () {
        if (t.allQuestionsCount > t.report_list.length) {
            t.report_list_busy = !0;
            //var r = basePath + "api/Survey/getSurveyReportData?survey_id=" + project.id + "&order=" + t.order;
            var r = basePath + "/System/Api/Survey/Report/" + project.id + "/" + t.order;
            e.get(r).success(function (e) {
                e.question && e.question[0] && (9 == e.question[0].type_id && (e.question[0].choice_length = e.question[0].choice.length + 1), 14 == e.question[0].type_id && angular.forEach(e.question[0].response_single.choice,
                function (e) {
                    e.contentArr = e.content.split("#**#"),
                    e.content = e.contentArr[0]
                }), 15 == e.question[0].type_id && angular.forEach(e.question[0].response_multiple.choice,
                function (e) {
                    e.contentArr = e.content.split("#**#"),
                    e.content = e.contentArr[0]
                }), 22833 == e.question[0].id && (e.question[0].response_single.answered = 248, e.question[0].response_single.choice[0].selected = 180, e.question[0].response_single.choice[0].selected_percent = "72.58%", e.question[0].response_single.choice[1].selected = 30, e.question[0].response_single.choice[1].selected_percent = "12.10%", e.question[0].response_single.choice[2].selected = 25, e.question[0].response_single.choice[2].selected_percent = "10.08%", e.question[0].response_single.choice[3].selected = 13, e.question[0].response_single.choice[3].selected_percent = "5.24%"), t.report_list.push(e.question[0]), i(function () {
                    if (t.order = e.question[0].order, t.allQuestionsCount = e.question[0].all_questions_count, e.question[0].content = e.question[0].content.replace(/<\/?[^>]*>/g, ""), 6 == e.question[0].type_id || 7 == e.question[0].type_id || 14 == e.question[0].type_id) t.constructionPie(e.question[0], e.question[0].response_single, t.selectedSortArray);
                    else if (8 == e.question[0].type_id || 15 == e.question[0].type_id) o(e.question[0], e.question[0].response_multiple),
                    t.constructionBar(e.question[0], e.question[0].response_multiple, t.selectedSortArray);
                    else if (9 == e.question[0].type_id) {
                        if (e.question[0].response_radio_array.length > 6) {
                            var r = 30 * (e.question[0].response_radio_array.length - 6);
                            $("#data-chart-radio" + e.question[0].id).css({
                                height: r + 400
                            }),
                            $("#data-chart-radio" + e.question[0].id).siblings(".data-form").css({
                                "padding-top": r + 470
                            })
                        }
                        t.constructionBarMuli(e.question[0])
                    } else if (13 == e.question[0].type_id) {
                        if (e.question[0].response_checkbox_array.length > 6) {
                            var r = 30 * (e.question[0].response_checkbox_array.length - 6);
                            $("#data-chart-radio" + e.question[0].id).css({
                                height: r + 400
                            }),
                            $("#data-chart-radio" + e.question[0].id).siblings(".data-form").css({
                                "padding-top": r + 470
                            })
                        }
                        e.question[0].response_radio_array = e.question[0].response_checkbox_array,
                        t.constructionBarMuli(e.question[0])
                    }
                    t.report_list_busy = !1
                }))
            })
        }
    };
    var o = function (e, t) {
        var r = 0;
        t.choice.length > 6 && (r = 30 * (t.choice.length - 6)),
        $("#data-chart-radio" + e.id).css({
            height: r + 400
        }),
        $("#data-chart-radio" + e.id).siblings(".data-form").css({
            "padding-top": r + 470
        })
    },
    a = function (e) {
        for (var t = 0,
        r = 0; r < e.length; r++) t += null != e[r].match(/[^\x00-\xff]/gi) ? 2 : 1;
        return t
    },
    c = function (e, t) {
        var r = "",
        i = e.length,
        n = t,
        s = Math.ceil(i / n);
        if (i > n) for (var o = 0; s > o; o++) {
            var a = "",
            c = o * n,
            l = c + n;
            a = o == s - 1 ? e.substring(c, i) : e.substring(c, l) + "\n",
            r += a
        } else r = e;
        return r
    };
    t.constructionPie = function (e, t, r) {
        var i = {
            title: {
                x: "center"
            },
            legend: {
                x: "center",
                y: "bottom",
                padding: 5,
                data: [],
                selected: {},
                formatter: function (e) {
                    return c(e, 30)
                }
            },
            toolbox: {
                show: !0,
                x: "right",
                y: "5",
                feature: {
                    saveAsImage: {
                        show: !0,
                        type: "jpg",
                        lang: ["点击保存"],
                        color: "#333"
                    }
                }
            },
            series: [{
                type: "pie",
                itemStyle: {
                    normal: {
                        label: {
                            show: !0
                        }
                    }
                },
                radius: "55%",
                center: ["50%", "50%"],
                data: []
            }]
        };
        date = new Date,
        i.toolbox.feature.saveAsImage.name = "chart-" + date.pattern("yyyyMMddhhmm");
        var n = 0;
        $.each(t.choice,
        function (e, t) {
            var r = {};
            r.value = t.selected;
            var s = t.content.replace(/<\/?[^>]*>/g, "");
            r.name = s,
            i.series[0].data.push(r),
            i.legend.data.push(s),
            "0%" === t.selected_percent && (i.legend.selected[s] = !1),
            n += a(s) + 6
        }),
        1 == r ? i.series[0].data.sort(function (e, t) {
            return e.selected > t.selected ? -1 : e.selected < t.selected ? 1 : 0
        }) : 2 == r && i.series[0].data.sort(function (e, t) {
            return e.selected < t.selected ? -1 : e.selected > t.selected ? 1 : 0
        }),
        $("#data-chart-radio" + e.id[0]).css({
            height: n / 150 * 40 + 400
        }),
        $("#data-chart-radio" + e.id[0]).css({
            width: n / 150 * 80 + 770
        }),
        $("#data-chart-radio" + e.id[0]).siblings(".data-form").css({
            "padding-top": n / 150 * 40 + 470
        }),
        i.series[0].radius = 55 - n / 150 * 10 + "%",
        i.series[0].itemStyle.normal.label = {
            show: !0,
            formatter: function (e, t, r, i) {
                var n = t + ":(" + i + "%)";
                return c(n, 20)
            }
        };
        var s = echarts.init(document.getElementById("data-chart-radio" + e.id[0]));
        s.setOption(i),
        window.onresize = s.resize
    },
    t.constructionBar = function (e, t, r) {
        var i = {
            title: {
                x: "center"
            },
            tooltip: {
                trigger: "axis",
                axisPointer: {
                    type: "shadow"
                },
                formatter: "{b}:({c}%)"
            },
            toolbox: {
                show: !0,
                x: "right",
                y: "5",
                feature: {
                    saveAsImage: {
                        show: !0,
                        type: "jpg",
                        lang: ["点击保存"],
                        color: "#333"
                    }
                }
            },
            grid: {
                x: 200
            },
            xAxis: [{
                type: "value",
                boundaryGap: [0, .01],
                min: 0,
                max: 100
            }],
            yAxis: [{
                type: "category",
                inteval: "auto",
                data: []
            }],
            series: [{
                type: "bar",
                barCategoryGap: "30%",
                itemStyle: {
                    normal: {
                        label: {
                            show: !0,
                            position: "right",
                            formatter: "{c}%"
                        }
                    }
                },
                data: []
            }]
        },
        n = echarts.init(document.getElementById("data-chart-radio" + e.id[0]));
        if (date = new Date, i.toolbox.feature.saveAsImage.name = "chart-" + date.pattern("yyyyMMddhhmm"), 2 == r) {
            var s = [];
            $.each(t.choice,
            function (e, t) {
                var r = parseFloat(t.selected_percent);
                s.push({
                    content: t.content,
                    selected: r
                })
            }),
            s.sort(function (e, t) {
                return e.selected < t.selected ? -1 : e.selected > t.selected ? 1 : 0
            }),
            $.each(s,
            function (e, t) {
                i.yAxis[0].data.push(t.content.replace(/<\/?[^>]*>/g, "")),
                i.series[0].data.push(t.selected)
            })
        } else if (1 == r) {
            var s = [];
            $.each(t.choice,
            function (e, t) {
                var r = parseFloat(t.selected_percent);
                s.push({
                    content: t.content,
                    selected: r
                })
            }),
            s.sort(function (e, t) {
                return e.selected > t.selected ? -1 : e.selected < t.selected ? 1 : 0
            }),
            $.each(s,
            function (e, t) {
                i.yAxis[0].data.push(t.content.replace(/<\/?[^>]*>/g, "")),
                i.series[0].data.push(t.selected)
            })
        } else $.each(t.choice,
        function (e, t) {
            var r = parseFloat(t.selected_percent);
            i.yAxis[0].data.push(t.content.replace(/<\/?[^>]*>/g, "")),
            i.series[0].data.push(r)
        });
        i.yAxis[0].data = i.yAxis[0].data.reverse(),
        i.series[0].data = i.series[0].data.reverse(),
        i.yAxis[0].axisLabel = {
            interval: 0,
            formatter: function (e) {
                return c(e, 15)
            }
        },
        n.setOption(i),
        window.onresize = n.resize
    },
    t.constructionBarMuli = function (e) {
        var t = {
            title: {
                x: "center"
            },
            legend: {
                x: "center",
                y: "bottom",
                data: [],
                formatter: function (e) {
                    return c(e, 20)
                }
            },
            tooltip: {
                trigger: "axis",
                axisPointer: {
                    type: "shadow"
                },
                formatter: function (e) {
                    var t = e[0].name + ":<br>";
                    return $.each(e,
                    function (e, r) {
                        t += r.seriesName + "(" + r.data + "%)<br>"
                    }),
                    t
                }
            },
            toolbox: {
                show: !0,
                x: "right",
                y: "5",
                feature: {
                    saveAsImage: {
                        show: !0,
                        type: "jpg",
                        lang: ["点击保存"],
                        color: "#333"
                    }
                }
            },
            grid: {
                x: 200
            },
            xAxis: [{
                type: "value",
                min: 0
            }],
            yAxis: [{
                type: "category",
                inteval: "auto",
                data: []
            }],
            series: []
        },
        r = echarts.init(document.getElementById("data-chart-radio" + e.id[0]));
        date = new Date,
        t.toolbox.feature.saveAsImage.name = "chart-" + date.pattern("yyyyMMddhhmm");
        var i = e.response_radio_array,
        n = e.choice,
        s = [];
        $.each(i,
        function (e, r) {
            t.yAxis[0].data.push(r.content.replace(/<\/?[^>]*>/g, "")),
            $.each(r.choice,
            function (e, t) {
                s[e] || (s[e] = []),
                s[e].push(parseFloat(t.selected_percent))
            })
        }),
        $.each(n,
        function (e, r) {
            r.content = r.content.replace(/<\/?[^>]*>/g, ""),
            r.content = r.content.replace(/[\n]/g, ""),
            t.legend.data.push(r.content);
            var i = {};
            i.name = r.content.replace(/<\/?[^>]*>/g, ""),
            i.type = "bar",
            i.stack = "总量",
            i.barCategoryGap = "30%",
            i.itemStyle = {
                normal: {
                    label: {
                        show: !0,
                        position: "inside",
                        formatter: "{c}%"
                    }
                }
            },
            i.data = s[e],
            t.series.push(i)
        }),
        t.yAxis[0].axisLabel = {
            interval: 0,
            formatter: function (e) {
                return c(e, 15)
            }
        },
        r.setOption(t),
        window.onresize = r.resize
    },
    t.pause = function () {
        var r = basePath + "home/sendStatus",
        i = {
            survey_id: project.id,
            action: "pause",
            YII_CSRF_TOKEN: FAP.surveyToken
        };
        e.post(r, i).success(function (e) {
            0 === e.error_code ? (t.status = "8", t.statusName = n[t.status]) : require.async(["home:static/js/survey/widget/operate_popup.js"],
            function (e) {
                e.show("问卷暂停失败，请重新暂停！", "确定", null)
            })
        })
    },
    t.publish = function () {
        var r = basePath + "home/sendStatus",
        i = {
            survey_id: project.id,
            action: "publish",
            YII_CSRF_TOKEN: FAP.surveyToken
        };
        e.post(r, i).success(function (e) {
            0 === e.error_code ? (t.status = "4", t.statusName = n[t.status]) : require.async(["home:static/js/survey/widget/operate_popup.js"],
            function (e) {
                e.show("问卷发布失败，请重新发布！", "确定", null)
            })
        })
    },
    Date.prototype.pattern = function (e) {
        var t = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12,
            "H+": this.getHours(),
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            S: this.getMilliseconds()
        },
        r = {
            0: "/u65e5",
            1: "/u4e00",
            2: "/u4e8c",
            3: "/u4e09",
            4: "/u56db",
            5: "/u4e94",
            6: "/u516d"
        };
        /(y+)/.test(e) && (e = e.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length))),
        /(E+)/.test(e) && (e = e.replace(RegExp.$1, (RegExp.$1.length > 1 ? RegExp.$1.length > 2 ? "/u661f/u671f" : "/u5468" : "") + r[this.getDay() + ""]));
        for (var i in t) new RegExp("(" + i + ")").test(e) && (e = e.replace(RegExp.$1, 1 == RegExp.$1.length ? t[i] : ("00" + t[i]).substr(("" + t[i]).length)));
        return e
    }
}]),
surveyReport.filter("unsafe", ["$sce",
function (e) {
    return function (t) {
        return e.trustAsHtml(t)
    }
}]),
surveyReport.filter("removeHtml", ["$sce",
function (e) {
    return function (t) {
        return t = t.replace(/<\/?[^>]*>/g, ""),
        e.trustAsHtml(t)
    }
}]),
surveyReport.filter("offset",
function () {
    return function (e, t) {
        return t = parseInt(t, 10),
        e.slice(t)
    }
}),
surveyReport.directive("surveyPagination",
function () {
    return {
        restrict: "EA",
        replace: !0,
        templateUrl: "pagination.html",
        link: function (e) {
            e.pages = [0],
            e.pageInit = function (t) {
                e.itemsPerPage = 12 == t.type_id ? 1 : 10,
                e.itemCount = t.response_text.response_list.length
            },
            e.setPage = function (t) {
                e.currentPage = t,
                e.pages = [];
                var r = e.pageCount(),
                i = t - 2 - (t + 3 > r ? t + 3 - r : 0);
                0 > i && (i = 0);
                var n = i + 4;
                n >= r && (n = r - 1);
                for (var s = i; n >= s; s += 1) e.pages.push(s)
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
surveyReport.run(["$templateCache",
function (e) {
    var t = '<div class="pagination" style="text-align: center; float: right; height: 30px; margin: 0px;" ng-class="pageInit({{report}})"><ul><li ng-class="prevPageDisabled()"><a href="" ng-click="setPage(0)">&lt;&lt;</a></li><li ng-class="prevPageDisabled()"><a href="" ng-click="prevPage()">&lt;</a></li><li ng-repeat="n in pages"            ng-class="{active: n == currentPage}" ng-click="setPage(n)"><a href="" ng-bind="{{n+1}}"></a></li><li ng-class="nextPageDisabled()"><a href="" ng-click="nextPage()">&gt;</a><li ng-class="nextPageDisabled()"><a href="" ng-click="setPage(pageCount()-1)">&gt;&gt;</a></li></li></ul></div>';
    e.put("pagination.html", t)
}]);