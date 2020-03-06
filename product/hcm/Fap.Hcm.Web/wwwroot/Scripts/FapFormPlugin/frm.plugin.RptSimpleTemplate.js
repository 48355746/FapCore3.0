var $annexBtn = $("<button  class=\"btn btn-info\" type=\"button\"><i class=\"ace-icon fa fa-paperclip\"></i>模板</button>");
$("#XlsFile").addClass("hide").parent().append($annexBtn);

$annexBtn.on(ace.click_event, function () {
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
        var $file = $("<input id=\"file-import\" type=\"file\"  class=\"file-loading\"/>");
        dialog.find('.bootbox-body').empty().append($file);
        $file.fileinput({
            language: language,
            uploadUrl: basePath + '/Api/Core/ImportExcelReportTemplate/' + $("#XlsFile").val(),
            showCaption: false,
            allowedFileExtensions: ["xlsx"],
            maxFileCount: 1,
            showClose: true
        });
    });
});


