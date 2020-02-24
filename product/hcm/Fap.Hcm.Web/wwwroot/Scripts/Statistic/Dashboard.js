var template = {
    widgetContainer: ["<div class=\"col-xs-12 col-sm-6 widget-container-col\">",
"<div class=\"widget-box\" id=\"###\">",
"    <div class=\"widget-header widget-header-flat widget-header-small\">",
"        <h5 class=\"widget-title\">",
"            <i class=\"ace-icon fa \"></i>",
"           ",
"        </h5>",
"        <div class=\"widget-toolbar\">",
"            <a href=\"#\" data-action=\"widgetsettings\">",
"                <i class=\"ace-icon fa fa-cog\"></i>",
"            </a>",
"            <a href=\"#\" data-action=\"widgetreload\"  class=\"green\">",
"                <i class=\"ace-icon fa fa-refresh\"></i>",
"            </a>",
"            <a href=\"#\" data-action=\"widgetremove\"  class=\"red\">",
"                <i class=\"ace-icon fa fa-times\"></i>",
"            </a>",
"            <a href=\"#\" data-action=\"fullscreen\" class=\"orange2\">",
"                <i class=\"ace-icon fa fa-expand\"></i>",
"            </a>",
"        </div>",
"    </div>",
"    <div class=\"widget-body\">",
"        <div class=\"widget-main\">",
"         ",
"        </div><!-- /.widget-main -->",
"    </div><!-- /.widget-body -->",
"</div><!-- /.widget-box -->",
"</div>"].join("")

}
var defaults = {
    cid:"",//控件id
    fid: "",//图表配置FID
    editable:false,//是否可编辑
    icon: "fa-signal",//图标
    title:"未命名",//标题
    container: "#widgetContainer",//容器
    url: basePath + '/api/report/chartsSource/',
    data:{},
    refresh: function () { },
    settings: function () { },
    remove: function () { }
};

var DashboardDatatableWidget = function (options) {
    options = $.extend({}, defaults, options);
    debugger
    var fid = options.fid;
    var cid = options.cid;
    if (cid == "") {
        cid = (new Date()).valueOf();
    }
    var $gridTemplate = $(template.widgetContainer.replace(/###/, "widget-box-" + cid));
    $gridTemplate.find(".widget-title").append(options.title);
    $gridTemplate.find(".widget-title i").removeClass().addClass("ace-icon fa " + options.icon);
    $(options.container).append($gridTemplate);

    var $ctrlWidget = $("#widget-box-" + cid);
    //初始化
    var loadDashboadWidget = function (rv) {
        var $widgetMain = $ctrlWidget.find(".widget-main");
        $widgetMain.empty();
        if (!rv) {
            $.ajax({
                url: basePath + "/api/report/GetDashboardByFid/" + options.fid,    //请求的url地址
                dataType: "json",   //返回格式为json
                async: false, //请求是否异步，默认为异步，这也是ajax重要特性
                data: { "id": "value" },    //参数值
                type: "GET",   //请求方式
                beforeSend: function () {
                    //请求前的处理
                },
                success: function (req) {
                    //请求成功时处理
                    rv = req;
                }
            });
        }
        var ss = JSON.parse(rv.statSet);
        var $table = $("<table  class=\"table table-striped table-bordered table-hover\"> </table>");
        var $thead = $("<thead></thead>");
        var $tr = $("<tr></tr>");
        var columns = [];
        if (!ss || !ss.fields) {
            return;
        }
        $.each(ss.fields, function (i, d) {
            $tr.append("<th>" + d.text + "</th>");
            columns.push({ "data": d.tablename + "_" + d.name });
        })
        $thead.append($tr);
        $table.append($thead);
        $widgetMain.append($table);
        var dtboptions = {
            "ajax": basePath + "/api/report/gridSource/" + options.fid,
            "bPaginate": true, //翻页功能 
            "bLengthChange": false, //改变每页显示数据数量  
            "bFilter": false, //过滤功能  
            "bSort": true, //排序功能  
            "bInfo": true,//页脚信息 
            "bAutoWidth": true,
            "sScrollX": "100%",
            "sScrollXInner": "220%",
            "bScrollCollapse": true,
            "iDisplayLength": 10,
            "oLanguage": {
                "sLengthMenu": "每页显示 _MENU_ 条记录",
                "sZeroRecords": "抱歉， 没有找到",
                "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
                "sInfoEmpty": "没有数据",
                "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
                "oPaginate": {
                    "sFirst": "首页",
                    "sPrevious": "前一页",
                    "sNext": "后一页",
                    "sLast": "尾页"
                },
                "sZeroRecords": "没有检索到数据",
            },
            "columns": columns
        };
        if (columns && columns.length <= 5) {
            dtboptions.sScrollXInner = "100%";
        }
        $table.wrap("<div class='dataTables_borderWrap' />").dataTable(dtboptions);

    }
    $ctrlWidget.find(".widget-toolbar a[data-action=widgetreload]").on(ace.click_event, function (e) {
        e.preventDefault();
        if (options.refresh && $.isFunction(options.refresh)) {
            options.refresh.call(this, options.fid);
        }
        loadDashboadWidget();
    })
    //loadDashboadWidget();
    if (options.editable) {
        //设置
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetsettings]").on(ace.click_event, function (e) {
            e.preventDefault();
            if (options.settings && $.isFunction(options.settings)) {
                options.settings.call(this, options.fid);
            }
            var dialog = bootbox.dialog({
                title: "数据表设置",
                message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
                buttons: {
                    success: {
                        label: $.lang("global_oper_enter", "确定"),
                        className: "btn-primary",
                        callback: function () {
                            var csmodel = getResult();
                            csmodel.fid = options.fid;
                            $.post(basePath + "/api/report/updateDashboard", csmodel, function (rv) {
                                if (rv) {
                                    $.msg("success");
                                    loadDashboadWidget(rv);
                                }
                            })

                        }
                    },
                    cancel: {
                        label: "关闭", className: "btn-default"
                    }
                }

            });
            dialog.init(function () {
                dialog.find('.bootbox-body').load(basePath + "Statistic/ReportDesigner/GridOptions?fid=" + options.fid)
                //var $this = $(this);
                dialog.find('.bootbox-body').ace_scroll({
                    size: 450
                    //styleClass: 'scroll-left scroll-margin scroll-thin scroll-dark scroll-light no-track scroll-visible'
                });

            });
        })

        $ctrlWidget.find(".widget-toolbar a[data-action=widgetremove]").on(ace.click_event, function (e) {
            e.preventDefault();
            if (options.remove && $.isFunction(options.remove)) {
                options.remove.call(this, options.fid);
            }
            var $this = $(this);
            bootbox.confirm("确认要删除吗？", function (result) {
                if (result) {
                    $.get(basePath + "/api/report/deleteDashboard/" + options.fid, function (rv) {
                        if (rv) {
                            $this.closest(".widget-box").remove();
                        } else {
                            $.msg("删除失败！");
                        }
                    })
                }
            })
        })
    } else {
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetsettings]").remove();
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetremove]").remove();
    }

    
    return loadDashboadWidget;
}

var DashboardEChartsWidget = function (options) {
    options = $.extend({}, defaults, options);
    debugger
    var fid = options.fid;
    var cid = options.cid;
    if (cid == "")
    {
        cid=(new Date()).valueOf(); 
    }
    var $gridTemplate = $(template.widgetContainer.replace(/###/, "widget-box-" + cid));
    $gridTemplate.find(".widget-title").append(options.title);
    $gridTemplate.find(".widget-title i").removeClass().addClass("ace-icon fa " + options.icon);
    $(options.container).append($gridTemplate);
    var $ctrlWidget = $("#widget-box-" + cid);
    //初始化
    var loadDashboadWidget = function () {
        var $widgetMain = $ctrlWidget.find(".widget-main");
        $widgetMain.empty();
        var $echart = $("<div  style=\"width: 550px;height:350px;\"></div>");
        $widgetMain.append($echart);

        var myChart = echarts.init($echart.get(0));
        $.get(options.url + fid, options.data).done(function (option) {
            myChart.setOption(option);

            var $box = $widgetMain.closest('.widget-box');
            //全屏图表自适应大小
            $box.on("fullscreened.ace.widget", function () {
                myChart.resize({
                    width: $widgetMain.width(),
                    height: $widgetMain.height(),
                    silent: false
                });
            })
            //初始化自适应大小
            myChart.resize({
                width: $widgetMain.width(),
                height: $widgetMain.height(),
                silent: false
            });
        });
    }
    loadDashboadWidget();
    $ctrlWidget.find(".widget-toolbar a[data-action=widgetreload]").on(ace.click_event, function (e) {
        e.preventDefault();
        if (options.refresh && $.isFunction(options.refresh)) {
            options.refresh.call(this, options.fid);
        }
        loadDashboadWidget();
    })
    if (options.editable) {
        //设置
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetsettings]").on(ace.click_event, function (e) {
            e.preventDefault();
            if (options.settings && $.isFunction(options.settings)) {
                options.settings.call(this, options.fid);
            }
            var dialog = bootbox.dialog({
                title: "图表设置",
                message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
                buttons: {
                    success: {
                        label: $.lang("global_oper_enter", "确定"),
                        className: "btn-primary",
                        callback: function () {
                            var csmodel = getResult();
                            csmodel.fid = options.fid;
                            $.post(basePath + "/api/report/updateDashboard", csmodel, function (rv) {
                                if (rv) {
                                    $.msg("success");
                                    loadDashboadWidget(rv);
                                }
                            })

                        }
                    },
                    cancel: {
                        label: "关闭", className: "btn-default"
                    }
                }

            });
            dialog.init(function () {
                dialog.find('.bootbox-body').load(basePath + "Statistic/ReportDesigner/ChartOptions?fid=" + options.fid)
                //var $this = $(this);
                dialog.find('.bootbox-body').ace_scroll({
                    size: 450
                    //styleClass: 'scroll-left scroll-margin scroll-thin scroll-dark scroll-light no-track scroll-visible'
                });

            });
        })

        $ctrlWidget.find(".widget-toolbar a[data-action=widgetremove]").on(ace.click_event, function (e) {
            e.preventDefault();
            if (options.remove && $.isFunction(options.remove)) {
                options.remove.call(this, options.fid);
            }
            var $this = $(this);
            bootbox.confirm("确认要删除吗？", function (result) {
                if (result) {
                    $.get(basePath + "/api/report/deleteDashboard/" + options.fid, function (rv) {
                        if (rv) {
                            $this.closest(".widget-box").remove();
                        } else {
                            $.msg("删除失败！");
                        }
                    })
                }
            })
        })
    } else {
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetsettings]").remove();
        $ctrlWidget.find(".widget-toolbar a[data-action=widgetremove]").remove();
    }

    return loadDashboadWidget;
}
