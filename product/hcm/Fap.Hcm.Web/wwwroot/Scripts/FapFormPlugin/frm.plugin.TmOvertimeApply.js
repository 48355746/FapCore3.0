$("#HoursLength").attr("readonly", "readonly");
$("#StartTime,#EndTime,#RestHours").on("change", function () {
    if ($("#StartTime").val() == "" || $("#EndTime").val() == "")
    {
        return false;
    }
    //休息时长
    var rh = 0;
    if ($("#RestHours").val() != "")
    {
        rh = $("#RestHours").val();
    }
    $.ajax({
        url: basePath + '/api/time/getworkovertime',
        type: "post",
        datatype: "json",
        data: { StartTime: $("#StartTime").val(), EndTime: $("#EndTime").val(), RestHours: rh },
        async: false,
        success: function (data) {
            $("#msgannauljq").remove();
            if (data.msg != "") {
                $("#frm-TmOvertimeApply").before("<div id='msgannauljq' class=alert style='color:red' alert-block alert-success><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>加班信息：</strong>" + data.msg + "</div>");
                $("#HoursLength").val(0);
                //$("#intervalhour").val(0);

            }
            else {
                if (data.workhours >= data.minworkhours) {
                    $("#frm-TmOvertimeApply").before("<div id='msgannauljq' class=alert style='color:red' alert-block alert-success><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>加班信息：</strong>您的加班时长为：" + data.workhours + "小时.</div>");
                }
                else {
                    $("#frm-TmOvertimeApply").before("<div id='msgannauljq' class=alert style='color:red' alert-block alert-success><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>加班信息：</strong>加班时长不得小于：" + data.minworkhours + "小时.</div>");
                }
                $("#HoursLength").val(data.workhours.toFixed(1));
                //$("#intervalhour").val(data.workhours);
            }
        },
        error: function (e) {
            alert("异常！");
        }
    });

});