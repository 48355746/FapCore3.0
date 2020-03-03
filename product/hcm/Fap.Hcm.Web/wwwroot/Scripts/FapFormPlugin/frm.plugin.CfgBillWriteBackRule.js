$("#FieldMapping").on("focus", function () {
    var billTable = $("#DocEntityUid").val();
    var bizTable = $("#BizEntityUid").val();    
    if (billTable === '' || bizTable === '') {
       $.msg($.lang("bill_biz_required","单据实体和业务实体必选"));
        return;
    }
    var dialog = bootbox.dialog({
        title: '设置关联',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    var rv = [];
                    $("#filed-join option").each(function () {

                        rv.push({ "id": $(this).val(), "text": $(this).text() });
                    });
                    $("#frm-grid-wfbusinessmapping #FieldMapping").val(JSON.stringify(rv));
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-wfbusinessmapping #FieldMapping").val("");
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {
        var strVar = "";
        strVar += "<form class=\"form-inline\">";
        strVar += "            <select class=\"form-control\" id=\"filed-billtable\" multiple=\"multiple\" style=\"height:400px;width:130px;\"><\/select>";
        strVar += "           <select class=\"form-control\" id=\"filed-biztable\" multiple=\"multiple\" style=\"height:400px;width:130px;\"><\/select>       ";
        strVar += "<div class=\"form-control\"  style=\"height:60px;width:80px;border:0\">";
        strVar += "            <button type=\"button\" id=\"btnLock\" class=\"btn btn-info btn-sm btn-link\">";
        strVar += "                <i class=\"ace-icon fa fa-lock bigger-110\"><\/i>关联<\/button>";
        strVar += "            <button type=\"button\" id=\"btnUnLock\" class=\"btn btn-info btn-sm btn-link\">";
        strVar += "                <i class=\"ace-icon fa fa-unlock bigger-110\"><\/i>解除<\/button>";
        strVar += "	</div>";
        strVar += "<select class=\"form-control\" id=\"filed-join\" multiple=\"multiple\" style=\"height:400px;width:200px;\"><\/select>";
        strVar += "        <\/form>";

        dialog.find('.bootbox-body').html(strVar);

        var urlBill = basePath + '/Api/Core/FieldList/' + billTable;
        var urlBiz = basePath + '/Api/Core/FieldList/' + bizTable;
        $.get(urlBill, function (ev) {
            var selBill = $("#filed-billtable");
            var selBiz = $("#filed-biztable");
            $.each(ev, function (i, d) {
                selBill.append("<option value='" + d.tableName + "." + d.colName + "'>" + d.colComment + "</option>");
            });
            $.get(urlBiz, function (ev) {
                $.each(ev, function (i, d) {
                    selBiz.append("<option value='" + d.tableName + "." + d.colName + "'>" + d.colComment + "</option>");
                });
                var mpv = $("#frm-grid-wfbusinessmapping #FieldMapping").val();
                if (mpv === '')
                    return;
                var jmp = JSON.parse(mpv);
                $.each(jmp, function (i, d) {
                    var rv = d.id;
                    var ov = d.text;
                    $("#filed-join").append("<option value='" + rv + "'>" + ov + "</option>");
                });

            });
        });
        $("#btnLock").on(ace.click_event, function () {
            var rv = $('#filed-billtable').val();
            var ov = $('#filed-biztable').val();
            if (rv === null || ov === null)
                return;
            var jv = rv[0] + "," + ov[0];
            var rt = $('#filed-billtable option:selected').text();
            var ot = $('#filed-biztable option:selected').text();
            //var billText = $("#DocEntityUidMC").val();
            //var bizText = $("#BizEntityUidMC").val();
            $("#filed-join").append("<option value='" + jv + "'>" + rt + "-->" + ot + "</option>");
        });
        $("#btnUnLock").on(ace.click_event, function () {
            $("#filed-join option:selected").remove();
        });
    });
});
$("#CustomSql").on("focus", function () {
    var billTable = $("#DocEntityUid").val();
    var bizTable = $("#BizEntityUid").val();
    if (billTable === '' || bizTable === '') {
        $.msg($.lang("bill_biz_required", "单据实体和业务实体必选"));
        return;
    }
    var sqlContent = $(this).val();
    var dialog = bootbox.dialog({
        title: '设置SQL',
        size: "large",
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    $("#frm-grid-wfbusinessmapping #CustomSql").val($("#txtCustomSql").val());
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-wfbusinessmapping #CustomSql").val("");
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }
    });
    dialog.init(function () {
        var urlBack = basePath + '/System/Manage/BillWriteBackSet';
        $.get(urlBack, function (ev) {
            dialog.find('.bootbox-body').html(ev);
            initBillWriteback(billTable, bizTable, sqlContent);

        });
    });
    dialog.on('shown.bs.modal', function (e) {
        initSql();
    });
});