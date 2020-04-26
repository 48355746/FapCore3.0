$("#TableNameMC").on("change", function () {    
    initTemplate();
});
setTimeout(function () { initTemplate(); }, 0);

function initTemplate() {
    var tn = $("#TableName").val();
    if (tn === '') {
        return;
    }
    var furl = basePath + '/Core/Api/FieldList/' + tn;
    $.get(furl, function (data) {
        var filterData = $.grep(data, function (d, i) {
            return d.isDefaultCol === 0 && d.colProperty !== 3;
        });
        //加入常量
        //filterData.unshift({ colComment: '处理人常量' });
        //filterData.unshift({ colComment: '处理结果常量' });
        //filterData.unshift({ colComment: '处理Url常量' });
        filterData.unshift({ colComment: '业务名称' });
        filterData.unshift({ colComment: '业务申请人' });
        filterData.unshift({ colComment: '业务处理人' });
        filterData.unshift({ colComment: '业务申请时间' });
        filterData.unshift({ colComment: '业务处理时间' });
        filterData.unshift({ colComment: '业务流程状态' });
        filterData.unshift({ colComment: '业务审批结论' });
        filterData.unshift({ colComment: '业务审批意见' });
        filterData.unshift({ colComment: '单据编码' });
        $(".wysiwyg-toolbar").remove();
        $('.wysiwyg-editor').ace_wysiwyg({
            toolbar:
                [
                    {
                        name: 'font',
                        title: '字体',
                        values: ['Some Font!', 'Microsoft YaHei', 'Arial', 'Verdana', 'Comic Sans MS', 'Custom Font!']
                    },
                    null,
                    {
                        name: 'fontSize',
                        title: '大小',
                        values: { 1: 'Size#1 Text', 2: 'Size#1 Text', 3: 'Size#3 Text', 4: 'Size#4 Text', 5: 'Size#5 Text' }
                    },
                    null,
                    { name: 'bold', title: '加粗' },
                    { name: 'italic', title: '斜体' },
                    { name: 'strikethrough', title: '删除线' },
                    { name: 'underline', title: '下划线' },
                    null,
                    'insertunorderedlist',
                    'insertorderedlist',
                    'outdent',
                    'indent',
                    null,
                    { name: 'justifyleft' },
                    { name: 'justifycenter' },
                    { name: 'justifyright' },
                    { name: 'justifyfull' },
                    null,
                    {
                        name: 'createLink',
                        placeholder: 'url地址',
                        button_class: 'btn-purple',
                        button_text: '添加链接'
                    },
                    { name: 'unlink' },
                    null,
                    {
                        name: 'insertImage',
                        title: '插入图片',
                        placeholder: 'url地址',
                        button_class: 'btn-primary',
                        //choose_file:false,//hide choose file button
                        button_text: '选择本地',// 'Set choose_file:false to hide this',
                        button_insert_class: 'btn-pink',
                        button_insert: '插入图片'
                    },
                    null,
                    {
                        name: 'foreColor',
                        title: '前景色',
                        values: ['red', 'green', 'blue', 'navy', 'orange'],
                        /**
                            You change colors as well
                        */
                    },
                    null,
                    {
                        name: 'backColor',
                        title: '背景色'
                    },
                    null,
                    { name: 'undo' },
                    { name: 'redo' },
                    null,
                    'viewSource',
                    null,
                    { name: 'insertText', values: filterData }

                ],
            //speech_button:false,//hide speech button on chrome

            'wysiwyg': {
                hotKeys: {} //disable hotkeys
            }
        }).prev().addClass('wysiwyg-style1');
    });
}