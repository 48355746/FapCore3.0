//---请假单据js注入---

$('#frm-TmLeaveApply').on("mouseover", "#IntervalDay", function () {
    $(this).prop('disabled', "disabled");
});

$('#frm-TmLeaveApply').on("mouseout", "#IntervalDay", function () {
    $(this).removeAttr('disabled');
});
$('#frm-TmLeaveApply').on("mouseover", "#IntervalHour", function () {
    $(this).prop('disabled', "disabled");
});

$('#frm-TmLeaveApply').on("mouseout", "#IntervalHour", function () {
    $(this).removeAttr('disabled');
});
var leavelDays = function () {
    var startTime = $("#StartTime").val();
    var endTime = $("#EndTime").val();
    var empUid = $("#AppEmpUid").val();
    if (startTime === "" || endTime === "" || empUid==="") {
        return;
    }    
    var leaveType = $('#LeaveType').val();
    $.post(basePath + "/Time/Api/LeavelDays", { empUid: empUid, startDateTime: startTime, endDateTime: endTime }, function (rv) {
        if (rv.success) {
            var days = rv.data.days;
            var hours = rv.data.hours;
            $("#IntervalDay").val(rv.data.days);
            $("#IntervalHour").val(rv.data.hours);
            if (leaveType === "Annaul") {//年假
                $.get(basePath + "/Time/Api/Annual/ValidDays", { empUid: empUid, startTime: startTime }, function (rva) {
                    if (rva.success) {
                        if (days > rva.data) {
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
                        if (hours > rvb.data) {
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