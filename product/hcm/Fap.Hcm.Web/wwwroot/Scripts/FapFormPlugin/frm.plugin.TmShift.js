//排班单据js注入

$("#EndTime").attr("readonly", "readonly");
$("#RestEndTime").attr("readonly", "readonly");
function getEndTime(stdate) {

	//var times = Number($("#WorkHoursLength").val()) + Number($("#RestHoursLength").val());
    $("#EndTime").val(moment("2000-1-1 " + stdate).add(Number($("#WorkHoursLength").val()), "hour").add(Number($("#RestMinutesLength").val()), "minute").format("HH:mm"));
}
function getRestEndTime(stdate) {

    $("#RestEndTime").val(moment("2000-1-1 " + stdate).add(Number($("#RestMinutesLength").val()), "minute").format("HH:mm"));
};
$("#StartTime").on('change', function (ev) {
    if ($("#WorkHoursLength").val() == "" || $("#RestMinutesLength").val() == "") {
	}
	else {
		getEndTime($(this).val());
	}
});
$("#WorkHoursLength,#RestMinutesLength").blur(function () {
    if ($("#WorkHoursLength").val() == "" || $("#RestMinutesLength").val() == "") {
	}
	else {
		getEndTime($("#StartTime").val());
	}

});
$("#RestStartTime,#RestMinutesLength").blur(function () {
    if ($("#RestStartTime").val() == "" || $("#RestMinutesLength").val() == "") {
    }
    else {
        getRestEndTime($("#RestStartTime").val());
    }

});