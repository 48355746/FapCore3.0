$(function () {
    $(".multilangpopoverclose").on(ace.click_event,
        function () {
            $(this).closest(".popover").css("display", "none");
        });
    $("#frm-grid-orgdept #Pid" + "MC").next().on(ace.click_event,
        function () {
            if ($(this).prev().attr("disabled") == "disabled") {
                return;
            }
            var extra = [];
            loadRefMessageBox('父部门', 'frm-grid-orgdept', '9cf511e5828780f23b44', 'Pid', 'TreeReference', extra)
        });
    $("#frm-grid-orgdept #Pid" + "MC").on(ace.click_event,
        function (e) {
            $(this).next().trigger(ace.click_event);
            e.preventDefault();
        }) $("#frm-grid-orgdept input[name='DeptOrder']").TouchSpin({

            min: -1000000000,
            max: 1000000000,
            step: 1,
            decimals: 0,
            boostat: 5,
            maxboostedstep: 10,

        });

    $("#frm-grid-orgdept #filegrid-orgdeptAttachment").on(ace.click_event,
        function () {
            loadFileMessageBox('666831603552485376', 'frm-grid-orgdept', initFilegrid - orgdept666831603552485376);
        }) var initFilegrid - orgdept666831603552485376 = function() {
            $("#frm-grid-orgdept666831603552485376-FILE").fileinput({
                language: 'zh',
                uploadUrl: "https://localhost:5001/Core/Api/uploadfile/666831580395732992",
                fileType: "any",
                uploadExtraData: {
                    fid: '666831580395732992'
                },
                allowedPreviewTypes: ['image', 'text'],

                showUpload: true,
                showCaption: false,
                overwriteInitial: false,
                slugCallback: function (filename) {
                    return filename.replace('(', '_').replace(']', '_');
                },
                browseClass: "btn btn-primary",
                previewFileIcon: "<i class='glyphicon glyphicon-king'></i>"
            }).on('fileloaded',
                function (event, file, previewId, index, reader) {
                    //$(this).fileinput('upload');
                    // alert(index);
                    var files = $(this).fileinput('getFileStack');
                    $(this).fileinput('uploadSingle', index, files, false);
                }).on('fileuploaded',
                    function (event, data, previewId, index) {
                        if (data.response.success == false) {
                            bootbox.alert('上传失败：' + data.response.msg);
                        } else {
                            loadFileList('grid-orgdept', 'Attachment', '666831580395732992');
                        }
                    });
        }
    $("#frm-grid-orgdept #DeptManager" + "MC").next().on(ace.click_event,
        function () {
            if ($(this).prev().attr("disabled") == "disabled") {
                return;
            }
            var extra = [];
            loadRefMessageBox('部门经理', 'frm-grid-orgdept', 'bf8d11e58287516ab924', 'DeptManager', 'GridReference', extra)
        });
    $("#frm-grid-orgdept #DeptManager" + "MC").on(ace.click_event,
        function (e) {
            $(this).next().trigger(ace.click_event);
            e.preventDefault();
        }) $("#frm-grid-orgdept #Director" + "MC").next().on(ace.click_event,
            function () {
                if ($(this).prev().attr("disabled") == "disabled") {
                    return;
                }
                var extra = [];
                loadRefMessageBox('负责人', 'frm-grid-orgdept', 'eb6d11e59793a7b8371a', 'Director', 'GridReference', extra)
            });
    $("#frm-grid-orgdept #Director" + "MC").on(ace.click_event,
        function (e) {
            $(this).next().trigger(ace.click_event);
            e.preventDefault();
        }) $("#frm-grid-orgdept input[name='PreparationNums']").TouchSpin({

            min: -1000000000,
            max: 1000000000,
            step: 1,
            decimals: 0,
            boostat: 5,
            maxboostedstep: 10,

        });

    $('textarea.limited').inputlimiter({
        remText: '%n 字符剩余...',
        limitText: '最大字符数 : %n.'
    });
});