var $annexBtn = $("<button  class=\"btn btn-info\" type=\"button\"><i class=\"ace-icon fa fa-paperclip\"></i>模板</button>");
$("#XlsFile").addClass("hide").parent().append($annexBtn);

$annexBtn.on(ace.click_event, function () {
    var title = "<h3>Excel报表模板说明书!!!</h3>"+
				"<p><a href='"+ basePath +"/Templates/FapReport_Tutorial.docx'><i class=\"ace-icon fa fa-download green\"></i>下载说明书</a></p>";
    var dialog = bootbox.dialog({
        title: '<i class="ace-icon fa fa-file-excel-o"></i> ' + $.lang("upload_template", "上传模板"),
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        buttons: {
            cancel: {
                label: $.lang("close", "关闭"), className: "btn-default"
            }
        }

    });

    dialog.init(function () {        
        var $file = $("<input id=\"file-import\" type=\"file\"  class=\"file-loading\"/>");
        dialog.find('.bootbox-body').empty().append(title).append($file);
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


