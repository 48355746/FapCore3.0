$(document).on(ace.click_event,".gritter-image", function () {
    var empphoto = $(this).data("image");
    var modal =
                        '<div class="modal fade">\
					  <div class="modal-dialog">\
					   <div class="modal-content">\
						<div class="modal-header">\
							<button type="button" class="close" data-dismiss="modal">&times;</button>\
							<h4 class="blue">改变头像</h4>\
						</div>\
						\
						<form class="no-margin">\
						 <div class="modal-body">\
							<div class="space-4"></div>\
							<div style="width:75%;margin-left:12%;"><input type="file" name="file-input" /></div>\
						 </div>\
						\
						 <div class="modal-footer center">\
							<button type="submit" class="btn btn-sm btn-success"><i class="ace-icon fa fa-check"></i> 提交</button>\
							<button type="button" class="btn btn-sm" data-dismiss="modal"><i class="ace-icon fa fa-times"></i> 取消</button>\
						 </div>\
						</form>\
					  </div>\
					 </div>\
					</div>';


    var modal = $(modal);
    modal.modal("show").on("hidden", function () {
        modal.remove();
    });
    var working = false;
    var form = modal.find('form:eq(0)');
    var file = form.find('input[type=file]').eq(0);
    file.ace_file_input({
        style: 'well',
        btn_choose: '点击选择新头像',
        btn_change: null,
        no_icon: 'ace-icon fa fa-picture-o',
        thumbnail: 'small',
        before_remove: function () {
            //don't remove/reset files while being uploaded
            return !working;
        },
        allowExt: ['jpg', 'jpeg', 'png', 'gif'],
        allowMime: ['image/jpg', 'image/jpeg', 'image/png', 'image/gif']
    });

   
    form.on('submit', function (e) {
        e.preventDefault();
        working = true;
        var files = file.data('ace_input_files');
        if (!files || files.length == 0) return false;//no files selected
        file.ace_file_input('disable');
        form.find('button').attr('disabled', 'disabled');
        form.find('.modal-body').append("<div class='center'><i class='ace-icon fa fa-spinner fa-spin bigger-150 orange'></i></div>");

        var deferred;
        if ("FormData" in window) {
            //for modern browsers that support FormData and uploading files via ajax
            //we can do >>> var formData_object = new FormData($form[0]);
            //but IE10 has a problem with that and throws an exception
            //and also browser adds and uploads all selected files, not the filtered ones.
            //and drag&dropped files won't be uploaded as well

            //so we change it to the following to upload only our filtered files
            //and to bypass IE10's error
            //and to include drag&dropped files as well
            formData_object = new FormData();//create empty FormData object

            //serialize our form (which excludes file inputs)
            $.each(form.serializeArray(), function (i, item) {
                //add them one by one to our FormData 
                formData_object.append(item.name, item.value);
            });
            //and then add files
            form.find('input[type=file]').each(function () {
                var field_name = $(this).attr('name');
                //for fields with "multiple" file support, field name should be something like `myfile[]`

                var files = $(this).data('ace_input_files');
                if (files && files.length > 0) {
                    for (var f = 0; f < files.length; f++) {
                        formData_object.append(field_name, files[f]);
                    }
                }
            });


            upload_in_progress = true;
            file.ace_file_input('loading', true);

            deferred = $.ajax({
                url: basePath + "/Api/Core/uploadfile/" + empphoto,
                type: "POST",
                processData: false,//important
                contentType: false,//important
                dataType: 'json',
                data: formData_object
                
                ,
                xhr: function() {
                    var req = $.ajaxSettings.xhr();
                    if (req && req.upload) {
                        req.upload.addEventListener('progress', function(e) {
                            if(e.lengthComputable) {	
                                var done = e.loaded || e.position, total = e.total || e.totalSize;
                                var percent = parseInt((done/total)*100) + '%';
                                //percentage of uploaded file
                            }
                        }, false);
                    }
                    return req;
                },
                beforeSend : function() {
                },
                success: function () {
                    form.find('button').removeAttr('disabled');
                    form.find('input[type=file]').ace_file_input('enable');
                    form.find('.modal-body > :last-child').remove();

                    modal.modal("hide");
                    working = false;
                }
            })
        }
        else {
            bootbox.alert("请使用新版本浏览器");
        }
    });
});
