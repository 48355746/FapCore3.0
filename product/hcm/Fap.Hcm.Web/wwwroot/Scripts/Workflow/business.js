
function applyBusiness(businessUid,title, initData) {  

    var openUrl = $.randomUrl(basePath+'/Workflow/Business/ApplyBill?businessUid=' + businessUid);

    var dialog = bootbox.dialog({
        title: title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer: false,
        buttons: {
            tempSave: {
                label: $.lang("temporary", '暂存'),
                className: "btn-link btn-info",
                callback: function () {
                    $.msg("请到[业务中心]查看详情");
                    var r = temporarySave();
                    if (r === false) {
                        return r;
                    }
                }
            }, submit: {
                label: $.lang("submit", "提交"),
                className: "btn-link btn-primary",
                callback: function () {
                    submitBill(function () {
                        bootbox.hideAll();
                        $.msg("请到[业务中心]查看详情");
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
                    dialog.find("[name$=" + key + "]").trigger("change");
                }
            }
        });
    });
}