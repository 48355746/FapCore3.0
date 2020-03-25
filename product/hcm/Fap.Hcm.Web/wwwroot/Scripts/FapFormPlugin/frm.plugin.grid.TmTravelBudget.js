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
    $("#FeeBudget").val(total.toFixed(2));
});
//编辑状态激发
$("#grid-TmTravelBudget").on("jqGridInlineEditRow", function (obj, rowid, o) {
    o.oneditfunc = function (rowid) {
        $("#grid-TmTravelBudget #" + rowid + "_FixRMBAmount").val(100);

    };
});
$("#grid-TmTravelBudget").off("change", "input[name='Amount']").on("change", "input[name='Amount']", function () {
    var amount = $(this).val();
    var rowid = $(this).attr("rowid");
    var rate = $("#grid-TmTravelBudget #" + rowid + "_ExchangeRate").val();
    SumRMB(rowid, amount, rate);
});
$("#grid-TmTravelBudget").off("change", "input[name='ExchangeRate']").on("change", "input[name='ExchangeRate']", function () {
    var rate = $(this).val();
    var rowid = $(this).attr("rowid");
    var amount = $("#grid-TmTravelBudget #" + rowid + "_Amount").val();
    SumRMB(rowid, amount, rate);
});
//计算超出金额
$("#grid-TmTravelBudget").off("change", "input[name='RMBAmount']").on("change", "input[name='RMBAmount']", function () {
    var rowid = $(this).attr("rowid");    
    var amount = $(this).val();
    var fixamount = $("#grid-TmTravelBudget #" + rowid + "_FixRMBAmount").val();
    if (parseFloat(amount) > parseFloat(fixamount)) {
        $("#grid-TmTravelBudget #" + rowid + "_ExceedRMBAmount").val((amount - fixamount).toFixed(2));
    } else {
        $("#grid-TmTravelBudget #" + rowid + "_ExceedRMBAmount").val(0);
    }
});
//计算合计
function SumRMB(rowid, amount, rate) {
    var sum = (amount * rate).toFixed(2);
    $("#grid-TmTravelBudget #" + rowid + "_RMBAmount").val(sum);
    $("#grid-TmTravelBudget #" + rowid + "_RMBAmount").trigger("change");
}