//---请假单据js注入---
//需要校验这两个字段
//$("#IntervalHour").attr("readonly", "readonly");
//$("#IntervalDay").attr("readonly", "readonly");
$('#LeaveType').change(function () {
        $.ajax({
            url: basePath + '/api/time/getleavetypedays',
            type: 'get',
            async: false,//使用同步的方式,true为异步方式
            data: { leaveType: $(this).val() },//这里使用json对象
            success: function (data) {
                $("#msgannaulcount").remove();
                var wkt="工作日";
                if(data.isWorkDay===0)
                {
                    wkt="自然日";
                }

                if (data.typeCode==="Tuneoff") {
                    $("#TmLeaveApplyLeaveTypeMC").closest("div").append("<div class='error'>可休" + data.hoursLength + "小时</div>");
                    $("#IntervalHour").rules("add", {
                        range: [0, data.hoursLength],
                        messages: {
                            range: "时长必须在0到" + data.hoursLength + "之间！"
                        }
                    });
                }
                else {
                    $("#TmLeaveApplyLeaveTypeMC").closest("div").append("<div class='error'>可休:"+ data.allowDays + "天（" + wkt +"）</div>");
                    $("#IntervalDay").rules("add", {
                        range: [0, data.allowDays],
                        messages: {
                            range: "天数必须在0到" + data.allowDays + "之间！"
                        }
                    });
                }
                
            },
            fail: function () {
                //code here...
            }
        });
});
var GetScheduling = function (StartTime, EndTime) {
    if (StartTime === "") {
        return;
    }
    if (EndTime === "") {
        return;
    }
    var LeaveType = $('#LeaveType').val();
    if (LeaveType === "")
    {
        $("#StartTime").val("");
        $("#EndTime").val("");
    }
    $.ajax({
        url: basePath + '/api/time/getworkdaybydate',
        type: "post",
        dataType: "json",
        data: { StartTime: StartTime, EndTime: EndTime, LeaveType: $('#LeaveType').val() },
        async: false,
        success: function (data) {
            $("#msgannauljq").remove();
            if (data.msg!= "") {
                //$("#frm-TmLeaveApply").before("<div id='msgannauljq' class='alert alert-block alert-success'><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>请假信息：</strong>" + data.msg + "</div>");
                $("#IntervalDay").val(0);
                $("#IntervalHour").val(0);
                
            }
            else {
                //$("#frm-TmLeaveApply").before("<div id='msgannauljq' class='alert  alert-block alert-success'><button type=button class=close data-dismiss=alert><i class=ace-icon fa fa-times></i></button><i class=ace-icon fa fa-check green></i><strong class=green>请假信息：</strong>您的请假天数为：" + data.days + ".</div>");
                $("#IntervalDay").val(data.days.toFixed(1));
                $("#IntervalHour").val(data.workhours.toFixed(1));
            }
        },
        error: function (e) {
            alert("异常！");
        }
    });
};
$("#StartTime").on('change', function () {
    GetScheduling($("#StartTime").val(), $("#EndTime").val());
});

$("#EndTime").on('change', function () {
    GetScheduling($("#StartTime").val(), $("#EndTime").val());
});