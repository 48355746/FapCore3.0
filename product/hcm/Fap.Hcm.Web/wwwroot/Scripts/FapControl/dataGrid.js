
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
        $.msg($.lang("select_row_empty", "请选中数据操作"));
        return null;
    }
    else {
        var selectedIDs = grid.getGridParam("selarrrow");
        var result = [];
        for (var i = 0; i < selectedIDs.length; i++) {
            var rowData = grid.jqGrid('getRowData', selectedIDs[i]);
            result.push(rowData);
        }
        if (result.length === 0) {
            $.msg($.lang("select_row_empty", "请选中数据操作"));
            return null;
        }
        return result;
    }
}
//获取选中行单选
function getSelectedRow(grdid) {
    var grid = $("#" + grdid);
    var rowKey = grid.getGridParam("selrow");
    if (!rowKey) {
        $.msg($.lang("select_row_empty", "请选中数据操作"));
        return null;
    }
    else {
        var rowData = grid.jqGrid('getRowData', rowKey);
        return rowData;
    }
}
//刷新
function refreshGrid(gridid) {
    $("#" + gridid).trigger("reloadGrid");
}
//重新加载
function reloadGrid(gridid, postData) {
    if (postData === undefined || postData === null) {
        postData = { filters: null };
    }
    $("#" + gridid).jqGrid('setGridParam', {
        datatype: 'json',
        postData: postData, //发送数据
        page: 1
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
var loadFormMessageBox = function (title, gid, icon, tablename, fid, menuid,readonlyCols, fromInitCallback, saveCompletedCallback) {
    var buttons = {
        success: {
            label: $.lang("save", "保存"),
            className: "btn-primary btn-link",
            callback: function () {
                var formid = 'frm-' + gid;
                //持久化
                var res = Persistence(formid, tablename, fromInitCallback, saveCompletedCallback);
                if (res === false) {
                    return false;
                }
                if (res.success === true) {
                    if ($('#' + gid).length && $('#' + gid).length > 0) {
                        $('#' + gid).jqGrid('setGridParam', {
                            //page: 1
                        }).trigger("reloadGrid"); //重新载入
                    }
                    if (fid !== 0) {
                        return false;
                    }
                } else {
                    return false;
                }
            }
        }
    };
    if (fid === 0) {
        buttons.SaveAndAdd = {
            label: $.lang("save_add", "保存并新增"),
            className: "btn-primary btn-link",
            callback: function () {
                var formid = 'frm-' + gid;
                //持久化
                var res = Persistence(formid, tablename);
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
        };
    } else {
        var grid = $("#" + gid);
        buttons.PreBtn = {
            label: $.lang("previous", "上一条"),
            className: "btn-primary btn-link",
            callback: function () {
                var rows = grid.jqGrid('getRowData');
                var fids = $.map(rows, function (d) {
                    return d.Fid;
                });
                var ids = $.map(rows, function (d) {
                    return d.Id;
                });
                var index = $.inArray(fid, fids);
                grid.jqGrid('setSelection', ids[index - 1]);
                if (index === 0) {
                    $.msg($.lang("first_row", "已到达第一条数据"));
                } else {
                    fid = fids[index - 1];
                    initDialog();
                }
                return false;
            }
        };
        buttons.NextBtn = {
            label: $.lang("next", "下一条"),
            className: "btn-primary btn-link",
            callback: function () {
                var rows = grid.jqGrid('getRowData');
                var fids = $.map(rows, function (d) {
                    return d.Fid;
                });
                var ids = $.map(rows, function (d) {
                    return d.Id;
                });
                var index = $.inArray(fid, fids);
                grid.jqGrid('setSelection', ids[index + 1]);
                if (index === fids.length - 1) {
                    $.msg($.lang("end_row", "已到达最后一条数据"));
                } else {
                    fid = fids[index + 1];
                    initDialog();
                }
                return false;
            }
        };
    }
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon ' + icon + '"></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer: false,
        buttons: buttons
    });
    dialog.init(function () {
        initDialog();
    });
    function initDialog() {
        var url = $.randomUrl(basePath + '/Component/Dataform/' + fid);
        $.get(url, { gid: gid, menuid: menuid, fs: 1, tn: tablename, rcols: readonlyCols }, function (ev) {
            dialog.find('.bootbox-body').html(ev);
            if ($.isFunction(fromInitCallback)) {
                fromInitCallback();
            }

        });
    }
};


//查看数据
var viewFormMessageBox = function (fid, gid,tableName, menuid) {
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-search-plus"></i> ' + $.lang("view", "查看"),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer: false
    });
    dialog.init(function () {
        dialog.find('.bootbox-body').load(basePath + "/Component/DataForm/" + fid, { gid: gid, tn: tableName, menuid: menuid, fs: 3 });

    });
};
//删除数据
//logicDelete 逻辑删除
var deleteGridRow = function (gid, tableName,logicDelete, onCompletedCallback) {
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
        bootbox.confirm($.lang("confirm_delete", "确定要删除选中的数据吗?"), function (result) {
            if (result) {
                $.post(basePath + "/Core/Api/Persistence",
                    { "oper": "del", "logicDelete": logicDelete, "tableName": tableName, maindata: { "Fid": dr } }, function (rv) {
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
                            $.msg(rv.msg);
                        }
                    });
            }
        });
    } else {
        $.msg($.lang("select_row_empty", "请选中数据操作"));
    }
};
//isSearch 查询控件参照，这时候多选
var openRefrenceWindow = function (title, colfid, refurl, selectcallback, clearcallback,afterShowcallback, isSearch) {
    if (isSearch === "undefined") {
        isSearch = "0";
    }
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-search "></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    var res;
                    if (isSearch === "1") {
                        res = GetRefResults();
                        if (res) {
                            let code = res.map(function (d) { return d.code; });
                            let name = res.map(function (d) { return d.name; });
                            selectcallback && selectcallback(code, name);
                        } 
                    } else {
                        res = GetRefResult();
                        if (res) {
                            selectcallback && selectcallback(res.code, res.name);
                        } else { $.msg($.lang("select_row_empty", "请选中数据操作")); return; }
                    }
                }
            },
            danger: {
                label: $.lang("clear", "清空"),
                className: "btn-sm btn-danger",
                callback: function () {
                    clearcallback && clearcallback();
                }
            },

            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });
    dialog.on("shown.bs.modal", function () {
        if (refurl.indexOf("GridReference") > -1)
            $(window).triggerHandler('resize.jqGrid');//触发窗口调整,使Grid得到正确的大小
        afterShowcallback && afterShowcallback(dialog);
    });
    dialog.init(function () {
        var url = basePath + '/Component/' + refurl + '/' + colfid + "?isSearch=" + isSearch;
        $.get(url, function (ev) {
            dialog.find('.bootbox-body').html(ev);

        });
    });

};
//批量编辑
var loadBatchUpdateMessageBox = function (title, gid, qryCols, tablename, menuUid, callback) {
    var rowDatas = getSelectedRows(gid);
    if (rowDatas === null) {
        return;
    }
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-pencil-square-o"></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }
    });
    dialog.init(function () {
        dialog.find('.bootbox-body').html(`  <div id="modal-wizard-container"> 
                                               <div> 
                                                <ul class="steps"> 
                                                 <li data-step="1" class="active"> <span class="step">1</span> <span class="title">`+ $.lang("select_property", "选择编辑属性") + `</span> </li> 
                                                 <li data-step="2"> <span class="step">2</span> <span class="title">`+ $.lang("set_property", "设置属性值") + `</span> </li> 
                                                </ul> 
                                               </div> 
                                                <hr/>
                                               <div class="step-content"> 
                                                <div class="step-pane active" data-step="1"> 
                                                 <div class="center"> 
                                                 </div> 
                                                </div> 
                                                <div class="step-pane" data-step="2"> 
                                                 <div class="center"> 
                                                 </div> 
                                                </div> 
                                               </div> 
                                              </div>`);
        //下面button采用a是因为bootbox默认处理button事件
        dialog.find(".modal-footer").html(`<div class="wizard-actions">
												<!-- #section:plugins/fuelux.wizard.buttons -->
												<a class="btn btn-prev">
													<i class="ace-icon fa fa-arrow-left"></i>
													`+ $.lang("previous_step", "上一步") + `
												</a>

												<a class="btn btn-success btn-next" data-last="`+ $.lang("finish", "完成") + `">
													`+ $.lang("next_step", "下一步") + `
													<i class="ace-icon fa fa-arrow-right icon-on-right"></i>
												</a>
                                                <button class="btn btn-danger btn-sm pull-left">
													<i class="ace-icon fa fa-times"></i>
													`+ $.lang("cancel", "取消") + `
												</button>
												<!-- /section:plugins/fuelux.wizard.buttons -->
											</div>`);
        var $fieldList = $("<select multiple='multiple' size='10' id='dualfieldlistbox_" + tablename + "' name='dualfieldlistbox_" + tablename + "'></select>");
        dialog.find(".modal-body .step-content [data-step=1]").append($fieldList);
        $.get(basePath + "/Core/Api/FieldList/" + tablename, { qryCols: qryCols }, function (data) {
            $fieldList.empty();
            $.each(data, function (i, d) {
                if (d.isDefaultCol === 1 || d.showAble === 0) {
                    return true;
                }
                $fieldList.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
            });
            $fieldList.bootstrapDualListbox({
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false

            });
        });
        dialog.find('#modal-wizard-container').ace_wizard({
            //手动指定按钮
            buttons: '.wizard-actions:eq(0)'
        }).on('actionclicked.fu.wizard', function (e, info) {
            if (info.step === 1) {
                var fields = $fieldList.val();
                if (fields === null) {
                    $.msg($.lang("select_row_empty", "请选中数据操作"));
                    e.preventDefault();
                    return;
                }
                dialog.find(".modal-body .step-content [data-step=2]").html('<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>');
                var url = $.randomUrl(basePath + '/Component/Dataform/0');
                $.get(url, { gid: gid, menuid: menuUid, fs: 1, qrycols: fields.join() }, function (ev) {
                    dialog.find(".modal-body .step-content [data-step=2]").html(ev);
                });
                //e.preventDefault();
            } else if (info.step === 2) {
                //alert(0);
            }
        }).on('finished.fu.wizard', function (e) {
            if (!$("#frm-" + gid).valid()) {
                e.preventDefault();
                return false;
            }
            //var fids = $.map(rowDatas, function (d) {
            //    return d.Fid;
            //});
            var ids = $.map(rowDatas, function (d) {
                return d.Id;
            });
            var entityData = {};
            entityData.oper = "batch_edit";
            entityData.mainData = GetFapFormData("frm-" + gid);
            entityData.tableName = tablename;
            entityData.avoid_repeat_token = entityData.mainData["avoid_repeat_token"];
            entityData.Ids = ids.join();
            $.ajax({
                type: "post",
                url: basePath + '/Core/Api/Persistence?from=form',//这里不用带tn 因为 表单中有tn值
                data: entityData,
                async: false,
                dataType: "json",
                headers: {
                    //CSRF攻击
                    'RequestVerificationToken': $("input[name='__RequestVerificationToken']").val()
                },
                success: function (rv) {
                    if (rv.success) {
                        dialog.modal("hide");
                        $.msg(rv.msg);
                        reloadGrid(gid);
                    } else {
                        bootbox.alert(rv.msg);
                    }
                }
            });
        });
    });
};

//导出excel数据
//title 标题
//gid 表格id
//qryCols 导出列
//tablename 表名
//callback 扩展js方法
var loadExportExcelMessageBox = function (title, gid, qryCols, tablename, callback) {
    var $fieldList = $("<select multiple='multiple' size='10' id='duallistbox_" + tablename + "' name='duallistbox_" + tablename + "'></select>");

    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-file-excel-o"></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    var fields = $fieldList.val();
                    if (fields === null) {
                        $.msg($.lang("select_row_empty", "请选中数据操作"));
                        return;
                    }
                    var index = layer.load();
                    var postData = $('#' + gid).jqGrid("getGridParam", "postData");
                    postData.QuerySet.ExportCols = fields.join();
                    $.post(basePath + "/Core/Api/ExportExcelData", postData, function (data) {
                        if (data.success) {
                            window.location.href = basePath + "/" + data.data;
                            //bootbox.alert("生成成功");
                        } else {
                            $.msg($.lang("error", "错误"));
                        }
                        layer.close(index);
                    });
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {
        dialog.find('.bootbox-body').html('');
        dialog.find('.bootbox-body').append($fieldList);
        $.get(basePath + "/Core/Api/FieldList/" + tablename, { qryCols: qryCols }, function (data) {
            $fieldList.empty();
            $.each(data, function (i, d) {
                if (d.colName === "Id" || d.colName === "Fid") {
                    $fieldList.append("<option value='" + d.colName + "'>" + d.colComment + "</option>");
                    return true;
                }
                $fieldList.append("<option value='" + d.colName + "' selected>" + d.colComment + "</option>");

            });
            $fieldList.bootstrapDualListbox({
                nonSelectedListLabel: '<span class="text-primary h4">' + $.lang("pending_item", "待选项") + '</span> ',
                selectedListLabel: '<span class="text-primary h4">' + $.lang("export_item", "导出项") + '</span> ',
                preserveSelectionOnMove: 'moved',
                moveOnSelect: false,
                showFilterInputs: false

            });
        });
    });

};
var loadExportWordMessageBox = function (title, gid, qryCols, tablename, callback) {
    var rowDatas = getSelectedRows(gid);
    if (rowDatas === null)
        return;
    $.post(basePath + "/Core/Api/PrintWordTemplate", { rows: rowDatas, tablename: tablename }, function (rv) {
        if (rv.success) {
            window.location.href = basePath + "/" + rv.data;
        } else {
            var dialog = bootbox.dialog({
                title: '<i class="ace-icon fa fa-file-word-o"></i> ' + $.lang("upload_template", "上传模板"),
                message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
                buttons: {
                    cancel: {
                        label: $.lang("close", "关闭"), className: "btn-default"
                    }
                }

            });

            dialog.init(function () {
                var title = `<h3>系统未发现Word模板，请先上传word模板!!!</h3>
										<p>
										word模板编辑说明：需要替换的内容请使用 "\${列名}"来进行占位。<br/>例如:占位"姓名"，请使用"\${姓名}"。
                                        <strong>系统常量：\${当前日期}，\${登录人}</strong>    
                                       <br/> 注意：系统仅支持<strong>.docx</strong>后缀word模板
										</p>`;

                var $file = $("<input id=\"file-import\" type=\"file\"  class=\"file-loading\">");
                dialog.find('.bootbox-body').empty().append(title).append($file);
                $file.fileinput({
                    language: language,
                    uploadUrl: basePath + '/Core/Api/ImportWordTemplate/' + tablename,
                    showCaption: false,
                    allowedFileExtensions: ["docx"],
                    showClose: true
                });
            });
        }
    });
};
var loadChartMessageBox = function (title, gridid, tablename) {
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-bar-chart"></i> ' + $.lang("chart", "图表"),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size:"fullscreen",
        buttons: {
            cancel: {
                label: $.lang("close", "关闭"), className: "btn-default"
            }
        }

    }); 
    dialog.init(function () {
        $.get(basePath + '/Component/ChartSet', { tablename: tablename, gridId: gridid }, function (ev) {
            dialog.find(".modal-body").html(ev);
        });
    });
};
//导出excel模板
//title 标题
//tablename 表名
var loadExportExcelTmpl = function (qryCols, tableName) {
    var postData = { TableName: tableName, QueryCols: qryCols };
    $.post(basePath + "/Core/Api/ExportExcelTmpl", postData, function (data) {
        if (data.success) {
            window.location.href = basePath + "/" + data.data;
            //bootbox.alert("生成成功");
        } else {
            $.msg($.lang("error", "错误"));
        }
    });
};
var loadExportExcelTemplData = function (qryCols, gid) {
    var postData = $('#' + gid).jqGrid("getGridParam", "postData");
    postData.QuerySet.ExportCols = qryCols;
    $.post(basePath + "/Core/Api/ExportExcelTmplData", postData, function (data) {
        if (data.success) {
            window.location.href = basePath + "/" + data.data;
            //bootbox.alert("生成成功");
        } else {
            bootbox.alert($.lang("error", "错误"));
        }
    });
};
//导入
var loadImportDataMessageBox = function (title, gid, qryCols, tablename, callback) {
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-cloud-upload"></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    $('#' + gid).jqGrid('setGridParam', {
                        //page: 1
                    }).trigger("reloadGrid"); //重新载入
                }
            },
            cancel: {
                label: $.lang("cancel", "取消"), className: "btn-default"
            }
        }

    });
    dialog.init(function () {
        var title1 = $("<h3 class=\"header smaller lighter blue\">1、" + $.lang("dowload_template", "下载模板") + "</h3>");
        //下载链接
        var $linkDown = $("<button class=\"btn btn-info btn-sm\"><i class=\"ace-icon fa fa-download\"></i>" + $.lang("download_empty_template", "下载空模板") + "</button>");
        var $linkDownData = $("<button class=\"btn btn-success btn-sm\"><i class=\"ace-icon fa fa-download\"></i>" + $.lang("download_data_template", "下载数据模板") + "</button>");
        var title2 = "<h3 class=\"header smaller lighter blue\">2、" + $.lang("import_data", "导入数据") + "</h3>";
        var $file = $("<input id=\"file-import\" type=\"file\"  class=\"file-loading\">");
        var $p = $("<p>").append($linkDown).append($linkDownData);
        dialog.find('.bootbox-body').empty().append(title1).append($p).append(title2).append($file);
        //下载模板
        $linkDown.on(ace.click_event, function () {
            loadExportExcelTmpl(qryCols, tablename);
        });
        $linkDownData.on(ace.click_event, function () {
            loadExportExcelTemplData(qryCols, gid);
        });
        $file.fileinput({
            language: language,
            uploadUrl: basePath + '/Core/Api/ImportExcelData/' + tablename,
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
        title: $.lang("view_annex", "查看附件"),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {

            cancel: {
                label: $.lang("close", "关闭"), className: "btn-default"
            }
        }

    });
    dialog.init(function () {
        var loadUrl;

        loadUrl = basePath + "/Component/AttachmentInfo/" + fid;

        $.get(loadUrl, function (ev) {
            dialog.find('.bootbox-body').html(ev);

        });
    });


};
var attachmentInfo = function (cellvalue, options, rowObject) {
    var tempHtml = "<button class=\"btn  btn-link btn-attachment\" data-value=\"" + cellvalue + "\">";
    tempHtml += "         <i class=\"ace-icon fa  fa-paperclip bigger-120 blue\"></i>" + $.lang("annex", "附件") + "</button>";
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
    var imageHtml = "<img src='/Component/Photo/" + cellValue + "'  style='width:60px' originalValue='" + cellValue + "' />";
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
    if (cellValue === "undefined" || cellValue === null) {
        return "<label data-value=''></label>";
    }
    var colName = options.colModel.name + "MC";
    var v = rowObject[colName] === null ? "" : rowObject[colName];

    return "<label data-value='" + cellValue + "'>" + v + "</label>";
};
var unformatReference = function (cellValue, options, cellObject) {

    return $(cellObject.innerHTML).data("value");
};
//搜索参照控件
var searchOptionReference = function (el, title, fid, reftype) {
    $(el).addClass("hide");
    var $ctrlRef = $(`<span class="input-icon input-icon-right">
												<input type="text" style="width:100%;height:30px;padding-right: 25px !important;">
												<i class="ace-icon fa fa-search block"></i>
											</span>`);
    $(el).parent().append($ctrlRef);
    var $ref = $ctrlRef.find('input:text');
    let v = $(el).val();
    //赋值txt
    if (v !== undefined && v !== "") {
        $.post(basePath + "/Core/Api/RefSearchText", { rv: v, colUid: fid }, function (rv) {
            if (rv.success) {
                $ref.val(rv.data);
            } else {
                $.msg(rv.msg);
            }
        })
    }
    $ref.on(ace.click_event, function (e) { $(this).next().trigger(ace.click_event); e.preventDefault(); });
    $ref.next().on(ace.click_event, function () {
        openRefrenceWindow(title, fid, reftype, function (code, name) {
            $(el).val(code);
            $(el).trigger("change");
            $ref.val(name);
        }, function () {
                $(el).val("");
                $ref.val("");
                $(el).trigger("change");
        }, function (dialog) {
            var parentZ = $(el).parents("*[role=dialog]").filter(':first').css("z-index");
            let zIndex = parseInt(parentZ, 10) + 2;
            dialog.zIndex(zIndex);
        }, "1");
    });
}
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
function resetGridSize(grdid, wrapper) {
    var parent_width = $('#' + grdid).closest(wrapper).width();
    $('#' + grdid).jqGrid('setGridWidth', parent_width);
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
    buttons.eq(1).prepend('<i class="ace-icon fa fa-times"></i>');

    buttons = form.next().find('.navButton a');
    buttons.find('.ui-icon').hide();
    buttons.eq(0).append('<i class="ace-icon fa fa-chevron-left"></i>');
    buttons.eq(1).append('<i class="ace-icon fa fa-chevron-right"></i>');
}

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
function loadQueryProgram(form, gridid, tn) {
    var dialog = form.closest('.ui-jqdialog');
    var buttons = dialog.find('.EditTable');
    var $selQryPrm = $(`<select id=fbox_"` + gridid + `_selectqry"><option value=''>---` + $.lang("common_query", "常用查询") + `---</option></select>`);

    if (buttons.find('.EditButton select[id*="_selectqry"]')[0] === undefined) {
        $.get(basePath + "/Core/Api/QueryProgram/" + tn, function (rvm) {
            if (rvm.success) {
                $("#" + gridid).data("queryprogram", rvm.data);
                $.each(rvm.data, function (i, d) {
                    $selQryPrm.append(`<option value="` + d.fid + `">` + d.programName + `</option>`);
                });
            }
        });
        $selQryPrm.on('change', function () {
            var fid = $(this).find('option:selected').val();
            var qryData = $("#" + gridid).data("queryprogram");
            var filter = $.grep(qryData, (d) => { return d.fid === fid; });
            if (filter.length === 1) {
                $("#fbox_" + gridid).jqFilter("addFilter", filter[0].queryCondition);
                buttons.find('.EditButton a[id*="_search"]').trigger('click');
            }
        });
        buttons.find('.EditButton a[id*="_search"]').before($selQryPrm.eq(0));
    }
}
//重绘后执行
function addQueryProgram(form, gridid, tn) {
    var $qryProgram = $(`<a id="fbox_` + gridid + `_queryprogram" class="fm-button ui-state-default ui-corner-all fm-button-icon-left btn btn-sm btn-default pull-right"><span class="ace-icon fa fa-save"></span>` + $.lang("saveas_common_query", "保存为常用查询") + `</a>`);
    form.find('.add-rule').first().after($qryProgram.eq(0));
    $qryProgram.on(ace.click_event, function () {
        var dialog = form.closest('.ui-jqdialog');
        var buttons = dialog.find('.EditTable');
        buttons.find('.EditButton a[id*="_search"]').trigger('click');
        var jqPostData = $('#' + gridid).jqGrid("getGridParam", "postData");
        if (jqPostData.filters === undefined) {
            $.msg($.lang("first_query", "请先查询"));
            return;
        }
        //var sqlRv = JSON.stringify(jqPostData.filters );
        bootbox.prompt($.lang("common_query", "常用查询"), function (result) {
            if (result === null) {
                //alert(1);
            } else {
                if (result !== "") {
                    $.post(basePath + "/Core/Api/QueryProgram", { ProgramName: result, TableName: tn, QueryCondition: jqPostData.filters }, function (rvm) {
                        if (rvm.success) {
                            $.msg($.lang("success", "成功"));
                            //添加新的查询方案
                            var qryData = $("#" + gridid).data("queryprogram");
                            let d = rvm.data;
                            qryData.push(d);
                            $("#" + gridid).data("queryprogram", qryData);
                            buttons.find('.EditButton select[id*="_selectqry"]').append(`<option value="` + d.fid + `">` + d.programName + `</option>`);

                        } else {
                            bootbox.alert($.lang("failure", "失败"));
                        }
                    });
                } else {
                    bootbox.alert($.lang("common_query_required", "常用查询名称必填"));
                }

            }
        });
    });

}

function beforeDeleteCallback(e) {
    var form = $(e[0]);
    if (form.data('styled')) return false;

    form.closest('.ui-jqdialog').find('.ui-jqdialog-titlebar').wrapInner('<div class="widget-header" />');
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
    var rowIndex = editOptions.rowId;
    return $('<span class="col-sm-11"><span class="input-icon input-icon-right"><input type="text" id="' + rowIndex + '_' + colName + '" name="' + colName + '" rowid="' + rowIndex + '" data-value="' + value + '" value="' + txt + '" /> <i class="ace-icon fa fa-search block"></i>        </span></span >');
}
function getReferenceElementValue(elem, oper, value) {
    if (oper === "set") {
        var input = $(elem).find("input:text");
        input.val(value).data('value', value);
    }
    if (oper === "get") {
        return $(elem).find("input:text").data("value") === undefined ? "" : $(elem).find("input:text").data("value");
    }
}