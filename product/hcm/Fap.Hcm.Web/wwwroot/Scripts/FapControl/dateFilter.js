; $(function () {
    //日期过滤控件js
    $(document).off(ace.click_event, "[data-rel=datefilter]").on(ace.click_event, "[data-rel=datefilter]", function () {
        var $this = $(this);
        var dateResult = {};
        var $action = $this.data("action");
        var current = moment().format('YYYY-MM-DD');
        if ($action == "currdate") {
            dateResult.start = current + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i>当天<span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "currmonth") {
            var currmonth = moment().format("YYYY-MM") + "-01";
            dateResult.start = currmonth + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i>当月<span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "week") {
            var week = moment().subtract(7, 'days').format('YYYY-MM-DD');
            dateResult.start = week + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i>七天<span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "month") {
            var month = moment().subtract(1, 'months').format('YYYY-MM-DD');
            dateResult.start = month + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i> 一个月<span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "threemonth") {
            var threemonth = moment().subtract(3, 'months').format('YYYY-MM-DD');
            dateResult.start = threemonth + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i>三个月 <span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "sixmonth") {
            var sixmonth = moment().subtract(6, 'months').format('YYYY-MM-DD');
            dateResult.start = sixmonth + " 00:00:00";
            dateResult.end = current + " 23:59:59";
            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                datefilterComplete(dateResult);
            }
            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i>六个月<span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
        } else if ($action == "custom") {
            var dialog = bootbox.dialog({
                title: '时间范围',
                message: ['<div class="input-daterange input-group">',
                    '<input type="text" class="input-sm form-control" id="startDatepicker" name="start" />',
                    '<span class="input-group-addon">',
                    '<i class="fa fa-exchange"></i>',
                    '</span>',
                    '<input type="text" class="input-sm form-control" id="endDatepicker" name="end" />',
                    '</div>'].join(""),
                buttons: {
                    success: {
                        label: MultiLangHelper.getResName("global_oper_enter", "确定"),
                        className: "btn-primary",
                        callback: function () {
                            var tempStart = $('.bootbox-body #startDatepicker').val();
                            var tempEnd = $('.bootbox-body #endDatepicker').val();
                            if (tempStart == '') {
                                tempStart = current + " 00:00:00";
                            }
                            if (tempEnd == '') {
                                tempEnd = current + " 23:59:59";
                            }
                            dateResult.start = tempStart;
                            dateResult.end = tempEnd;
                            if (datefilterComplete && $.isFunction(datefilterComplete)) {
                                datefilterComplete(dateResult);
                            }
                            $this.closest("ul").prev().html("<i class=\"ace-icon fa fa-calendar bigger-120 \"></i> 自定义：" + tempStart + " 至 " + tempEnd + " <span class=\"ace-icon fa fa-caret-down icon-on-right\"></span>");
                        }
                    },
                    cancel: {
                        label: MultiLangHelper.getResName("global_oper_cancel", "取消"), className: "btn-default"
                    }
                }

            });

            dialog.init(function () {
                $('.bootbox-body #startDatepicker,#endDatepicker').scroller('destroy').scroller($.extend({ preset: 'datetime', stepMinute: 1 }, { theme: 'android-ics light', mode: 'scroller', display: 'modal', lang: 'zh' }));
                $(".bootbox-body #startDatepicker").on("change", function (e) {
                    $('.bootbox-body #endDatepicker').scroller('destroy').scroller($.extend({ preset: 'datetime', minDate: new Date($(this).val()), stepMinute: 1 }, { theme: 'android-ics light', mode: 'scroller', display: 'modal', lang: 'zh' }));
                });
                $(".bootbox-body #endDatepicker").on("change", function (e) {
                    $('.bootbox-body #startDatepicker').scroller('destroy').scroller($.extend({ preset: 'datetime', maxDate: new Date($(this).val()), stepMinute: 1 }, { theme: 'android-ics light', mode: 'scroller', display: 'modal', lang: 'zh' }));
                });
            });
        }
    })
});