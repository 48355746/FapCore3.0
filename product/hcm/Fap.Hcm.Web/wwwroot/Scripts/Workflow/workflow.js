/**
  * 流程图
  * */
function workflowChart(processUid, billUid) {
    var openUrl = $.randomUrl(basePath + "/Workflow/Business/FlowChart?processUid=" + processUid + "&billUid=" + billUid);
    bootboxWindow($.lang('view', '查看'), openUrl);
    
}
//查看申请单据
function viewBill(processUid, businessUid, billUid) {    
    var openUrl = $.randomUrl(basePath + '/Workflow/Business/ApplyViewBill?processUid=' + processUid + '&businessUid=' + businessUid + '&billUid=' + billUid);
    bootboxWindow($.lang('view', '查看'), openUrl, {
        print: {
            label: $.lang('print', '打印'),
            className: "btn-primary btn-link",
            callback: function () {
                printBill();
                return false;
            }
        }
    });
}
//查看已批单据
function viewApprovalBill(businessUid, billUid) {
    var openUrl = basePath + "/Workflow/Business/ApprovalViewBill?billUid=" + billUid + "&businessUid=" + businessUid ;
    bootboxWindow($.lang('view', '查看'), openUrl);
}