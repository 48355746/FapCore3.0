function applyBusiness(title, processUid, frmType, businessUid, initData) {
    var openUrl = $.randomUrl(basePath + '/Workflow/Business/ApplyBill?processUid=' + processUid + '&businessUid=' + businessUid);

    var dialog = bootbox.dialog({
        title: '"' + title + '"申请',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer: false,
        buttons: {
            tempSave: {
                label: '暂存',
                className: "btn-info",
                callback: function () {
                    temporarySave();
                    return false;
                }
            }, submit: {
                label: '提交',
                className: "btn-primary",
                callback: function () {
                    submitBill(function () {
                        $.msg('提交成功,业务中心查看我的申请');
                    });
                    return false;
                }
            }
        }

    });
    dialog.init(function () {
        $.get(openUrl, function (ev) {
            dialog.find('.bootbox-body').html(ev);
            if (initData) {
                for (var key in initData) {
                    dialog.find("[name$=" + key + "]").val(initData[key]);
                }
            }
        });
    });

}