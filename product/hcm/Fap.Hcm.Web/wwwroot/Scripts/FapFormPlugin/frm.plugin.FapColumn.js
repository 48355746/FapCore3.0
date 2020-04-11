$("#frm-grid-columnmetadata #ColType").on("change", function () {
    var tv = $(this).val();
    if (tv === 'INT') {
        $("#frm-grid-columnmetadata #ColLength").val(4);
        $("#frm-grid-columnmetadata #ColDefault").val(0);
    } else if (tv === 'LONG') {
        $("#frm-grid-columnmetadata #ColLength").val(8);
        $("#frm-grid-columnmetadata #ColDefault").val(0);
    } else if (tv === 'DOUBLE') {
        $("#frm-grid-columnmetadata #ColLength").val(18);
        $("#frm-grid-columnmetadata #ColPrecision").val(2);
        $("#frm-grid-columnmetadata #ColDefault").val('0.0');
    } else if (tv === 'DATETIME') {
        $("#frm-grid-columnmetadata #ColLength").val(23);
    } else if (tv === 'BOOL') {
        $("#frm-grid-columnmetadata #ColLength").val(1);
        $("#frm-grid-columnmetadata #ColDefault").val(1);
    } else if (tv === 'UID') {
        $("#frm-grid-columnmetadata #ColLength").val(20);
        $("#frm-grid-columnmetadata #ColDefault").val('${FAP::UUID}');
    }
});
$("#frm-grid-columnmetadata #CtrlType").on("change", function () {
    $("#frm-grid-columnmetadata").find('label').removeClass("red");
    $("#RefType").rules("remove");
    $("#RefTableMC").rules("remove");
    $("#RefCodeMC").rules("remove");
    $("#RefNameMC").rules("remove");
    $("#MinValue").rules("remove");
    $("#MaxValue").rules("remove");
    $("#ComboxSource").rules("remove");
    $("#FileSuffix").val('');
    $("#FileCount").val('0');
    $("#FileSize").val('0');
    $("#RefID").val("");
    var tv = $(this).val();
    if (tv === 'TEXT' || tv === 'EMAIL' || tv === 'CITY' || tv === 'NATIVE') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('128');
    } else if (tv === 'PASSWORD' || tv === 'PHONE' || tv === 'STAR') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('20');
    }
    else if (tv === 'DOUBLE' || tv === 'MONEY') {
        $("#frm-grid-columnmetadata #ColType").val('DOUBLE');
        $("#frm-grid-columnmetadata #ColLength").val('20');
        $("#frm-grid-columnmetadata #ColPrecision").val('2');
        $("#frm-grid-columnmetadata #ColDefault").val('0.0');
    } else if (tv === 'INT') {
        $("#frm-grid-columnmetadata #ColType").val('INT');
        $("#frm-grid-columnmetadata #ColLength").val('4');
        $("#frm-grid-columnmetadata #ColDefault").val('0');
    } else if (tv === 'DATETIME') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('23');
    } else if (tv === 'DATE') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('10');
    } else if (tv === 'CHECKBOX') {
        $("#frm-grid-columnmetadata #ColType").val('BOOL');
        $("#frm-grid-columnmetadata #ColLength").val('4');
        $("#frm-grid-columnmetadata #ColDefault").val('1');
    } else if (tv === 'RADIO') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('32');
    } else if (tv === 'MEMO') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('512');
    } else if (tv === 'RICHTEXTBOX') {
        $("#frm-grid-columnmetadata #ColType").val('CLOB');
        $("#frm-grid-columnmetadata #ColLength").val('5000');
    } else if (tv === 'COMBOBOX') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('64');
        $("#ComboxSource").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#ComboxSource").rules("add", {
            required: true,
            messages: {
                required: "下拉数据源必须填写"
            }
        });
    } else if (tv === 'REFERENCE') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('64');
        $("#RefID").val("Fid");
        $("#RefType").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefTableMC").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefNameMC").closest('.ctrlcontainer').prev('label').addClass("red");

        $("#RefType").rules("add", {
            required: true,
            messages: {
                required: "参照类型必须填写"
            }
        });
        $("#RefTableMC").rules("add", {
            required: true,
            messages: {
                required: "参照表必须填写"
            }
        });
        $("#RefNameMC").rules("add", {
            required: true,
            messages: {
                required: "参照名称列必须填写"
            }
        });
    }
    else if (tv === 'CHECKBOXLIST') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('128');
        $("#RefType").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefTableMC").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefCodeMC").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefNameMC").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#RefType").rules("add", {
            required: true,
            messages: {
                required: "参照类型必须填写"
            }
        });
        $("#RefTableMC").rules("add", {
            required: true,
            messages: {
                required: "参照表必须填写"
            }
        });
        $("#RefCodeMC").rules("add", {
            required: true,
            messages: {
                required: "参照编码列必须填写"
            }
        });
        $("#RefNameMC").rules("add", {
            required: true,
            messages: {
                required: "参照名称列必须填写"
            }
        });
    } else if (tv === 'RANGE') {
        $("#frm-grid-columnmetadata #ColType").val('STRING');
        $("#frm-grid-columnmetadata #ColLength").val('32');
        $("#MinValue").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#MaxValue").closest('.ctrlcontainer').prev('label').addClass("red");
        $("#MinValue").rules("add", {
            required: true,
            messages: {
                required: "最小值必须填写"
            }
        });
        $("#MaxValue").rules("add", {
            required: true,
            messages: {
                required: "最大值必须填写"
            }
        });
    } else if (tv === "FILE") {
        $("#frm-grid-columnmetadata #ColType").val('UID');
        $("#frm-grid-columnmetadata #ColLength").val('20');
        $("#frm-grid-columnmetadata #ColDefault").val('${FAP::UUID}');
        $("#FileSuffix").val('txt,doc,jpg,gif,png');
        $("#FileCount").val(10);
        $("#FileSize").val(10240);
    } else if (tv === 'IMAGE') {
        $("#frm-grid-columnmetadata #ColType").val('UID');
        $("#frm-grid-columnmetadata #ColLength").val('20');
        $("#frm-grid-columnmetadata #ColDefault").val('${FAP::UUID}');
        $("#FileSuffix").val('jpg,gif,png');
        $("#FileCount").val(10);
        $("#FileSize").val(10240);
    }
});

$("#RefCols").on("focus", function () {
    var tn = $("#RefTable").val();
    if (tn !== '') {
        var dialog = bootbox.dialog({
            title: '选择其他显示列',
            message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
            buttons: {
                success: {
                    label: $.lang("ok", "确定"),
                    className: "btn-primary",
                    callback: function () {
                        var rowDatas = getSelectedRows("grid-datagrid_FapColumn");
                        if (rowDatas) {
                            var cols = $.map(rowDatas, function (d) {
                                return d.ColName;
                            });
                            $("#frm-grid-columnmetadata #RefCols").val(cols.join());

                        } else { $.msg("请选择一条数据！"); return; }
                    }
                },
                danger: {
                    label: "清空!",
                    className: "btn-sm btn-danger",
                    callback: function () {
                        $("#frm-grid-columnmetadata #RefCols").val("");
                    }
                },
                cancel: {
                    label: $.lang("cancel", "取消"), className: "btn-default"
                }
            }

        });
        dialog.on("shown.bs.modal", function () {
            $(window).triggerHandler('resize.jqGrid');//触发窗口调整,使Grid得到正确的大小
        });
        dialog.init(function () {
            var url = basePath + '/Component/DataGrid';
            $.get(url, { "TableName": "FapColumn", "Cols": "Id,Fid,ColName,ColComment", "Condition": "TableName='" + tn + "' and IsDefaultCol=0" }, function (ev) {
                dialog.find('.bootbox-body').html(ev);
            });
        });
    }
});

$("#RefReturnMapping").on("focus", function () {
    var reftn = $("#RefTable").val();
    var oritn = $("#TableName").val();
    if (reftn === '')
        return;
    var dialog = bootbox.dialog({
        title: '设置参照关联',
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
                    $("#frm-grid-columnmetadata #RefReturnMapping").val(JSON.stringify(rv));
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-columnmetadata #RefReturnMapping").val("");
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
                    selOri.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
                });
                var mpv = $("#frm-grid-columnmetadata #RefReturnMapping").val();
                if (mpv === '')
                    return;
                var jmp = JSON.parse(mpv);
                $.each(jmp, function (i, d) {
                    var rv = d.RefCol;
                    var ov = d.FrmCol;
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
            var jv = { "RefCol": rv[0], "FrmCol": ov[0] };
            var rt = $('#filed-reftable option:selected').text();
            var ot = $('#filed-oritable option:selected').text();
            $("#filed-join").append("<option value='" + JSON.stringify(jv) + "'>" + rt + "--" + ot + "</option>");
        });
        $("#btnUnLock").on(ace.click_event, function () {
            $("#filed-join option:selected").remove();
        });
    });
});
var strConstant = "";
strConstant += "<div class=\"control-group form-inline\">";
strConstant += "												<label class=\"control-label bolder blue\">常量选择：<\/label>";
strConstant += "";
strConstant += "												<div class=\"radio\">";
strConstant += "													<label>";
strConstant += "														<input name=\"form-field-radio\" type=\"radio\" value=\"${FAP::CURRENTDATE}\" class=\"ace\" \/>";
strConstant += "														<span class=\"lbl\">当前日期<\/span>";
strConstant += "													<\/label>";
strConstant += "												<\/div>";
strConstant += "";
strConstant += "												<div class=\"radio\">";
strConstant += "													<label>";
strConstant += "														<input name=\"form-field-radio\" type=\"radio\" value=\"${FAP::CURRENTEMPLOYEE}\" class=\"ace\" \/>";
strConstant += "														<span class=\"lbl\">登录员工<\/span>";
strConstant += "													<\/label>";
strConstant += "												<\/div>";
strConstant += "";
strConstant += "												<div class=\"radio\">";
strConstant += "													<label>";
strConstant += "														<input name=\"form-field-radio\" type=\"radio\" value=\"${FAP::CURRENTDEPT}\" class=\"ace\" \/>";
strConstant += "														<span class=\"lbl\">登录员工部门<\/span>";
strConstant += "													<\/label>";
strConstant += "												<\/div>";
strConstant += "";
strConstant += "												<div class=\"radio\">";
strConstant += "													<label>";
strConstant += "														<input name=\"form-field-radio\" type=\"radio\" value=\"${FAP::CURRENTDEPTCODE}\" class=\"ace\" \/>";
strConstant += "														<span class=\"lbl\">登录员工部门编码<\/span>";
strConstant += "													<\/label>";
strConstant += "												<\/div>";
strConstant += "";
strConstant += "												<div class=\"radio\">";
strConstant += "													<label>";
strConstant += "														<input  name=\"form-field-radio\" value=\"${FAP::CURRENTUSER}\" type=\"radio\" class=\"ace\" \/>";
strConstant += "														<span class=\"lbl\"> 登录用户<\/span>";
strConstant += "													<\/label>";
strConstant += "												<\/div>";
strConstant += "											<\/div>";

var strDefault = "";
strDefault += "<div>";
strDefault += "															<label for=\"form-field-9\"  class=\"control-label bolder blue\">值：<\/label>";
strDefault += "";
strDefault += "															<textarea class=\"form-control limited\" id=\"form-field-default\" rows=\"5\" maxlength=\"1000\"><\/textarea>";
strDefault += "														<\/div>";

$("#frm-grid-columnmetadata #ColDefault").on("focus", function () {
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
        dialog.find('.bootbox-body').html(strConstant + strDefault);
        $("#form-field-default").val($("#frm-grid-columnmetadata #ColDefault").val());
        $("input[name=form-field-radio]").on(ace.click_event, function () {
            $("#form-field-default").val($(this).val());
        });
    });
});

$("#frm-grid-columnmetadata #RefCondition").on("focus", function () {
    var reftn = $("#RefTable").val();
    if (reftn === '')
        return;
    var dialog = bootbox.dialog({
        title: '设置默认值',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    $("#frm-grid-columnmetadata #RefCondition").val($("#form-field-default").val());
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#frm-grid-columnmetadata #RefCondition").val("");
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {
        dialog.find('.bootbox-body').html(strConstant + strDefault);
        $("#form-field-default").val($("#frm-grid-columnmetadata #RefCondition").val());
        $("input[name=form-field-radio]").on(ace.click_event, function () {
            $("#form-field-default").val($("#form-field-default").val() + $(this).val());
        });
    });
});