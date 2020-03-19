$("#EmpConditionDesc").on(ace.click_event, function () {
    var dialog = bootbox.dialog({
        title: '设置默认值',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    $("#frm-grid-columnmetadata #ColDefault").val($("#form-field-default").val());
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-columnmetadata #ColDefault").val("");
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {
        var url = basePath + '/Component/EntityCondition';
        $.get(url, { "entity": "Employee"}, function (ev) {
            dialog.find('.bootbox-body').html(ev);
        });
    });
});