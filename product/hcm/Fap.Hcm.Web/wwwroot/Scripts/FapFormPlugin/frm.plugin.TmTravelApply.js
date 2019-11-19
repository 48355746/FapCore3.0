
//出差单据js注入

$("#TravelDays").attr("readonly", "readonly");
var GetScheduling = function (StartTime, EndTime) {
    if (StartTime == "") {
        return;
    }
    if (EndTime == "") {
        return;
    }


    $.ajax({
        url: basePath + '/api/time/getworkdaybydate',
        type: "post",
        dataType: "json",
        data: { StartTime: StartTime, EndTime: EndTime },
        async: false,
        success: function (data) {
            $("#msgannauljq").remove();
            if (data.msg != "") {
                
                $("#frm-TmTravelApply").before("<div id='msgannauljq' class=alert style='color:red' alert-block alert-success><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>出差信息：</strong>" + data.msg + "</div>");
                $("#TravelDays").val(0);
                $("#HoursLength").val(0);
            }
            else {
                $("#frm-TmTravelApply").before("<div id='msgannauljq' class=alert style='color:red' alert-block alert-success><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>出差信息：</strong>您的出差天数为：" + data.days + ".</div>");
                $("#TravelDays").val(data.days);
                $("#HoursLength").val(data.workhours);
            }
        },
        error: function (e) {
            alert("异常！");
        }
    });
};
$("#StartTime,#EndTime").change(function () {
    GetScheduling($("#StartTime").val(), $("#EndTime").val());
});
