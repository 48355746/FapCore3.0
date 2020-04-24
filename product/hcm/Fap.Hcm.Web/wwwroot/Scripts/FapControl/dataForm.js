(function ($) {
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };

})(jQuery);
//获取checkboxlist 的值，形如:"a:b;c:d"
jQuery.GetChkListValue = function (cellValue) {
    var rv = [];
    if (!cellValue) {
        return rv;
    }
    var values = cellValue.split(";");
    var arrLength = values.length;
    if (arrLength < 1) {
        return [];
    }
    for (var i = 0; i < arrLength; i++) {
        var chks = values[i].split(":");
        if (chks.length > 0) {
            rv.push(chks[0]);
            //cellText.push(chks[1]);
        }
    }
    return rv;
};

//获取表单数据
var GetFapFormData = function (formid) {
    var disabledCtrl = [];
    $("#" + formid + " :disabled").each(function () {//移除disable,后台方可取值        
        $(this).attr("disabled", false);
        disabledCtrl.push($(this));
    });
    var formData = {};
    $("#" + formid + " .form-control").each(function () {
        if ($(this).attr("id") === undefined) {
            return;
        }
        if ($(this).is('input') && $(this).attr('type') === 'checkbox') {
            formData[$(this).attr("id")] = $(this).is(":checked") ? $(this).val() : 0;
            return;
        }

        //富文本编辑控件
        if ($(this).hasClass("wysiwyg-editor")) {
            formData[$(this).attr("id")] = $(this).html();
            return;
        }
        formData[$(this).attr("id")] = $(this).val();
    });
    //获取checkbox分组数据
    $("#" + formid + " .control-group.checkboxlist").each(function () {

        var cn = $(this).data("ctrlname");
        var cv = [];
        $("input[name='" + cn + "']:checked").each(function () {
            var val = $(this).val() + ":" + $(this).data("text");
            cv.push(val);
        });
        formData[cn] = cv.join(";");
    });
    //取值完毕恢复disabled
    if (disabledCtrl) {
        $.each(disabledCtrl, function (i, item) {
            item.attr("disabled", true);
        });
    }
    return formData;
};
var GetFapChildGridData = function (formid) {
    var lblGrids = $("#" + formid).find("#lblchildgrid");
    if (lblGrids.length === 1) {
        var griddata = [], inlineValidate = true;
        var tables = lblGrids[0].innerText.trim();
        $.each(tables.split(','), function (i, table) {
            var rowData = $('#grid-' + table).jqGrid('getRowData', null, true);
            if (rowData === false) {
                inlineValidate = false;
                return false;
            }
            griddata.push({ "tn": table, "data": rowData });
        });
        //未校验通过
        if (!inlineValidate) {
            return false;
        }
        return griddata;
    }
    return null;
};
//持久化{success:true,data:obj}noPrompt--是否弹框
var Persistence = function (formid, tableName, beforeSaveCallback, afterSaveCallback) {
    if (!$("#" + formid).valid()) {
        $.msg($.lang("form_validation_failed",'表单校验失败')+":" + $(".error").html());
        return false;
    } else {
        //富文本编辑控件
        var richtext = $("#" + formid).find(".wysiwyg-editor");
        if (richtext !== '' && richtext.first().data("nullable") === 0 && richtext.first().html() === '') {
            $.msg($.lang("richtext_required","富文本内容不能为空！"));
            return false;
        }
        //校验附件是否上传
        var attachment = $("#" + formid).find(".form-control.attachment");
        if (attachment !== '' && attachment.first().data("nullable") === 0 && attachment.first().data("editable") === 1) {
            var fid = attachment.first().val();
            var result = false;
            $.ajax({
                type: "get",
                url: basePath + "/Component/ExistFile/" + fid,
                async: false,
                dataType: "text",
                success: function (rv) {
                    if (rv.success) {
                        result = true;
                    }
                }
            });
            if (!result) {
                $.msg($.lang("file_required","必须要上传附件！"));
                return false;
            }
        }
        var entityData = {};
        //主表单数据
        entityData.mainData = GetFapFormData(formid);
        entityData.tableName = tableName;
        //子表数据
        var childTableDatas = GetFapChildGridData(formid);
        if (childTableDatas === false) {
            //校验失败
            return false;
        }
        if (childTableDatas !== null) {
            entityData.childDataList = childTableDatas;
        }
        //干预赋值
        if (beforeSaveCallback !== undefined && $.isFunction(beforeSaveCallback)) {
            beforeSaveCallback(entityData.mainData);
        }
        //判断Id的值
        if (entityData.mainData["Id"] !== "") {
            entityData.oper = "edit";
        } else {
            entityData.oper = "add";
        }
        entityData.avoid_repeat_token = entityData.mainData["avoid_repeat_token"];
        var rv = { success: false };
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
            success: function (result) {
                rv = result;
                if (result.success === true) {
                    //赋值给ID 为了防止重复增加
                    if (rv.data) {
                        var resultData = rv.data;// JSON.parse(rv.data);

                        $("#" + formid + " .form-control#Id").val(resultData.Id);
                        $("#" + formid + " .form-control#Ts").val(resultData.Ts);
                        //保存成功后事件
                        if (afterSaveCallback !== undefined && $.isFunction(afterSaveCallback)) {
                            afterSaveCallback(resultData);
                        }
                    }
                }
                if (result.msg) {
                    $.msg(result.msg);
                }
            }
        });

        return rv;
    }
}
    ;
$(function () {
    //设置弹出参照框的显示头样式
    $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
        _title: function (title) {
            var $title = this.options.title || '&nbsp;';
            if (("title_html" in this.options) && this.options.title_html === true)
                title.html($title);
            else title.text($title);
        }
    }));
    //off掉注册事件，否则会每次增加
    $(document).off(ace.click_event, ".formctrl.deletefile");
    //设置
    $(document).on(ace.click_event, ".formctrl.deletefile", function (e) {
        //删除
        e.preventDefault();
        var fid = $(this).data("filefid");
        var df = $(this);
        bootbox.confirm($.lang("confirm_delete","确定删除此附件?"), function (result) {
            if (result) {
                //
                $.get(basePath + "/Component/DeleteFile/" + fid, function (rv) {
                    if (rv.success) {
                        df.parent().parent().remove();
                    }
                });
            }
        });
    });
    //off掉注册事件，否则会每次增加
    $(document).off(ace.click_event, ".formctrl.attached-file");
    //预览
    $(document).on(ace.click_event, ".formctrl.attached-file", function (e) {
        e.preventDefault();
        var fid = $(this).data("filefid");
        $.get(basePath + "/Component/PreviewFile/" + fid, function (data) {
            if (data.success) {
                if (data.id === 'img') {
                    openNewWindow(basePath + "/UploadFiles/View/" + data.msg);
                } else if (data.id === "0" || data.id === 0) {
                    $.msg($.msg("not_support_preview","无法预览"));
                } else {
                    openNewWindow(basePath + "/Content/Pdf/web/viewer.html?file=/UploadFiles/View/" + data.msg);
                }
            } else {
                $.msg(data.msg);
            }
        });
    });
});

//extra 参照form上控件的值 array
var loadRefMessageBox = function (title, frmid, colfid, ctrlid, refurl, extra) {
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-search "></i> ' + title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            success: {
                label: $.lang("ok", "确定"),
                className: "btn-primary",
                callback: function () {
                    var res = GetRefResult();
                    
                    if (res) {
                        $("#" + frmid + " #" + ctrlid + "MC").val(res.name).change();
                        $("#" + frmid + " #" + ctrlid).val(res.code).change();
                        //扩展返回值
                        if (res.frmcols !== undefined && res.frmcols !== '') {
                            var fcs = res.frmcols.split(',');
                            var exts = res.ext.split("^-^");
                            for (var i = 0; i < fcs.length; i++) {
                                debugger
                                $("#" + frmid + " #" + fcs[i]).val(exts[i]).change();
                                var reg = /<[^>]+>/gim;
                                if (exts[i].match(reg)) {
                                    $("#" + frmid + " #" + fcs[i]).html(exts[i]).change();
                                }
                            }
                        }

                    } else { $.msg($.lang("select_row","请选择一条数据！")); return; }
                }
            },
            danger: {
                label:$.lang("clear", "清空!"),
                className: "btn-sm btn-danger",
                callback: function () {
                    $("#" + frmid + " #" + ctrlid + "MC").val("").change();
                    $("#" + frmid + " #" + ctrlid).val("").change();
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
    });
    dialog.init(function () {
        var url = basePath + '/Component/' + refurl + '/' + colfid ;
        if (extra !== '') {
            var param = extra.join("&");
            url += "?" + param;
        }
        $.get(url, function (ev) {
            dialog.find('.bootbox-body').html(ev);

        });
    });

};
//文件上传窗口
var loadFileMessageBox = function (ctrlid, frmid, initFileEvent) {
    //加载前，需要获取参照对话框
    var dialog = bootbox.dialog({
        title: $.lang("annex","附件"),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            cancel: {
                label: $.lang("close","关闭"), className: "btn-default"
            }
        }

    });
    dialog.init(function () {
        dialog.find('.bootbox-body').html("<div class=\"row\">  <div  class=\"col-lg-12\"> <input type=\"file\" class=\"file-loading\"  id=\"" + frmid + ctrlid + "-FILE\" name=\"" + frmid + ctrlid + "-FILE\" multiple   /></div></div>");
        //初始化file控件
        initFileEvent();

        //这里写加载已有的附件列表
    });

};
//附件列表
var loadFileList = function (frmid, ctrlName, bid) {
    $.get(basePath + "/Component/AttachmentList/" + bid, function (rv) {
        let $fv = $("#frm-" + frmid + " #" + ctrlName + "_filelist");
        $fv.empty();
        var child= '<ul class="attachment-list pull-left  list-unstyled">';
        child += rv;
        child += '    </ul>';
        $fv.append(child);
    });

};
//加载图片头像
var loadImageControl = function (ctrlid) {
    try {//ie8 throws some harmless exceptions, so let's catch'em
        $.fn.editable.defaults.mode = 'inline';
        //first let's add a fake appendChild method for Image element for browsers that have a problem with this
        //because editable plugin calls appendChild, and it causes errors on IE at unpredicted points
        try {
            document.createElement('IMG').appendChild(document.createElement('B'));
        } catch (e) {
            Image.prototype.appendChild = function (el) { }
        }

        var last_gritter;
        $('#' + ctrlid).editable({
            type: 'image',
            name: 'avatar',
            value: null,
            image: {
                //specify ace file input plugin's options here
                btn_choose: $.lang("replace_photo", "更换照片"),
                droppable: true,
                maxSize: 11000000,//~100Kb

                //and a few extra ones here
                name: 'avatar',//put the field name here as well, will be used inside the custom plugin
                on_error: function (error_type) {//on_error function will be called when the selected file has a problem
                    if (last_gritter) $.gritter.remove(last_gritter);
                    if (error_type === 1) {//file format error
                        last_gritter = $.gritter.add({
                            title: $.lang("error_tip", "错误提示") + '!',
                            text: $.lang("please_select", "请选择") + 'jpg|gif|png image!',
                            class_name: 'gritter-error gritter-center'
                        });
                    } else if (error_type === 2) {//file size rror
                        last_gritter = $.gritter.add({
                            title: $.lang("error_tip", "错误提示") + '!',
                            text: $.lang("file_maxed", "文件大小不要超过") + '10M!',
                            class_name: 'gritter-error gritter-center'
                        });
                    }
                    else {//other error
                    }
                },
                on_success: function () {
                    $.gritter.removeAll();
                }
            },
            url: function (params) {
                // ***UPDATE AVATAR HERE*** //
                //for a working upload example you can replace the contents of this function with
                //examples/profile-avatar-update.js
                var submit_url = basePath + "/Component/UploadFile/";//please modify submit_url accordingly
                var deferred = null;
                var avatar = '#' + ctrlid;
                //if value is empty (""), it means no valid files were selected
                //but it may still be submitted by x-editable plugin
                //because "" (empty string) is different from previous non-empty value whatever it was
                //so we return just here to prevent problems
                var value = $(avatar).next().find('input[type=hidden]:eq(0)').val();
                if (!value || value.length === 0) {
                    deferred = new $.Deferred;
                    deferred.resolve();
                    return deferred.promise();
                }
                var $form = $(avatar).next().find('.editableform:eq(0)');
                var file_input = $form.find('input[type=file]:eq(0)');
                var pk = $(avatar).attr('data-pk');//primary key to be sent to server
                submit_url += pk;
                var ie_timeout = null;
                if ("FormData" in window) {
                    var formData_object = new FormData();//create empty FormData object

                    //serialize our form (which excludes file inputs)
                    $.each($form.serializeArray(), function (i, item) {
                        //add them one by one to our FormData
                        formData_object.append(item.name, item.value);
                    });
                    //and then add files
                    $form.find('input[type=file]').each(function () {
                        var field_name = $(this).attr('name');
                        var files = $(this).data('ace_input_files');
                        if (files && files.length > 0) {
                            formData_object.append(field_name, files[0]);
                        }
                    });

                    //append primary key to our formData
                    formData_object.append('pk', pk);

                    deferred = $.ajax({
                        url: submit_url,
                        type: 'POST',
                        processData: false,//important
                        contentType: false,//important
                        dataType: 'json',//server response type
                        data: formData_object
                    });
                }
                else {
                    deferred = new $.Deferred;

                    var temporary_iframe_id = 'temporary-iframe-' + (new Date()).getTime() + '-' + (parseInt(Math.random() * 1000));
                    var temp_iframe =
                        $('<iframe id="' + temporary_iframe_id + '" name="' + temporary_iframe_id + '" \
									frameborder="0" width="0" height="0" src="about:blank"\
									style="position:absolute; z-index:-1; visibility: hidden;"></iframe>')
                            .insertAfter($form);

                    $form.append('<input type="hidden" name="temporary-iframe-id" value="' + temporary_iframe_id + '" />');

                    //append primary key (pk) to our form
                    $('<input type="hidden" name="pk" />').val(pk).appendTo($form);

                    temp_iframe.data('deferrer', deferred);
                    //we save the deferred object to the iframe and in our server side response
                    //we use "temporary-iframe-id" to access iframe and its deferred object

                    $form.attr({
                        action: submit_url,
                        method: 'POST',
                        enctype: 'multipart/form-data',
                        target: temporary_iframe_id //important
                    });

                    $form.get(0).submit();
                    //if we don't receive any response after 30 seconds, declare it as failed!
                    ie_timeout = setTimeout(function () {
                        ie_timeout = null;
                        temp_iframe.attr('src', 'about:blank').remove();
                        deferred.reject({ 'status': 'fail', 'message': 'Timeout!' });
                    }, 30000);
                }
                //deferred callbacks, triggered by both ajax and iframe solution
                deferred
                    .done(function (result) {//success
                        //var res = result[0];//the `result` is formatted by your server side response and is arbitrary
                        if (result.success) {
                            var avatar = '#' + ctrlid;
                            var pk = $(avatar).attr('data-pk');
                            $(avatar).get(0).src = $.randomUrl(basePath + "/Component/Photo/" + pk);
                            if (last_gritter) $.gritter.remove(last_gritter);
                            last_gritter = $.gritter.add({
                                title: $.lang("photo_updated", "照片已更新") + '!',
                                text: '',
                                class_name: 'gritter-info gritter-center'
                            });
                        }
                        //else alert(res.message);
                    })
                    .fail(function (result) {//failure
                        alert($.lang("error", "遇到错误"));
                    })
                    .always(function () {//called on both success and failure
                        if (ie_timeout) clearTimeout(ie_timeout);
                        ie_timeout = null;
                    });

                return deferred.promise();
                // ***END OF UPDATE AVATAR HERE*** //

                //return deferred.promise();

                // ***END OF UPDATE AVATAR HERE*** //
            },

            success: function (response, newValue) {
                //$(avatar).get(0).src = "@Url.Content("~/Home/UserPhoto")";
            }
        });
    } catch (e) { throw e; }
};
