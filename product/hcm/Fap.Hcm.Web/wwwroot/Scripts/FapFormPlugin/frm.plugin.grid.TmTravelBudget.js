//grid 保存后事件，此js会注入到自由表单中
$("#grid-TmTravelBudget").on("jqGridInlineAfterSaveRow", function (obj,rowid, resp, tmp, o) {
    var rowDatas = $(this).jqGrid("getRowData", null, true);
    var total = 0;
    $.each(rowDatas, function (i, data) {
        if (isNaN(data.RMBAmount) || data.RMBAmount === "") {
            data.RMBAmount = 0;
        }
        total += parseFloat(data.RMBAmount);
    });
    $("#lblTotal").text(total.toFixed(2));
});