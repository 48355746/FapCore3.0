
//出差单据js注入

$("#TravelDays").attr("readonly", "readonly");

var leavelTravelDays = function () {
    var startTime = $("#StartTime").val();
    var endTime = $("#EndTime").val();
    var empUid = $("#AppEmpUid").val();
    if (startTime === "" || endTime === "" || empUid === "") {
        return;
    }
    $.post(basePath + "/Time/Api/LeavelDays", { empUid: empUid, startDateTime: startTime, endDateTime: endTime }, function (rv) {
        if (rv.success) {
            $("#TravelDays").val(rv.data.days);
            $("#HoursLength").val(rv.data.hours);           
        } else {
            bootbox.alert(rv.msg);
        }
    });

};
$("#StartTime,#EndTime").change(function () {
    leavelTravelDays();
});
