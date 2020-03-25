
$("#StartTime,#EndTime,#RestHours").on("change", function () {
    var startTime = $("#StartTime").val();
    var endTime = $("#EndTime").val();
    if (startTime === "" || endTime === "")
    {
        return false;
    }
    //休息时长
    var rh = $("#RestHours").val();
    if (rh === "")
    {
        rh = 0;
    }
    let t1 = moment(startTime);

    let t2 = moment(endTime);

    var minute = t2.diff(t1, 'minute');
    var hours = (minute / 60.0).toFixed(2);
    $("#HoursLength").val(hours);
    

});