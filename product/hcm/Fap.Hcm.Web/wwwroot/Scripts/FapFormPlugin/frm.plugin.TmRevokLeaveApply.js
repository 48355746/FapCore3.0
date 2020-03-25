//销假
//---请假单据js注入---

$('#frm-TmRevokLeaveApply').on("mouseover", "#IntervalDay", function () {
    $(this).prop('disabled', "disabled");
});

$('#frm-TmRevokLeaveApply').on("mouseout", "#IntervalDay", function () {
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