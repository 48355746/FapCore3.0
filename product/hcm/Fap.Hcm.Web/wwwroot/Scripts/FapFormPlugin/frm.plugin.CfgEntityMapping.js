$("#Associate").on("focus", function () {
    var oritn = $("#OriEntity").val();
    var reftn = $("#AimsEntity").val();
    if (reftn === '')
        return;
    var dialog = bootbox.dialog({
        title: $.lang("property_associate_set",'设置属性关联'),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    var rv = [];
                    $("#filed-join option").each(function () {
                        rv.push(JSON.parse($(this).val()));
                    });
                    $("#frm-grid-CfgEntityMapping #Associate").val(JSON.stringify(rv));
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-CfgEntityMapping #Associate").val("");
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
        strVar += "            <select class=\"form-control\" id=\"filed-reftable\" multiple=\"multiple\" style=\"height:400px;width:130px;\"><\/select>";
        strVar += "           <select class=\"form-control\" id=\"filed-oritable\" multiple=\"multiple\" style=\"height:400px;width:130px;\"><\/select>       ";
        strVar += "<div class=\"form-control\"  style=\"height:60px;width:80px;border:0\">";
        strVar += "            <button type=\"button\" id=\"btnLock\" class=\"btn btn-info btn-sm btn-link\">";
        strVar += "                <i class=\"ace-icon fa fa-lock bigger-110\"><\/i>关联<\/button>";
        strVar += "            <button type=\"button\" id=\"btnUnLock\" class=\"btn btn-info btn-sm btn-link\">";
        strVar += "                <i class=\"ace-icon fa fa-unlock bigger-110\"><\/i>解除<\/button>";
        strVar += "	</div>";
        strVar += "<select class=\"form-control\" id=\"filed-join\" multiple=\"multiple\" style=\"height:400px;width:200px;\"><\/select>";
        strVar += "        <\/form>";

        dialog.find('.bootbox-body').html(strVar);

        var urlRef = basePath + '/Core/Api/FieldList/' + reftn;
        var urlOri = basePath + '/Core/Api/FieldList/' + oritn;
        $.get(urlRef, function (ev) {
            var selRef = $("#filed-reftable");
            var selOri = $("#filed-oritable");
            $.each(ev, function (i, d) {
                selRef.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
            });
            $.get(urlOri, function (ev) {
                $.each(ev, function (i, d) {
                    if (d.colProperty === "3") {
                        selOri.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
                    }
                });
                var mpv = $("#frm-grid-CfgEntityMapping #Associate").val();
                if (mpv === '')
                    return;
                var jmp = JSON.parse(mpv);
                $.each(jmp, function (i, d) {
                    var rv = d.AimsCol;
                    var ov = d.OriCol;
                    var rt = selRef.find("option[value=" + rv + "]").text();
                    var ot = selOri.find("option[value=" + ov + "]").text();
                    $("#filed-join").append("<option value='" + JSON.stringify(d) + "'>" + rt + "--" + ot + "</option>");
                });

            });
        });
        $("#btnLock").on(ace.click_event, function () {
            var rv = $('#filed-reftable').val();
            var ov = $('#filed-oritable').val();
            if (rv === null || ov === null)
                return;
            var jv = { "AimsCol": rv[0], "OriCol": ov[0] };
            var rt = $('#filed-reftable option:selected').text();
            var ot = $('#filed-oritable option:selected').text();
            $("#filed-join").append("<option value='" + JSON.stringify(jv) + "'>" + rt + "--" + ot + "</option>");
        });
        $("#btnUnLock").on(ace.click_event, function () {
            $("#filed-join option:selected").remove();
        });
    });
});