$("#frm-grid-columnmetadata #ColType").on("change", function () {
    var id = $("#frm-grid-columnmetadata #Id").val();
    if (id !== '') {
        return;
    }
    var tv = $(this).val();
    if (tv === 'STRING') {
        $("#frm-grid-columnmetadata #ColLength").val(40);
    } else if (tv === 'INT') {
        $("#frm-grid-columnmetadata #ColLength").val(4);
        $("#frm-grid-columnmetadata #ColDefault").val(0);
    } else if (tv === 'LONG') {
        $("#frm-grid-columnmetadata #ColLength").val(8);
        $("#frm-grid-columnmetadata #ColDefault").val(0);
    } else if (tv === 'DOUBLE') {
        $("#frm-grid-columnmetadata #ColLength").val(18);
        $("#frm-grid-columnmetadata #ColPrecision").val(2);
        $("#frm-grid-columnmetadata #ColDefault").val(0.0);
    } else if (tv === 'DATETIME') {
        $("#frm-grid-columnmetadata #ColLength").val(20);
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
    //$("#ColType").val('');
    $("#FileSuffix").val('');
    $("#FileCount").val('');
    $("#FileSize").val('');
    $("#RefID").val("");
    $("#frm-grid-columnmetadata #ColLength").val(20);
    $("#frm-grid-columnmetadata #ColDefault").val('');
    var tv = $(this).val();
    if (tv === 'COMBOBOX') {
        $("#ColLength").val(40);
    } else if (tv === 'REFERENCE') {
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
    } else if (tv === 'COMBOBOX') {
        $("#ComboxSource").rules("add", {
            required: true,
            messages: {
                required: "下拉数据源必须填写"
            }
        });
    }
    else if (tv === 'CHECKBOXLIST') {
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
        $("#FileSuffix").val('txt,doc,jpg,gif,png');
        $("#FileCount").val(10);
        $("#FileSize").val(10240);
        $("#ColType").val("UID");
        $("#frm-grid-columnmetadata #ColLength").val(20);
        $("#frm-grid-columnmetadata #ColDefault").val('${FAP::UUID}');
    } else if (tv === 'IMAGE') {
        $("#FileSuffix").val('jpg,gif,png');
        $("#FileCount").val(10);
        $("#FileSize").val(10240);
        $("#ColType").val("UID");
        $("#frm-grid-columnmetadata #ColLength").val(20);
        $("#frm-grid-columnmetadata #ColDefault").val('${FAP::UUID}');
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
                    label: MultiLangHelper.getResName("global_oper_enter", "确定"),
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
                    label: MultiLangHelper.getResName("global_oper_cancel", "取消"), className: "btn-default"
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

//(function(){
//    $.get(basePath + "/System/Api/Tools/ComboxSource", function(data) {
//        $("#ComboxSource").empty();
//        $("#ComboxSource").append('<option value=""></option>');
//        $.each(data, function(i, d){
//            $("#ComboxSource").append('<option value="'+d.category+'">'+d.categoryName+'</option>');
//        });
//    });
//})();