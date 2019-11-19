//扩展jsgrid工具栏按钮
var extendToolBar = function (grdid, pagerid, name, icon, extendEvent) {
    jQuery('#' + grdid).jqGrid('navButtonAdd', '#' + pagerid, {
        caption: name,
        title: name,
        position: 'first',
        buttonicon: icon,//'ace-icon fa fa-pencil blue'
        onClickButton: function () {
            extendEvent();
        }
    });
};
//获取选中行多选
function getSelectedRows(grdid) {
    var grid = $("#" + grdid);
    var rowKey = grid.getGridParam("selrow");
    if (!rowKey) {
        $.msg("没有行被选中！");
        return null;
    }
    else {
        var selectedIDs = grid.getGridParam("selarrrow");
        var result = [];
        for (var i = 0; i < selectedIDs.length; i++) {
            var rowData = grid.jqGrid('getRowData', selectedIDs[i]);
            result.push(rowData);
        }
        return result;
    }
}
//获取选中行单选
function getSelectedRow(grdid) {
    var grid = $("#" + grdid);
    var rowKey = grid.getGridParam("selrow");
    if (!rowKey) {
        $.msg("没有行被选中！");
        return null;
    }
    else {
        var rowData = grid.jqGrid('getRowData', rowKey);
        return rowData;
    }
}
function refreshBaseJqGrid(grdid) {
    var grid = $("#" + grdid);
    grid.jqGrid('setGridParam', {

    }).trigger("reloadGrid"); //重新载入
}
//新增，编辑方法
//title 标题
//gid 表格id
//icon 小图标
//tablename 表名
//id 业务数据id，0新增
//callback 扩展js方法
//title 标题;gid jqgrid的ID;icon 图标;tablename 表名（frm-tablename 表单名称）;
//id业务数据主键值;fromInitCallback 表单初始化事件;saveCompletedCallback 保存完毕事件
var loadFormMessageBox = function (title, gid, icon, tablename, id, fromInitCallback, saveCompletedCallback) {
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon ' + icon + '"></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer:false,
        buttons: {
            success: {
                label: MultiLangHelper.getResName("global_oper_save", "保存"),
                className: "btn-primary",
                callback: function () {
                    var formid = 'frm-' + tablename;
                    //持久化
                    var res = Persistence(formid, '');
                    if (res === false) {
                        return false;
                    }
                    if (res.success === true) {
                        if ($.isFunction(saveCompletedCallback)) {
                            saveCompletedCallback();
                        }
                        if ($('#' + gid).length && $('#' + gid).length > 0) {
                            $('#' + gid).jqGrid('setGridParam', {
                                //page: 1
                            }).trigger("reloadGrid"); //重新载入
                        }
                    } else {

                        return false;
                    }
                }
            },
            SaveAndAdd: {
                label: "保存并新增",
                className: "btn-primary",
                callback: function () {
                    var formid = 'frm-' + tablename;
                    //持久化
                    var res = Persistence(formid, '');
                    if (res === false) {
                        return false;
                    }
                    if (res.success === true) {
                        if ($.isFunction(saveCompletedCallback)) {
                            saveCompletedCallback();
                        }
                        if ($('#' + gid).length && $('#' + gid).length > 0) {
                            $('#' + gid).jqGrid('setGridParam', {
                                //page: 1
                            }).trigger("reloadGrid"); //重新载入
                        }
                        initDialog();
                        return false;
                    } else {

                        return false;
                    }
                }
            }
        }

    });
    dialog.init(function () {
        initDialog();
    });
    function initDialog() {
        var url = $.randomUrl(basePath + '/PublicCtrl/Dataform/' + id + '?tn=' + tablename);
        $.get(url, function (ev) {
            dialog.find('.bootbox-body').html(ev);
            if ($.isFunction(fromInitCallback)) {
                fromInitCallback();
            }

        });
    }
};


//查看数据
var viewFormMessageBox = function (fid, tablename) {
    var dialog = bootbox.dialog({
        title: '查看',
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer:false
    });
    dialog.init(function () {
        dialog.find('.bootbox-body').load(basePath + "/PublicCtrl/DataFormView/0", { fid: fid, tn: tablename });

    });
};
//删除数据
//logicDelete 逻辑删除
var deleteGridRow = function (gid, tableName, logicDelete,formtoken, onCompletedCallback) {
    var multiselect = $('#' + gid).jqGrid('getGridParam', 'multiselect');
    var dr;
    if (multiselect) {
        dr = $('#' + gid).jqGrid('getGridParam', 'selarrrow');
        if (dr.length === 0) {
            dr = null;
        }
        else {
            var result = [];
            for (var i = 0; i < dr.length; i++) {
                var rowData = $('#' + gid).jqGrid('getRowData', dr[i]);
                result.push(rowData.Fid);
            }
            dr = result.join(",");
        }
    } else {
        dr = jQuery('#' + gid).jqGrid('getGridParam', 'selrow');
        var rd = $('#' + gid).jqGrid('getRowData', dr);
        dr = rd.Fid;
    }
    if (dr) {
        bootbox.confirm('确定要删除选中的吗?', function (result) {
            if (result) {
                $.post(basePath + "/api/coreapi/Persistence/", { "oper": "del", "Table_Name": tableName, "formtoken": formtoken ,"logicdelete": logicDelete, "Fid": dr }, function (rv) {
                    if (rv.success) {
                        if ($.isFunction(onCompletedCallback)) {
                            onCompletedCallback();
                        }
                        if ($('#' + gid).length && $('#' + gid).length > 0) {
                            $('#' + gid).jqGrid('setGridParam', {
                                //page: 1
                            }).trigger("reloadGrid"); //重新载入
                        }
                    }
                    if (rv.msg) {
                        bootbox.alert(rv.msg);
                    }
                });
            }
        });
    } else {
        bootbox.alert('请选择一条数据');
    }

};
var openRefrenceWindow = function (title, colid,  refurl, selectcallback,clearcallback) {
    var dialog = bootbox.dialog({
        title: title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: MultiLangHelper.getResName("global_oper_enter", "确定"),
                className: "btn-primary",
                callback: function () {
                    var res = GetRefResult();
                    if (res) {
                        selectcallback && selectcallback(res.code, res.name);                                          
                    } else { $.msg("请选择一条数据！"); return; }
                }
            },
            danger: {
                label: "清空!",
                className: "btn-sm btn-danger",
                callback: function () {
                    clearcallback && clearcallback();
                }
            },

            cancel: {
                label: MultiLangHelper.getResName("global_oper_cancel", "取消"), className: "btn-default"
            }
        }

    });
    dialog.on("shown.bs.modal", function () {
        if (refurl.indexOf("GridReference") > -1)
            $(window).triggerHandler('resize.jqGrid');//触发窗口调整,使Grid得到正确的大小
    });
    dialog.init(function () {
        var url = basePath + '/PublicCtrl/' + refurl + '/' + colid ;        
        $.get(url, function (ev) {
            dialog.find('.bootbox-body').html(ev);

        });
    });

};
//批量编辑
var loadBatchUpdateMessageBox = function (title, gid, icon, tablename, id, callback) {
    var rowDatas = getSelectedRows(gid);
    if (rowDatas === null)
        return;
    var $modal = $("#modal-wizard-" + gid);
    if ($modal.length === 0) {
        var wizardModal = ["<div id=\"modal-wizard-" + gid + "\" class=\"modal\">",
            "    <div class=\"modal-dialog\">",
            "        <div class=\"modal-content\">",
            "            <div id=\"modal-wizard-container\">",
            "                <div class=\"modal-header\">",
            "                    <ul class=\"steps\">",
            "                        <li data-step=\"1\" class=\"active\">",
            "                            <span class=\"step\">1</span>",
            "                            <span class=\"title\">选择编辑属性</span>",
            "                        </li>",
            "                        <li data-step=\"2\">",
            "                            <span class=\"step\">2</span>",
            "                            <span class=\"title\">设置属性值</span>",
            "                        </li>                       ",
            "                    </ul>",
            "                </div>",
            "                <div class=\"modal-body step-content\">",
            "                    <div class=\"step-pane active\" data-step=\"1\">",
            "                        <div class=\"center\">",
            // "                            <h4 class=\"blue\">Step 1</h4>",
            "                        </div>",
            "                    </div>",
            "                    <div class=\"step-pane\" data-step=\"2\">",
            "                        <div class=\"center\">",
            //"                            <h4 class=\"blue\">Step 2</h4>",
            "                        </div>",
            "                    </div>                   ",
            "                </div>",
            "            </div>",
            "            <div class=\"modal-footer wizard-actions\">",
            "                <button class=\"btn btn-sm btn-prev\">",
            "                    <i class=\"ace-icon fa fa-arrow-left\"></i>",
            "                   上一步",
            "                </button>",
            "                <button class=\"btn btn-success btn-sm btn-next\" data-last=\"完成\">",
            "                   下一步",
            "                    <i class=\"ace-icon fa fa-arrow-right icon-on-right\"></i>",
            "                </button>",
            "                <button class=\"btn btn-danger btn-sm pull-left btnCancel\">",
            "                    <i class=\"ace-icon fa fa-times\"></i>",
            "                    取消",
            "                </button>",
            "            </div>",
            "        </div>",
            "    </div>",
            "</div>"].join("");
        $("body").append($(wizardModal));
        $modal = $(wizardModal);
    }
    $modal.find(".modal-footer .btnCancel").on(ace.click_event, function () {
        $modal.modal("hide");
        $modal.remove();
    });
    var $fieldList = $("<select multiple='multiple' size='10' id='dualfieldlistbox_" + tablename + "' name='dualfieldlistbox_" + tablename + "'></select>");
    $modal.find('#modal-wizard-container').ace_wizard().on('actionclicked.fu.wizard', function (e, info) {
        if (info.step === 1) {
            var fields = $fieldList.val();
            if (fields === null) {
                $.msg("请选择要批量更新的属性！");
                e.preventDefault();
                return;
            }
            $modal.find(".step-content [data-step=2]").html('<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>');
            var url = $.randomUrl(basePath + '/PublicCtrl/Dataform/0?tn=' + tablename + "&frm=batchupdate&qrycols=" + fields.join());
            $.get(url, function (ev) {
                $modal.find(".step-content [data-step=2]").html(ev);
            });
            //e.preventDefault();
        } else if (info.step === 2) {
            //alert(0);
        }
    }).on('finished.fu.wizard', function (e) {
        if (!$("#frm-batchupdate").valid()) {
            e.preventDefault();
            return false;
        }
        var fids = $.map(rowDatas, function (d) {
            return d.Fid;
        });
        var formData = GetFapFormData("frm-batchupdate");
        formData.Fids = fids;
        $.post(basePath + "/api/coreapi/BatchUpdate", formData, function (rv) {
            if (rv.success) {
                $modal.modal("hide");
                $modal.remove();
                $.msg("批量更新成功！");
                refreshBaseJqGrid(gid);
            } else {
                bootbox.alert(rv.msg);
            }
        });
    }).on('stepclick.fu.wizard', function (e) {
        //e.preventDefault();//this will prevent clicking and selecting steps
        //alert(20);
    });
    //$('#modal-wizard .wizard-actions .btn[data-dismiss=modal]').removeAttr('disabled');

    $modal.modal("show");
    $modal.find(".modal-body [data-step=1]").html("");
    $modal.find(".modal-body [data-step=1]").append($fieldList);

    $.get(basePath + "/api/coreapi/fieldlist/" + tablename, function (data) {
        $fieldList.empty();
        $.each(data, function (i, d) {
            if (d.isDefaultCol === 1 || d.showAble === 0) {
                return true;
            }
            $fieldList.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
        });
        $fieldList.bootstrapDualListbox({
            //nonSelectedListLabel: '<span class="text-primary h5">所有项</span> ',
            //selectedListLabel: '<span class="text-primary h5">选中项</span> ',
            preserveSelectionOnMove: 'moved',
            moveOnSelect: false

        });
    });
};

//导出excel数据
//title 标题
//gid 表格id
//icon 小图标
//tablename 表名
//id 业务数据id，0新增
//callback 扩展js方法
var loadExportMessageBox = function (title, gid, icon, tablename, id, callback) {
    var $fieldList = $("<select multiple='multiple' size='10' id='duallistbox_" + tablename + "' name='duallistbox_" + tablename + "'></select>");

    var dialog = bootbox.dialog({
        title: title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: MultiLangHelper.getResName("global_oper_enter", "确定"),
                className: "btn-primary",
                callback: function () {
                    var fields = $fieldList.val();
                    if (fields === null) {
                        $.msg("请选择导出字段！");
                        return;
                    }

                    //alert($fieldList.val());
                    var postData = $('#' + gid).jqGrid("getGridParam", "postData");
                    postData.QuerySet.ExportCols = fields.join();
                    //var sqlRv = JSON.stringify(postData);
                    //alert(sqlRv);
                    $.post(basePath + "/api/coreapi/export", postData, function (data) {
                        if (data.rv) {
                            window.location.href = basePath + "/UploadFiles/" + data.fn;
                            //bootbox.alert("生成成功");
                        } else {
                            $.msg("生成文件异常！");
                        }
                    });
                }
            },
            cancel: {
                label: MultiLangHelper.getResName("global_oper_cancel", "取消"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {
        dialog.find('.bootbox-body').html('');
        dialog.find('.bootbox-body').append($fieldList);
        $.get(basePath + "/api/coreapi/fieldlist/" + tablename, function (data) {
            $fieldList.empty();
            $.each(data, function (i, d) {
                if (d.isDefaultCol === 1 && d.colName !== 'Fid') {
                    return true;
                }
                $fieldList.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
            });
            $fieldList.bootstrapDualListbox({
                nonSelectedListLabel: '<span class="text-primary h5">所有项</span> ',
                selectedListLabel: '<span class="text-primary h5">选中项</span> ',
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false

            });
        });

    });

};
//导出excel模板
//title 标题
//tablename 表名
var loadExportTmplMessageBox = function (title, tablename) {
    $.get(basePath + "/api/coreapi/exporttmpl/" + tablename, function (data) {
        if (data.rv) {
            window.location.href = basePath + "/UploadFiles/" + data.fn;
            //bootbox.alert("生成成功");
        } else {
            $.msg("模板生成失败！");
        }
    });
};
var loadExportDataMessageBox = function (title, tableName) {
    var postData = { TableName: tableName };
    $.post(basePath + "/api/coreapi/exportdata", postData, function (data) {
        if (data.rv) {
            window.location.href = basePath + "/UploadFiles/" + data.fn;
            //bootbox.alert("生成成功");
        } else {
            bootbox.alert("生成文件异常！");
        }
    });
};
//导入
var loadImportDataMessageBox = function (title, gid, icon, tablename, id, callback) {
    var dialog = bootbox.dialog({
        title: title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: MultiLangHelper.getResName("global_oper_enter", "确定"),
                className: "btn-primary",
                callback: function () {
                    $('#' + gid).jqGrid('setGridParam', {
                        //page: 1
                    }).trigger("reloadGrid"); //重新载入
                }
            },
            cancel: {
                label: MultiLangHelper.getResName("global_oper_cancel", "取消"), className: "btn-default"
            }
        }

    });
    dialog.init(function () {
        var title1 = $("<h3 class=\" smaller lighter blue\">1.下载模板</h3>");
        //下载链接
        var $linkDown = $("<button class=\"btn btn-success\"><i class=\"ace-icon fa fa-download\"></i>下载模板</button>");
        var $linkDownData = $("<button class=\"btn btn-success\"><i class=\"ace-icon fa fa-download\"></i>下载数据</button>");
        var title2 = "<h3 class=\" smaller lighter blue\">2.导入数据</h3>";
        var $file = $("<input id=\"file-import\" type=\"file\"  class=\"file-loading\">");
        dialog.find('.bootbox-body').empty().append(title1).append($linkDown).append($linkDownData).append(title2).append($file);
        //下载模板
        $linkDown.on(ace.click_event, function () {
            loadExportTmplMessageBox('', tablename);
        });
        $linkDownData.on(ace.click_event, function () {
            loadExportDataMessageBox('', tablename);
        });
        $file.fileinput({
            language: 'zh',
            uploadUrl: basePath + '/api/coreapi/impdata/' + tablename,
            showCaption: false,
            allowedFileExtensions: ["xls", "xlsx"],
            showClose: true
        });


    });

};
//gridcomplete事件中注册
var registAttachmentFuntion = function (grdid) {
    $("#" + grdid + " .btn-attachment").on(ace.click_event, function (e) {
        e.preventDefault();
        var fid = $(this).data("value");
        showAttachmentWin(fid, grdid);
    });
};
//显示附件框
var showAttachmentWin = function (fid, grdid) {
    var dialog = bootbox.dialog({
        title: "附件查看",
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {

            cancel: {
                label: "关闭", className: "btn-default"
            }
        }

    });
    dialog.init(function () {
        var loadUrl;

        loadUrl = basePath + "/PublicCtrl/AttachmentInfo/" + fid;

        $.get(loadUrl, function (ev) {
            dialog.find('.bootbox-body').html(ev);

        });
    });


};
var attachmentInfo = function (cellvalue, options, rowObject) {
    var tempHtml = "<button class=\"btn  btn-link btn-attachment\" data-value=\"" + cellvalue + "\">";
    tempHtml += "         <i class=\"ace-icon fa  fa-paperclip bigger-120 blue\"></i>附件</button>";
    return tempHtml;
};
var unattachmentInfo = function (cellvalue, options, rowObject) {
    return $(cellObject.innerHTML).data("value");
};
// The FormatFunction for CustomFormatter gets three parameters           
// cellValue - the original value of the cell
// options - as set of options, e.g
//   options.rowId - the primary key of the row
//   options.colModel - colModel of the column
// rowData - array of cell data for the row, so you can access other cells in the row if needed
var formatImage = function (cellValue, options, rowObject) {
    var imageHtml = "<img src='/Home/UserPhoto/" + cellValue + "'  style='width:60px' originalValue='" + cellValue + "' />";
    return imageHtml;
};

// The FormatFunction for CustomFormatter gets three parameters           
// cellValue - the original value of the cell
// options - as set of options, e.g
//   options.rowId - the primary key of the row
//   options.colModel - colModel of the column
// cellObject - the HMTL of the cell (td) holding the actual value
var unformatImage = function (cellValue, options, cellObject) {
    return $(cellObject.innerHTML).attr("originalValue");
};
var formatReference = function (cellValue, options, rowObject) {
    if (cellValue === undefined) {
        return "<label data-value=''></label>";
    }
    var colName = options.colModel.name + "MC";
    var v = rowObject[colName] === null ? "" : rowObject[colName];

    return "<label data-value='" + cellValue +"'>" + v+ "</label>";
};
var unformatReference = function (cellValue, options, cellObject) {
    
    return $(cellObject.innerHTML).data("value");
};
//replace icons with FontAwesome icons like above
function updatePagerIcons(table) {
    var replacement =
    {
        'ui-icon-seek-first': 'ace-icon fa fa-angle-double-left bigger-140',
        'ui-icon-seek-prev': 'ace-icon fa fa-angle-left bigger-140',
        'ui-icon-seek-next': 'ace-icon fa fa-angle-right bigger-140',
        'ui-icon-seek-end': 'ace-icon fa fa-angle-double-right bigger-140'
    };
    $('.ui-pg-table:not(.navtable) > tbody > tr > .ui-pg-button > .ui-icon').each(function () {
        var icon = $(this);
        var $class = $.trim(icon.attr('class').replace('ui-icon', ''));

        if ($class in replacement) icon.attr('class', 'ui-icon ' + replacement[$class]);
    });
}
function enableTooltips(table) {
    $('.navtable .ui-pg-button').tooltip({ container: 'body' });
    $(table).find('.ui-pg-div').tooltip({ container: 'body' });
}

//采用元数据生成表单，此js没用
function style_edit_form(form) {
    //日期控件自己加     
    //form.find('input[name=sdate]').datepicker({ format: 'yyyy-mm-dd', autoclose: true })


    //form.find('input[name=stock]').addClass('ace ace-switch ace-switch-5').after('<span class="lbl"></span>');
    //don't wrap inside a label element, the checkbox value won't be submitted (POST'ed)
    //.addClass('ace ace-switch ace-switch-5').wrap('<label class="inline" />').after('<span class="lbl"></span>');

    //update buttons classes
    var buttons = form.next().find('.EditButton .fm-button');
    buttons.addClass('btn btn-sm').find('[class*="-icon"]').hide();//ui-icon, s-icon
    buttons.eq(0).addClass('btn-primary').prepend('<i class="ace-icon fa fa-check"></i>');
    buttons.eq(1).prepend('<i class="ace-icon fa fa-times"></i>')

    buttons = form.next().find('.navButton a');
    buttons.find('.ui-icon').hide();
    buttons.eq(0).append('<i class="ace-icon fa fa-chevron-left"></i>');
    buttons.eq(1).append('<i class="ace-icon fa fa-chevron-right"></i>');
};

function style_delete_form(form) {
    var buttons = form.next().find('.EditButton .fm-button');
    buttons.addClass('btn btn-sm btn-white btn-round').find('[class*="-icon"]').hide();//ui-icon, s-icon
    buttons.eq(0).addClass('btn-danger').prepend('<i class="ace-icon fa fa-trash-o"></i>');
    buttons.eq(1).addClass('btn-default').prepend('<i class="ace-icon fa fa-times"></i>');
}

function style_search_filters(form) {
    form.find('.delete-rule').val('X');
    form.find('.add-rule').addClass('btn btn-xs btn-primary');
    form.find('.add-group').addClass('btn btn-xs btn-success');
    form.find('.delete-group').addClass('btn btn-xs btn-danger');
}
function style_search_form(form) {
    var dialog = form.closest('.ui-jqdialog');
    var buttons = dialog.find('.EditTable');
    buttons.find('.EditButton a[id*="_reset"]').addClass('btn btn-sm btn-info').find('.ui-icon').attr('class', 'ace-icon fa fa-retweet');
    buttons.find('.EditButton a[id*="_query"]').addClass('btn btn-sm btn-inverse').find('.ui-icon').attr('class', 'ace-icon fa fa-comment-o');
    buttons.find('.EditButton a[id*="_search"]').addClass('btn btn-sm btn-purple').find('.ui-icon').attr('class', 'ace-icon fa fa-search');
}

function beforeDeleteCallback(e) {
    var form = $(e[0]);
    if (form.data('styled')) return false;

    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar').wrapInner('<div class="widget-header" />')
    style_delete_form(form);

    form.data('styled', true);
}

function beforeEditCallback(e) {
    var form = $(e[0]);
    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar').wrapInner('<div class="widget-header" />')
    style_edit_form(form);
}


function formatRating(cellValue, options, rowObject) {
    var color = (parseInt(cellValue) > 0) ? "green" : "red";
    var cellHtml = "<span style='color:" + color + "' originalValue='" +
                         cellValue + "'>" + cellValue + "</span>";

    return cellHtml;
}

function unformatRating(cellValue, options, cellObject) {
    return $(cellObject.html()).attr("originalValue");
}
//针对checkboxlist格式化(id:name;id:name...)
function formatCheckboxList(cellValue, options, cellObject) {
    if (!cellValue) {
        return "<span  originalValue=''></span>";
    }
    var values = cellValue.split(";");
    var arrLength = values.length;
    if (arrLength < 1) {
        return "<span  originalValue='" + cellValue + "'>" + cellValue + "</span>";
    }
    var cellUid = [], cellText = [];
    for (var i = 0; i < arrLength; i++) {
        var chks = values[i].split(":");
        if (chks.length > 0) {
            cellUid.push(chks[0]);
            cellText.push(chks[1]);
        }
    }
    var cellHtml = "<span  originalValue='" +
                         cellUid.join() + "'>" + cellText.join() + "</span>";
    return cellHtml;
}
function unformatCheckboxList(cellValue, options, cellObject) {
    return $(cellObject.html()).attr("originalValue");
}
function createReferenceEditElement(value, editOptions, rowObject) {
    var colName = editOptions.name + "MC";
    var txt = rowObject[colName];
    return $('<span class="col-sm-11"><span class="input-icon input-icon-right"><input type="text" id="form-field-icon-2" data-value="' + value + '" value="' + txt + '" /> <i class="ace-icon fa fa-search block"></i>        </span></span >');
}
function getReferenceElementValue(elem, oper, value) {
    if (oper === "set") {
        var input = $(elem).find("input:text");
        input.val(value).data('value',value);
    }
    if (oper === "get") {
        return $(elem).find("input:text").data("value") === undefined ? "" : $(elem).find("input:text").data("value");
    }
}