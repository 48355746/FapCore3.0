//销假
//---请假单据js注入---

$('#frm-TmRevokLeaveApply').on("mouseover", "#IntervalDay", function () {
    $(this).prop('disabled', "disabled");
});

$('#frm-TmRevokLeaveApply').on("mouseout", "#IntervalDay", function () {
    $(this).removeAttr('disabled');
});
$('#frm-TmRevokLeaveApply').on("mouseover", "#IntervalHour", function () {
    $(this).prop('disabled', "disabled");
});

$('#frm-TmRevokLeaveApply').on("mouseout", "#IntervalHour", function () {
    $(this).removeAttr('disabled');
});
var leavelDays = function () {
    var startTime = $("#StartTime").val();
    var endTime = $("#EndTime").val();
    var empUid = $("#AppEmpUid").val();
    if (startTime === "" || endTime === "" || empUid === "") {
        return;
    }
    var leaveType = $('#RevokType').val();
    $.post(basePath + "/Time/Api/LeavelDays", { empUid: empUid, startDateTime: startTime, endDateTime: endTime }, function (rv) {
        if (rv.success) {
            var days = rv.data.days;
            var hours = rv.data.hours;
            $("#IntervalDay").val(rv.data.days);
            $("#IntervalHour").val(rv.data.hours);
            if (leaveType === "Annaul") {
                $.get(basePath + "/Time/Api/Annual/ValidDays", { empUid: empUid ,startTime: startTime }, function (rva) {
                    if (rva.success) {
                        var oriDays = $("#OriIntervalDay").val();                        
                        var annual = parseFloat(rva.data) + parseFloat(oriDays);
                        if (parseFloat(days) > annual) {
                            $("#IntervalDay").rules("add", {
                                range: [0, rva.data],
                                messages: {
                                    range: "你最多还能休" + rva.data + "天年假！"
                                }
                            });
                        }
                    } else {
                        bootbox.alert(rva.msg);
                    }
                });
            } else if (leaveType === "Tuneoff") {//调休
                $.get(basePath + "/Time/Api/Overtime/ValidDays", { empUid: empUid }, function (rvb) {
                    if (rvb.success) {
                        var oriHours = $("#OriIntervalHour").val();//获取的调休时长不包括审批通过的调休时长，这里要加上
                        var h = parseFloat(rvb.data) + parseFloat(oriHours);
                        if (hours > h) {
                            $("#IntervalHour").rules("add", {
                                range: [0, rvb.data],
                                messages: {
                                    range: "你最多只能调休" + rvb.data + "小时！"
                                }
                            });
                        }
                    } else {
                        bootbox.alert(rvb.msg);
                    }
                });
            }
        } else {
            bootbox.alert(rv.msg);
        }
    });

};
$("#StartTime").on('change', function () {
    leavelDays();
});

$("#EndTime").on('change', function () {
    leavelDays();
});