function selectQuestion(e) {
    var i = '<div class="topic-type-content topic-type-question after-clear">',
    t = parseInt(absolute_id) + 1;
    switch (absolute_id++, choice_absolute_id[t] || (choice_absolute_id[t] = 0), e) {
        case "radio":
            choice_absolute_id[t]++;
            var s = choice_absolute_id[t];
            i += '<div class="question-title" type="6" name="radio-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N" choice-quote="0">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true" content-default="1">单选题</div></div></div><ul class="question-choice"><li class="choice" has_other="N" choice_absolute_id=' + s + ' ><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项1 </div></div><div class="option-tips"></div></li>',
            choice_absolute_id[t]++,
            s = choice_absolute_id[t],
            i += '<li class="choice" has_other="N" choice_absolute_id=' + s + '><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项2 </div></div><div class="option-tips"></div></li></ul>',
            i += '<div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            i += '<div class="operate visible-hide" ><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "checkbox":
            choice_absolute_id[t]++;
            var s = choice_absolute_id[t];
            i += '<div class="question-title" type="8" name="checkbox-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N" choice-quote="0" min="" max="" exclusive-options="">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true" content-default="1">多选题</div></div></div><ul class="question-choice"><li class="choice" has_other="N" choice_absolute_id=' + s + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项1 </div></div><div class="option-tips"></div></li>',
            choice_absolute_id[t]++,
            s = choice_absolute_id[t],
            i += '<li class="choice" has_other="N" choice_absolute_id=' + s + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项2 </div></div><div class="option-tips"></div></li></ul>',
            i += '<div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            i += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "input":
            i += '<div class="question-title" type="1" name="input-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true" content-default="1">单行填空题</div></div></div><ul class="question-choice"><li><input type="text" name="input" class="input-single"></li></ul>',
            i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "multi-input":
            i += '<div class="question-title" type="2" name="multi-input-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true" content-default="1">多行填空题</div></div></div><ul class="question-choice"><li class="auto-height"><textarea name="multi-input" class="multi-input"></textarea></li></ul>',
            i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "radio-matrix":
            i += '<div class="question-title" type="9" name="radio-matrix-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true" content-default="1">矩阵单选题</div></div></div><ul class="question-choice" style="float:left;width: 700px;"><li class="auto-height"><table style="border-collapse: collapse;"><tbody><tr><td>&nbsp</td><td name=radio-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项1</li></div></td><td name=radio-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项2</li></div></td></tr><tr><td class="radio_array_title" name="radio-matrix" ><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行1</div></div></td><td><input type="radio"/></td><td><input type="radio"/></td></tr><tr><td class="radio_array_title" name="radio-matrix"><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行2</div></div></td><td><input type="radio"/></td><td><input type="radio"/></td></tr></tbody></table></li></ul>',
            i += '<div class="add-area visible-hide" style="width: 34px; margin: 0px 0 0 46px;" choice="Y"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" style="margin-top: 14px;" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            i += '<div class="add-area visible-hide" choice="N"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            //i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "checkbox-matrix":
            i += '<div class="question-title" type="13" name="checkbox-matrix-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true" content-default="1">矩阵多选题</div></div></div><ul class="question-choice" style="float:left;width: 700px;"><li class="auto-height"><table style="border-collapse: collapse;"><tbody><tr><td>&nbsp</td><td name=checkbox-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项1</li></div></td><td name=checkbox-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项2</li></div></td></tr><tr><td class="checkbox_array_title" name="checkbox-matrix" ><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行1</div></div></td><td><input type="checkbox"/></td><td><input type="checkbox"/></td></tr><tr><td class="checkbox_array_title" name="checkbox-matrix"><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行2</div></div></td><td><input type="checkbox"/></td><td><input type="checkbox"/></td></tr></tbody></table></li></ul>',
            i += '<div class="add-area visible-hide" style="width: 34px; margin: 0px 0 0 46px;" choice="Y"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" style="margin-top: 14px;" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            i += '<div class="add-area visible-hide" choice="N"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            //i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            i += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "desc":
            i = '<div class="topic-type-content topic-type-question after-clear"><div class="question-title" type="10" name="description"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="N" has_other="N"></span><div class="position-relative"><div class="qs-content qs-high-content edit-area edit-title" contenteditable="true" content-default="1">描述说明</div></div></div>',
            i += '<div class="operate visible-hide" style="width: 141px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "page":
            i = '<div class="topic-type-content topic-page after-clear"> <div class="question-title" type="11" order="1" name="page" index="0" question-required="Y" has_other="N" title-quote="N" style="padding: 0px;height: 36px;"><span class="question-id" order="1" page="1" style="margin: 0px;"></span><div class="page-area">页码</div></div>',
            i += '<div class="operate visible-hide" style="width:94px;"><ul><li class="drag-area" title="移动"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "select":
            var s;
            i += '<div class="question-title" type="7" name="select-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N" choice-quote="0">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">选择列表</div></div></div><select class="question-choice" style="  padding: 0;margin: 15px 0 20px 35px;">';
            for (var a = 1; 4 > a; a++) choice_absolute_id[t]++,
            s = choice_absolute_id[t],
            i += '<option class="choice" has_other="N" choice_absolute_id=' + s + ">选项" + a + "</option>";
            i += '</select><span class="edit-select" style="  width: 60%;display: inline-block;line-height: 30px;" onclick="edit.editSelect(this)">编辑选项</span>',
            i += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "upload":
            i += '<div class="question-title" type="12" name="upload-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true" content-default="1">上传图片</div></div></div><ul class="question-choice"><li>  <div class=survey-question-upload-btn> <div class=survey-question-upload-inner> <img src=/Content/Survey/images/upload-icon.png> <div class=survey-question-upload-inner-desc>选择图片20M以内 </div> </div> <input type=file class=survey-question-upload-file multiple> </div></li></ul>',
            i += '<div class="operate visible-hide" style="width: 190px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "radio-img":
            choice_absolute_id[t]++;
            var s = choice_absolute_id[t];
            i += '<div class="question-title" type="14" name="radio-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N" choice-quote="0">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true" content-default="1">图片单选题</div></div></div>',
            i += '<ul class="question-choice"></ul>',
            i += '<div class="add-area survey-question-upload-img-wrap"><div class=survey-question-upload-img-btn> <div class=survey-question-upload-img-inner> <img src=/Content/Survey/images/upload-img-icon.png> <div class=survey-question-upload-img-inner-title>点击上传图片</div> <div class=survey-question-upload-img-inner-desc>最多可上传50张图片</div> </div> <input type=file class=survey-question-upload-img-file name=upload_img multiple> </div></div>',
            i += '<div class="operate visible-hide" ><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "checkbox-img":
            choice_absolute_id[t]++;
            var s = choice_absolute_id[t];
            i += '<div class="question-title" type="15" name="checkbox-question"><span class="question-id" order="1" page="1" index="1" absolute_id=' + t + ' question-required="Y" has_other="N" title-quote="N" choice-quote="0" min="" max="">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true" content-default="1">图片多选题</div></div></div>',
            i += '<ul class="question-choice"></ul>',
            i += '<div class="add-area survey-question-upload-img-wrap"><div class=survey-question-upload-img-btn> <div class=survey-question-upload-img-inner> <img src=/Content/Survey/images/upload-img-icon.png> <div class=survey-question-upload-img-inner-title>点击上传图片</div> <div class=survey-question-upload-img-inner-desc>最多可上传50张图片</div> </div> <input type=file class=survey-question-upload-img-file name=upload_img multiple> </div></div>',
            i += '<div class="operate visible-hide" ><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>'
    }
    i += "</div>",
    $("#question-box").append(i),
    edit.sortQuestions(),
    edit.attachLayer(),
    edit.updateChoiceShowLogic(),
    edit.scrollBox(),
    edit.visibleHandle(),
    elementInit()
}
function selectEditedQuestion(e) {
    var i = e.question_content,
    t = i.choice,
    s = "";
    s += '<div class="topic-type-content topic-type-question after-clear">';
    var a;
    switch (i.absolute_id ? a = i.absolute_id : (a = parseInt(absolute_id) + 1, absolute_id++), i.type_id) {
        case "6":
            choice_absolute_id[a] = 0;
            var l;
            s += '<div class="question-title" type="6" name="radio-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" choice-quote="' + i.choice_quote + '" absolute_id=' + a + '  question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice">';
            for (var o in t) t[o].required = "undefined" == typeof t[o].required ? 0 : parseInt(t[o].required, 10),
            t[o].choice_absolute_id ? (l = parseInt(t[o].choice_absolute_id, 10), l > parseInt(choice_absolute_id[a], 10) && (choice_absolute_id[a] = l)) : (l = parseInt(choice_absolute_id[a], 10) + 1, choice_absolute_id[a]++),
            s += '<li class="choice" has_other="' + t[o].is_other + '" choice_required="' + t[o].required + '" choice_absolute_id=' + l + ' ><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true">' + t[o].content + "</div></div>",
            "Y" === t[o].is_other && (s += '<input type="text" class="other-content"  style="width: 120px;height: 30px;vertical-align: middle;" >', 1 === parseInt(t[o].required, 10) && (s += '<span class="required-content">(必填)</span>')),
            s += '<div class="option-tips"></div></li>';
            s += '</ul><div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            s += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "14":
            choice_absolute_id[a] = 0;
            var l;
            s += '<div class="question-title" type="14" name="radio-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" choice-quote="' + i.choice_quote + '" absolute_id=' + a + '  question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice">';
            for (var o in t) t[o].required = "undefined" == typeof t[o].required ? 0 : parseInt(t[o].required, 10),
            t[o].contentArr = t[o].content.split("#**#"),
            t[o].choice_absolute_id ? (l = parseInt(t[o].choice_absolute_id, 10), l > parseInt(choice_absolute_id[a], 10) && (choice_absolute_id[a] = l)) : (l = parseInt(choice_absolute_id[a], 10) + 1, choice_absolute_id[a]++),
            s += '<li class="choice survey-question-radio-img" has_other="' + t[o].is_other + '" choice_required="' + t[o].required + '" choice_absolute_id=' + l + ' > <div class=survey-question-radio-choice> <a class="remove-child-element survey-question-radio-img-remove" onclick="edit.removeChildElement()"></a><div class=survey-question-radio-choice-img> <img src="' + t[o].contentArr[1] + '"> </div> <div class=survey-question-radio-choice-text> <input type=radio name=radio> <label class="edit-area edit-child-element" contenteditable=true>' + t[o].contentArr[0] + '</label> <div class="option-tips"></div></div> </div>',
            s += "</li>";
            s += "</ul>",
            s += '<div class="add-area survey-question-upload-img-wrap"><div class=survey-question-upload-img-btn> <div class=survey-question-upload-img-inner> <img src=/Content/Survey/images/upload-img-icon.png> <div class=survey-question-upload-img-inner-title>点击上传图片</div> <div class=survey-question-upload-img-inner-desc>最多可上传50张图片</div> </div> <input type=file class=survey-question-upload-img-file name=upload_img multiple> </div></div>',
            s += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "15":
            choice_absolute_id[a] = 0;
            var l;
            s += '<div class="question-title" type="15" name="checkbox-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" choice-quote="' + i.choice_quote + '" absolute_id=' + a + '  question-required="' + i.required + '" has_other="' + i.has_other + '" min="' + i.min + '" max="' + i.max + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice">';
            for (var o in t) t[o].required = "undefined" == typeof t[o].required ? 0 : parseInt(t[o].required, 10),
            t[o].contentArr = t[o].content.split("#**#"),
            t[o].choice_absolute_id ? (l = parseInt(t[o].choice_absolute_id, 10), l > parseInt(choice_absolute_id[a], 10) && (choice_absolute_id[a] = l)) : (l = parseInt(choice_absolute_id[a], 10) + 1, choice_absolute_id[a]++),
            s += '<li class="choice survey-question-checkbox-img" has_other="' + t[o].is_other + '" choice_required="' + t[o].required + '" choice_absolute_id=' + l + ' > <div class=survey-question-checkbox-choice> <a class="remove-child-element survey-question-checkbox-img-remove" onclick="edit.removeChildElement()"></a><div class=survey-question-checkbox-choice-img> <img src="' + t[o].contentArr[1] + '"> </div> <div class=survey-question-checkbox-choice-text> <input type=checkbox name=checkbox> <label class="edit-area edit-child-element" contenteditable=true>' + t[o].contentArr[0] + '</label> <div class="option-tips"></div></div> </div>',
            s += "</li>";
            s += "</ul>",
            s += '<div class="add-area survey-question-upload-img-wrap"><div class=survey-question-upload-img-btn> <div class=survey-question-upload-img-inner> <img src=/Content/Survey/images/upload-img-icon.png> <div class=survey-question-upload-img-inner-title>点击上传图片</div> <div class=survey-question-upload-img-inner-desc>最多可上传50张图片</div> </div> <input type=file class=survey-question-upload-img-file name=upload_img multiple> </div></div>',
            s += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "8":
            choice_absolute_id[a] = 0;
            var l;
            s += '<div class="question-title" type="8" name="checkbox-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" choice-quote="' + i.choice_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '" min="' + i.min + '" max="' + i.max + '" exclusive-options="' + (i.exclusive_options ? i.exclusive_options : "") + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">' + i.content + ' </div></div></div><ul class="question-choice">';
            for (var o in t) t[o].required = "undefined" == typeof t[o].required ? 0 : parseInt(t[o].required, 10),
            t[o].choice_absolute_id ? (l = parseInt(t[o].choice_absolute_id, 10), l > parseInt(choice_absolute_id[a], 10) && (choice_absolute_id[a] = l)) : (l = parseInt(choice_absolute_id[a], 10) + 1, choice_absolute_id[a]++),
            s += '<li class="choice" has_other="' + t[o].is_other + '" choice_required="' + t[o].required + '" choice_absolute_id=' + l + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true">' + t[o].content + "</div></div>",
            "Y" === t[o].is_other && (s += '<input type="text" class="other-content"  style="width: 120px;height: 30px;vertical-align: middle;" >', 1 === parseInt(t[o].required, 10) && (s += '<span class="required-content">(必填)</span>')),
            s += '<div class="option-tips"></div></li>';
            s += '</ul><div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            s += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "1":
            s += '<div class="question-title" type="1" name="input-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true">' + i.content + ' </div></div></div><ul class="question-choice"><li><input type="text" name="input" class="input-single"></li></ul>',
            s += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "2":
            s += '<div class="question-title" type="2" name="multi-input-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice"><li class="auto-height"><textarea name="multi-input" class="multi-input"></textarea></li></ul>',
            s += '<div class="operate visible-hide" style="width: 240px;" ><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "9":
            s += '<div class="question-title" type="9" name="radio-matrix-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice" style="float:left;width: 700px;"><li class="auto-height"><table style="border-collapse: collapse;"><tbody><tr><td>&nbsp</td>';
            for (var c in t) t[c].required = "undefined" == typeof t[c].required ? 0 : parseInt(t[c].required, 10),
            s += '<td name=radio-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" contenteditable="true" has_other="' + t[c].is_other + '" choice_required="' + t[c].required + '">' + t[c].content + "</li></div></td>";
            s += "</tr>";
            var n = i.radio_array_title;
            for (var d in n) {
                s += '<tr><td class="radio_array_title" name="radio-matrix"><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true">' + n[d].content + "</div></div></td>";
                for (var r = 0; r < t.length; r++) s += '<td><input type="radio"/>',
                "Y" == t[r].is_other && (s += '<input type="text" class="other-content" style="width: 120px;height: 30px;vertical-align: middle;" >', 1 === parseInt(t[r].required, 10) && (s += '<span class="required-content">(必填)</span>')),
                s += "</td>";
                s += "</tr>"
            }
            s += '</tbody></table></li></ul><div class="add-area visible-hide" style="width: 34px; margin: 0px 0 0 46px;" choice="Y"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" style="margin-top: 14px;" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            s += '<div class="add-area visible-hide" choice="N"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            //s += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            s += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "13":
            s += '<div class="question-title" type="13" name="checkbox-matrix-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true">' + i.content + '</div></div></div><ul class="question-choice" style="float:left;width: 700px;"><li class="auto-height"><table style="border-collapse: collapse;"><tbody><tr><td>&nbsp</td>';
            for (var c in t) t[c].required = "undefined" == typeof t[c].required ? 0 : parseInt(t[c].required, 10),
            s += '<td name=checkbox-matrix-choice><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" contenteditable="true" has_other="' + t[c].is_other + '" choice_required="' + t[c].required + '">' + t[c].content + "</li></div></td>";
            s += "</tr>";
            var u = i.checkbox_array_title;
            for (var d in u) {
                s += '<tr><td class="checkbox_array_title" name="checkbox-matrix"><div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true">' + u[d].content + "</div></div></td>";
                for (var r = 0; r < t.length; r++) s += '<td><input type="checkbox"/>',
                "Y" == t[r].is_other && (s += '<input type="text" class="other-content" style="width: 120px;height: 30px;vertical-align: middle;" >', 1 === parseInt(t[r].required, 10) && (s += '<span class="required-content">(必填)</span>')),
                s += "</td>";
                s += "</tr>"
            }
            s += '</tbody></table></li></ul><div class="add-area visible-hide" style="width: 34px; margin: 0px 0 0 46px;" choice="Y"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" style="margin-top: 14px;" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            s += '<div class="add-area visible-hide" choice="N"><ul><li class="add-choice" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" onclick="edit.batchAddChoice(this)"></li></ul></div>',
            //s += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            s += '<div class="operate visible-hide" style="width: 240px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "10":
            s = '<div class="topic-type-content topic-type-question after-clear"> <div class="question-title" type="10" name="description"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '"></span><div class="position-relative"><div class="qs-content qs-high-content edit-area edit-title" contenteditable="true">' + i.content + "</div></div></div>",
            s += '<div class="operate visible-hide" style="width: 141px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "11":
            s = '<div class="topic-type-content topic-page after-clear"> <div class="question-title" type="11" order="1" name="page" index="0" title-quote="' + i.title_quote + '" question-required="' + i.required + '" has_other="' + i.has_other + '" style="padding: 0px;height: 36px;"><span class="question-id" order="1" page="1" style="margin: 0px;"></span><div class="page-area">页码</div></div>',
            s += '<div class="operate visible-hide" style="width:94px;"><ul><li class="drag-area" title="移动"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "7":
            choice_absolute_id[a] = 0;
            var l;
            s += '<div class="question-title" type="7" name="radio-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" choice-quote="' + i.choice_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" tabindex="0" contenteditable="true">' + i.content + '</div></div></div><select class="question-choice" style="  padding: 0;margin: 15px 0 20px 35px;">';
            for (var o in t) t[o].choice_absolute_id ? (l = parseInt(t[o].choice_absolute_id, 10), l > parseInt(choice_absolute_id[a], 10) && (choice_absolute_id[a] = l)) : (l = parseInt(choice_absolute_id[a], 10) + 1, choice_absolute_id[a]++),
            s += '<option class="choice" has_other="' + t[o].is_other + '" choice_absolute_id=' + l + ">" + t[o].content + "</option>";
            s += '</select><span class="edit-select" style="  width: 60%;display: inline-block;line-height: 30px;" onclick="edit.editSelect(this)">编辑选项</span>',
            s += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
            break;
        case "12":
            s += '<div class="question-title" type="12" name="upload-question"><span class="question-id" order="1" page="1" index="1" title-quote="' + i.title_quote + '" absolute_id=' + a + ' question-required="' + i.required + '" has_other="' + i.has_other + '">1</span><div class="position-relative"><div class="qs-content edit-area edit-title" contenteditable="true">' + i.content + ' </div></div></div><ul class="question-choice"><li>  <div class=survey-question-upload-btn> <div class=survey-question-upload-inner> <img src=/Content/Survey/images/upload-icon.png> <div class=survey-question-upload-inner-desc>选择图片20M以内 </div> </div> <input type=file class=survey-question-upload-file multiple> </div></li></ul>',
            s += '<div class="operate visible-hide" style="width: 190px;"><ul><li class="drag-area" title="移动"></li><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>'
    }
    s += "</div>",
    $("#question-box").append(s),
    edit.sortQuestions(),
    edit.attachLayer(),
    edit.updateChoiceShowLogic(),
    edit.updateByChoiceQuote(),
    edit.showExclusiveOptions(),
    edit.visibleHandle()
}
function selectQuestionGroup(e) {
    e.siblings("ul").is(":visible") ? (e.css({
        background: "url(/Content/Survey/images/side_bar_hide.png) no-repeat",
        "border-bottom": "none"
    }), e.siblings("ul").hide()) : (e.css({
        background: "url(/Content/Survey/images/side_bar_show.png) no-repeat",
        "border-bottom": "1px solid #dbdbdb"
    }), e.parent().siblings("div").find(".classify-title").css({
        background: "url(/Content/Survey/images/side_bar_hide.png) no-repeat",
        "border-bottom": "none"
    }), e.siblings("ul").show(), e.parent().siblings("div").find("ul").hide())
}
function getRelation() {
    var e = {};
    return angular.forEach(redirect_relation,
    function (i, t) {
        var s = !1;
        i && angular.forEach(i,
        function (e) {
            e && (s = !0)
        }),
        s && (e[t] = i)
    }),
    e
}
function ImgEditSize(e) {
    return this.main = function (e) {
        this.obj = $(e),
        this.event()
    },
    this.status = !0,
    this.status_menu = !0,
    this.event = function () {
        var e = this;
        this.obj.mouseover(function () {
            e.status_menu = !1,
            e.Menu && e.DelMenu(),
            e.addMenu()
        }),
        this.obj.mouseout(function () {
            setTimeout(function () {
                e.DelMenu()
            },
            50)
        })
    },
    this.addMenu = function () {
        var e = this;
        this.Menu = $('<div class="img-edit-size"><ul><li class="enl" title="放大"></li><li class="ori" title="原图"></li><li class="nar" title="缩小"></li><li class="del" title="删除"></li></ul></div>'),
        $("body").append(e.Menu),
        this.Menu.css({
            position: "absolute",
            top: e.obj.offset().top + 5 + "px",
            left: e.obj.offset().left + 5 + "px"
        }),
        this.Menu.hover(function () {
            e.status = !1
        },
        function () {
            e.status = !0,
            e.status_menu = !0,
            setTimeout(function () {
                e.status_menu && e.DelMenu()
            },
            200)
        }),
        this.Menu.find(".enl").click(function () {
            var i = e.obj.width(),
            t = e.obj.height(),
            s = i / t;
            return i >= 580 ? !1 : (e.getImgWH(i + 20 * s, t + 20), !1)
        }),
        this.Menu.find(".ori").click(function () {
            e.imgSize(e.obj.attr("src"));
            return !1
        }),
        this.Menu.find(".nar").click(function () {
            var i = e.obj.width(),
            t = e.obj.height(),
            s = i / t;
            return 100 >= i ? !1 : (e.getImgWH(i - 20 * s, t - 20), !1)
        }),
        this.Menu.find(".del").click(function () {
            return e.obj.remove(),
            e.status = !0,
            e.DelMenu(),
            !1
        })
    },
    this.getImgWH = function (e, i) {
        this.obj.width(e),
        this.obj.height(i)
    },
    this.DelMenu = function () {
        this.status && this.Menu.remove()
    },
    this.imgSize = function (e) {
        var i = this,
        t = new Image;
        t.onload = function () {
            t.width > 620 && (t.width = i.obj.attr("width"), t.height = i.obj.attr("height")),
            i.getImgWH(t.width, t.height)
        },
        t.src = e
    },
    this.main(e)
}
function selectFinish() {
    $(".select-ul").each(function () {
        if ($(this).siblings(".add-area").length > 0) {
            var e = '<select class="question-choice" style="  padding: 0;margin: 15px 0 20px 35px;">';
            $(this).find(".choice").each(function () {
                e += '<option class="choice" has_other="N" choice_absolute_id=' + $(this).attr("choice_absolute_id") + ">" + $(this).find(".edit-area").html() + "</option>"
            }),
            e += '</select><span class="edit-select" onclick="edit.editSelect(this)" style="  width: 60%;display: inline-block;line-height: 30px;">编辑选项</span>',
            $(this).siblings(".question-title").after(e),
            $(this).siblings(".question-title").attr("type", "7"),
            $(this).siblings(".add-area").remove(),
            $(this).remove()
        }
    })
}
function fontStyle() {
    if ($(".edit-area-active").length) {
        var e = $(".edit-area-active").html();
        e = e.replace(/<\/?[^>(IMG)(img)][^>]*>/g, ""),
        $(".edit-area-active").html(e)
    }
}
function setLogic(e) {
    require.async(["home:static/js/survey/widget/edit_logic_pop.js"],
    function (i) {
        i.show(e, null, null)
    })
}
function elementInit() {
    $(".edit-area").focus(function () {
        $("div, td, li, label").removeClass("edit-area-active"),
        $(this).hasClass("edit-area-active") || $(this).addClass("edit-area-active"),
        activeFocus();
        var e = parseInt($(this).parents(".question-choice").siblings(".question-title").attr("type"), 10);
        14 != e && 15 != e && ($(this).after($(".edit-img")), $(".edit-img").css($(this).hasClass("qs-content") || $(this).hasClass("title-content") ? {
            width: "30px"
        } : $(this).hasClass("matrix-choice") ? {
            width: "65px"
        } : $(this).parents("td").hasClass("radio_array_title") || $(this).parents("td").hasClass("checkbox_array_title") || $(this).parents("li").hasClass("select_choice") ? {
            width: "95px"
        } : {
            width: "155px"
        }), $(".edit-img").show()),
        $(".edit-area").css({
            background: "#fff",
            border: "none"
        }),
        $(".title-content").css({
            border: "1px solid #dbdbdb"
        }),
        $(this).css({
            background: "#fdf9cd",
            border: "1px solid #333"
        }),
        14 != e && 15 != e && ($(this).hasClass("edit-title") ? ($(".upload-element-img").show(), $(".handle-element").hide()) : $(this).parents("td").hasClass("radio_array_title") || $(this).parents("td").hasClass("checkbox_array_title") || $(this).parents("li").hasClass("select_choice") ? ($(".handle-element").show(), $(".handle-child-element").hide(), $(".upload-element-img").hide()) : $(this).hasClass("matrix-choice") ? ($(".handle-element").show(), $(".up-child-element").hide(), $(".down-child-element").hide(), $(".upload-element-img").hide()) : $(this).hasClass("title-content") ? $(".edit-img").hide() : $(this).hasClass("desc-content") ? ($(".edit-img").css({
            width: "30px"
        }), $(".handle-element").hide()) : ($(".upload-element-img").show(), $(".handle-element").show()));
        var i = $(this).html().replace(/&nbsp;/gi, " ");
        i = i.replace(/<br>/gi, "\n"),
        i = $.trim(i),
        $(this).hasClass("edit-title") && edit.isDefaultQuestion(i) && ($(this).attr("data-value", i), $(this).html("")),
        ($(this).hasClass("edit-child-element") || $(this).hasClass("choice")) && edit.isDefaultChoice(i) && ($(this).attr("data-value", i), $(this).html(""))
    }),
    $(".title-content").on("keydown",
    function (e) {
        if ($(this).text().length >= 17) {
            var i = ($(this).text(), e.keyCode || e.which);
            if (8 != i && 46 != i) return !1
        }
    }),
    $(".edit-area").blur(function () {
        var e = $(this).html().replace(/&nbsp;/gi, " ");
        if (e = e.replace(/<br>/gi, "\n"), e = $.trim(e), $(this).hasClass("title-content")) return void (e.length > 17 ? ($(".survey-title .error-tips").addClass("error"), $(".survey-title .error-tips").text("问卷名称不能超过17字"), surveyTitle = "") : 0 == e.length ? ($(".survey-title .error-tips").addClass("error"), $(".survey-title .error-tips").text("问卷名称不能为空"), surveyTitle = "") : ($(".survey-title .error-tips").removeClass("error"), surveyTitle = e));
        if ("" == e) {
            var i = $(this).attr("data-value");
            if (void 0 !== i) $(this).html(i);
            else if ($(this).hasClass("edit-title")) $(this).nextAll(".error").remove(),
            $(this).after('<span class="error-tips error">题干为空</span>'),
            $(this).trigger("focus");
            else if ($(this).hasClass("edit-child-element")) {
                var t = $(this).parents(".question-choice").children().length;
                t > 1 && $(this).parents(".choice").find(".remove-child-element").click()
            }
        } else $(this).nextAll(".error").remove(),
        0 == edit.isDefaultQuestion(e) && $(this).removeAttr("data-value");
        setUpdateStatus()
    }),
    $(".edit-area").hover(function () {
        $(this).hasClass("edit-area-active") || $(this).css({
            background: "#fdf9cd"
        })
    },
    function () {
        $(this).hasClass("edit-area-active") || $(this).css({
            background: "#fff"
        })
    }),
    $(".edit-area").on("paste",
    function () {
        var e = $(this),
        i = parseInt($(this).parents(".question-choice").siblings(".question-title").attr("type"), 10);
        setTimeout(function () {
            var t = e.text();
            t = t.replace(/<[^<]*>/g, ""),
            (14 == i || 15 == i) && (t = t.substring(0, 50)),
            e.text(t)
        },
        0)
    }),
    $(".edit-area").keypress(function () {
        var e = parseInt($(this).parents(".question-choice").siblings(".question-title").attr("type"), 10);
        return (14 == e || 15 == e) && $(this).text().length >= 50 ? !1 : void 0
    }),
    $("body").on("focus", ".edit-area",
    function () {
        var e = $(this);
        return e.data("before", e.html()),
        e
    }).on("blur keyup paste input", ".edit-area",
    function () {
        var e = $(this);
        return e.data("before") !== e.html() && (e.data("before", e.html()), e.trigger("change")),
        e
    }),
    $(".edit-area").change(function () {
        edit.updateUploadAll()
    }),
    edit.updateQuestionType()
}
function getQuoteIndex(e) {
    var i = /\[Q[0-9]*[1-9][0-9]*\]/g,
    t = e.match(i);
    if (null === t) return !1;
    var s = [];
    return $.each(t,
    function (e, i) {
        var t = i.substring(2, i.length - 1);
        inArray(t, s) || s.push(t)
    }),
    s
}
function checkIndexes(e) {
    if (e) {
        var i = !0;
        return $.each(e,
        function (e, t) {
            "9" == $(".question-id[index=" + t + "]").parent().attr("type") && (i = !1)
        }),
        i
    }
    return !1
}
function activeFocus() {
    $(".choice .edit-area").css({
        display: "inline-block"
    }),
    $(".choice .edit-area-active").css({
        display: "inline-block"
    })
}
function setUpdateStatus() {
    updateStatus = 1
}
function clearUpdateStatus() {
    updateStatus = 1
}
var edit = function () {
    function e() {
        $(".topic-type-content").each(function () {
            var e = $(this).index();
            e += 1,
            $(this).find(".question-id").attr("order", e);
            var i = $(this).prevAll(".topic-page").length;
            i += 1,
            $(this).find(".question-id").attr("page", i);
            var t = $(this).find(".question-title").attr("name"),
            s = $("#question-box").find(".topic-page").length;
            s += 1;
            var a;
            if ("page" === t) $(this).find(".page-area").html("页码:" + i + "/" + s),
            a = 0;
            else if ("description" === t) a = 0;
            else {
                var l = 0;
                $(this).prevAll(".topic-type-question").each(function () {
                    var e = parseInt($(this).find(".question-title").attr("type"), 10);
                    10 != e && l++
                }),
                a = l + 1,
                $(this).find(".question-id").html("Q" + a),
                "Y" == $(this).find(".question-id").attr("question-required") && 0 == $(this).find(".required").length && $(this).find(".question-id").before('<span class="required">*</span>')
            }
            $(this).find(".question-id").attr("index", a),
            $("#page-tail").html("页码:" + s + "/" + s);
            var o = parseInt($(this).find(".question-title").attr("type"), 10);
            if (14 === o || 15 === o) {
                var c = $(this).find(".question-choice");
                N(c.siblings(".add-area"))
            }
        });
        var e = basePath + "/System/Api/Survey/UploadFile/0";
        $(".survey-question-upload-img-file").each(function () {
            $(this).fileupload({
                url: e,
                autoUpload: !0,
                sequentialUploads: !0,
                acceptFileTypes: /(\.|\/)(gif|jpe?g|png|bmp)$/i,
                done: function (e, i) {
                    var t = $(this).parents(".add-area").siblings(".question-choice").find(".choice").length;
                    50 > t && p($(this),i.result),// $.parseJSON(i.result)),
                    edit.updateUploadItem($(this).parents(".add-area"))
                }
            })
        }),
        $("img").load(function () {
            edit.updateUploadAll()
        })
    }
    function i() {
        $(".attach-left").css({
            top: $(".rows2").offset().top,
            width: $(".rows2").offset().left,
            height: $(".rows2").height(),
            left: 0
        }),
        $(".attach-right").css({
            top: $(".rows2").offset().top,
            right: 0,
            width: $(window).width() - $(".rows2").offset().left - $(".rows2").width(),
            height: $(".rows2").height()
        })
    }
    function t() {
        redirect_relation && $.each(redirect_relation,
        function (e, i) {
            if ($("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find(".choice_show_logic_show_questions").remove(), $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").find(".question-title-tip").remove(), i) if (i[0]) {
                var t = '<div class="question-title-tip">选中/填写任意项, ',
                s = "";
                $.each(i[0],
                function (e, i) {
                    if (i > 0) if (parseInt(e, 10) < 0) s = -1 === parseInt(e, 10) ? "提前结束(计入结果)" : "提前结束(不计入结果)";
                    else {
                        s = "跳转";
                        var t = $('[absolute_id="' + e + '"]').html();
                        "" == t && (t = $('[absolute_id="' + e + '"]').next().find(".edit-area").text(), t = '<a class="choice_show_logic_show_questions_link" title="' + t + '">' + t + "</a>"),
                        s += t ? t : ""
                    }
                }),
                t += s + "</div>",
                $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").append(t)
            } else $.each(i,
            function (i, t) {
                if (t) {
                    var s = "";
                    $.each(t,
                    function (e, i) {
                        if (i > 0) if (parseInt(e, 10) < 0) {
                            var t = -1 === parseInt(e, 10) ? "提前结束(计入结果)" : "提前结束(不计入结果)";
                            s = '<span class="choice_show_logic_show_questions"><a class="choice_show_logic_show_questions_link" title="' + t + '">' + t + "</a>"
                        } else {
                            s = '<span class="choice_show_logic_show_questions">跳转';
                            var a = $('[absolute_id="' + e + '"]').html();
                            "" == a && (a = $('[absolute_id="' + e + '"]').next().find(".edit-area").text(), a = '<a class="choice_show_logic_show_questions_link" title="' + a + '">' + a + "</a>"),
                            s += a ? a : ""
                        }
                    }),
                    $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find('[choice_absolute_id="' + i + '"]').find(".choice_show_logic_show_questions").remove(),
                    s += "</span>";
                    var a = $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").attr("type"); !s || "6" != a && "8" != a && "14" != a && "15" != a || $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find('[choice_absolute_id="' + i + '"]').find(".option-tips").append(s)
                }
            })
        })
    }
    function s(e) {
        var i = [];
        return redirect_relation && (redirect_relation[e] && !$.isEmptyObject(redirect_relation[e]) ? inArray(e, i) || i.push(e) : $.each(redirect_relation,
        function (t, s) {
            s && $.each(s,
            function (s, a) {
                a && $.each(a,
                function (s) {
                    parseInt(s, 10) === parseInt(e, 10) && (inArray(t, i) || i.push(t))
                })
            })
        })),
        i
    }
    function a() {
        var e = $.Deferred();
        if (w(surveyTitle)) {
            var i = arguments[0] ? arguments[0] : 0,
            t = arguments[1] ? arguments[1] : 0,
            s = {};
            s.survey_id = survey_init.survey_id,
            s.survey_name = $.trim($(".title-content").text());
            var a = $(".desc-content").html().replace(/&nbsp;/gi, " ").replace(/<br>/gi, "\n");
            s.test_content = $.trim(a),
            s.status = i,
            s.content = [],
            s.redirect_relation = getRelation();
            var c = 1;
            if ($(".topic-type-content").each(function () {
                var e = {};
                e.survey_id = survey_init.survey_id;
                var i = $(this).find(".qs-content").html();
                void 0 !== i && (i = i.replace(/&nbsp;/gi, " ").replace(/<br>/gi, "\n")),
                "" == i ? ($("html,body").animate({
                scrollTop: $(this).offset().top - 50 + "px"
            },
                500), $(this).find(".qs-content").trigger("focus"), c = 0) : e.content = $.trim(i),
                e.type_id = $(this).find(".question-title").attr("type"),
                e.order = $(this).find(".question-id").attr("order"),
                e.has_other = $(this).find(".question-id").attr("has_other"),
                e.title_quote = $(this).find(".question-id").attr("title-quote"),
                e.required = $(this).find(".question-id").attr("question-required"),
                e.page = $(this).find(".question-id").attr("page"),
                e.index = $(this).find(".question-id").attr("index"),
                e.absolute_id = $(this).find(".question-id").attr("absolute_id"),
                e.last_absolute_id = absolute_id,
                e.last_choice_absolute_id = choice_absolute_id;
                var t = 0;
                e.logic_hide = t;
                switch (e.type_id) {
                case "6":
                case "14":
                    if (e.choice = l($(this), e.type_id), e.choice_quote = $(this).find(".question-id").attr("choice-quote"), 0 !== parseInt(e.choice_quote) && !isNaN(parseInt(e.choice_quote))) {
                        var a = $("[absolute_id=" + e.choice_quote + "]").attr("order");
                        e.choice = s.content[parseInt(a, 10) - 1].choice
            }
                    break;
                case "8":
                case "15":
                    var n = $(this).find(".question-id").attr("min");
                    e.min = "" !== n && "undefined" !== n && null !== n && void 0 !== n ? n : "";
                    var d = $(this).find(".question-id").attr("max");
                    e.max = "" !== d && "undefined" !== d && null !== d && void 0 !== d ? d : "";
                    var r = $(this).find(".question-id").attr("exclusive-options");
                    if (e.exclusive_options = "" !== r && "undefined" !== r && null !== r && void 0 !== r ? r : "", e.choice = l($(this), e.type_id), e.choice_quote = $(this).find(".question-id").attr("choice-quote"), 0 !== parseInt(e.choice_quote) && !isNaN(parseInt(e.choice_quote))) {
                        var a = $("[absolute_id=" + e.choice_quote + "]").attr("order");
                        e.choice = s.content[parseInt(a, 10) - 1].choice
            }
                    break;
                case "7":
                    if (e.choice = l($(this), e.type_id), e.choice_quote = $(this).find(".question-id").attr("choice-quote"), 0 !== parseInt(e.choice_quote) && !isNaN(parseInt(e.choice_quote))) {
                        var a = $("[absolute_id=" + e.choice_quote + "]").attr("order");
                        e.choice = s.content[parseInt(a, 10) - 1].choice
            }
                    break;
                case "9":
                    e.choice = l($(this), e.type_id),
                    e.radio_array_title = o($(this), "radio_array_title");
                    break;
                case "13":
                    e.choice = l($(this), e.type_id),
                    e.checkbox_array_title = o($(this), "checkbox_array_title");
                    break;
                case "12":
                    e.min = "",
                    e.max = 4
            }
                s.content.push(e)
            }), 0 == c) return !1;
            var n = basePath + "/System/Api/Survey/Design";
            $.post(n, { content: JSON.stringify(s) }, function (rv) {
                if (rv.success) {
                    $(".time-save").css({
                        top: $(window).scrollTop(),
                        left: $(window).width() / 2
                    }).fadeIn(300).fadeOut(2e3)
                }
                e.resolve();
            })
            //$.ajax({
            //    type: "post",
            //    url: n,
            //    data: { content: JSON.stringify(s) },
            //    contentType: "application/json",
            //    traditional: !0,
            //    success: function (s) {
            //           if (s.success) {
            //            $(".time-save").css({
            //                top: $(window).scrollTop(),
            //                left: $(window).width() / 2
            //            }).fadeIn(300).fadeOut(2e3)
            //        }
            //        e.resolve()
            //    }
            //})
        }
        return e.promise();
    }
    function l(e, i) {
        var t = [],
        s = /&NBSP;/gi;
        return e.find(".choice").each(function () {
            var e = {};
            if ("9" == i || "13" == i) {
                e.order = $(this).parents("td").index();
                var a = $(this).html().replace(s, "");
                e.choice_absolute_id = $(this).parents("td").index()
            } else if ("7" == i) {
                e.order = $(this).index() + 1;
                var a = $(this).html().replace(s, "");
                e.choice_absolute_id = $(this).attr("choice_absolute_id")
            } else if ("14" == i) {
                e.order = $(this).index() + 1;
                var l = $(this).find(".edit-area").html().replace(s, ""),
                o = $(this).find(".survey-question-radio-choice-img>img").attr("src");
                a = l + "#**#" + o,
                e.choice_absolute_id = $(this).attr("choice_absolute_id")
            } else if ("15" == i) {
                e.order = $(this).index() + 1;
                var l = $(this).find(".edit-area").html().replace(s, ""),
                o = $(this).find(".survey-question-checkbox-choice-img>img").attr("src");
                a = l + "#**#" + o,
                e.choice_absolute_id = $(this).attr("choice_absolute_id")
            } else {
                e.order = $(this).index() + 1;
                var a = $(this).find(".edit-area").html().replace(s, "");
                e.choice_absolute_id = $(this).attr("choice_absolute_id")
            }
            e.content = a,
            e.is_other = $(this).attr("has_other"),
            e.required = $(this).attr("choice_required"),
            t.push(e)
        }),
        t
    }
    function o(e, i) {
        var t = [];
        return e.find("." + i).each(function () {
            var e = {};
            e.order = $(this).parent().index(),
            e.content = $(this).find(".edit-area").html(),
            t.push(e)
        }),
        t
    }
    function c() {
        var e = $("#question-box").find(".topic-type-content").length;
        e > 3 && $("html, body").animate({
            scrollTop: $("#edit-survey-content").height() - 200
        },
        "slow")
    }
    function n() {
        $(".topic-type-content").hover(function () {
            $(".add-area, .operate").not(".survey-question-upload-img-wrap").removeClass("visible-show").addClass("visible-hide"),
            $(this).find(".add-area, .operate").not(".survey-question-upload-img-wrap").removeClass("visible-hide").addClass("visible-show")
        })
    }
    function d(i) {
        var t = $(i).parents(".topic-type-content");
        t.after(t.clone());
        var s = parseInt(absolute_id) + 1;
        $(i).parents(".topic-type-content").next().find(".question-id").attr("absolute_id", s),
        absolute_id++,
        $(i).parents(".topic-type-content").next().find(".choice_show_logic_show_questions").remove(),
        choice_absolute_id[s] = 0,
        $(i).parents(".topic-type-content").next().find(".choice").each(function () {
            $(this).attr("choice_absolute_id") > choice_absolute_id[s] && (choice_absolute_id[s] = $(this).attr("choice_absolute_id"))
        }),
        e(),
        setUpdateStatus(),
        n(),
        elementInit()
    }
    function r(e) {
        $("#question-box").append($(".edit-img")),
        require.async(["home:static/js/survey/widget/operate_popup.js"],
        function (i) {
            i.show("删除此题卡", "确定", "取消").then(function () {
                var i = $(e).parents(".topic-type-content"),
                t = i.find(".question-id").attr("absolute_id");
                i.remove(),
                edit.sortQuestions(),
                edit.removeLogicCondition(t, i),
                setUpdateStatus()
            })
        })
    }
    function u(e) {
        redirect_relation && redirect_relation[e] && !$.isEmptyObject(redirect_relation[e]) && (delete redirect_relation[e], $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").siblings(".question-choice").find(".choice_show_logic_show_questions").remove(), $("#question-box").find('[absolute_id="' + e + '"]').parents(".question-title").find(".question-title-tip").remove(), edit.updateChoiceShowLogic())
    }
    function h(e) {
        var i = $(e).parents(".operate").siblings(".question-title").attr("type"),
        t = $(e).parents(".operate").siblings(".question-title").find(".question-id").attr("order");
        require.async(["home:static/js/survey/widget/question_handle.js"],
        function (s) {
            s.show(e, i, t, "确定", "取消")
        })
    }
    function p(e) {
        var i = $(e).parents(".add-area").siblings(".question-title").attr("type"),
        t = "",
        s = parseInt($(e).parents(".add-area").siblings(".question-title").find(".question-id").attr("absolute_id"));
        choice_absolute_id[s]++;
        var a = choice_absolute_id[s];
        switch (i) {
            case "6":
                var l = $(e).parents(".add-area").siblings(".question-choice").find(".choice").length + 1;
                t += '<li class="choice" has_other="N" choice_absolute_id=' + a + '><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项' + l + '</div></div><div class="option-tips"></div></li>',
                $(e).parents(".add-area").siblings(".question-choice").append(t),
                $(e).parents(".add-area").siblings(".question-choice").hasClass("select-ul") && $(e).parents(".add-area").siblings(".question-choice").find(".choice").addClass("select_choice");
                break;
            case "14":
                var o = arguments[1],
                l = $(e).parents(".add-area").siblings(".question-choice").find(".choice").length + 1;
                t += '<li class="choice survey-question-radio-img" has_other="N" choice_absolute_id=' + a + '><div class=survey-question-radio-choice> <div class=survey-question-radio-choice-img><a class="remove-child-element survey-question-radio-img-remove" onclick="edit.removeChildElement()"></a> <img src="' + o.img_url + '"> </div> <div class=survey-question-radio-choice-text> <input type=radio name=radio> <label class="edit-area edit-child-element" contenteditable=true>选项' + l + '</label> <div class="option-tips"></div></div> </div></li>',
                $(e).parents(".add-area").siblings(".question-choice").append(t),
                $(e).parents(".add-area").siblings(".question-choice").hasClass("select-ul") && $(e).parents(".add-area").siblings(".question-choice").find(".choice").addClass("select_choice");
                break;
            case "15":
                var o = arguments[1],
                l = $(e).parents(".add-area").siblings(".question-choice").find(".choice").length + 1;
                t += '<li class="choice survey-question-checkbox-img" has_other="N" choice_absolute_id=' + a + '><div class=survey-question-checkbox-choice> <div class=survey-question-checkbox-choice-img><a class="remove-child-element survey-question-checkbox-img-remove" onclick="edit.removeChildElement()"></a> <img src="' + o.img_url + '"> </div> <div class=survey-question-checkbox-choice-text> <input type=checkbox name=checkbox> <label class="edit-area edit-child-element" contenteditable=true>选项' + l + '</label> <div class="option-tips"></div></div> </div></li>',
                $(e).parents(".add-area").siblings(".question-choice").append(t),
                $(e).parents(".add-area").siblings(".question-choice").hasClass("select-ul") && $(e).parents(".add-area").siblings(".question-choice").find(".choice").addClass("select_choice");
                break;
            case "8":
                var l = $(e).parents(".add-area").siblings(".question-choice").find(".choice").length + 1;
                t += '<li class="choice" has_other="N" choice_absolute_id=' + a + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项' + l + '</div></div><div class="option-tips"></div></li>',
                $(e).parents(".add-area").siblings(".question-choice").append(t);
                break;
            case "9":
                if ("N" === $(e).parents(".add-area").attr("choice")) {
                    var l = $(e).parents(".add-area").siblings(".question-choice").find("tr").length;
                    t += '<tr><td class="radio_array_title" name="radio-matrix"> <div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行' + l + "</div></div></td>";
                    for (var c = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td").length, n = 1; c > n; n++) {
                        t += '<td><input type="radio">';
                        var d = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td:eq(" + n + ")").find(".choice");
                        "Y" == d.attr("has_other") && (t += '<input type="text" class="other-content"  style="width: 120px;height: 30px;vertical-align: middle;" >', 1 == d.attr("choice_required") && (t += '<span class="required-content">(必填)</span>')),
                        t += "</td>"
                    }
                    t += "</tr>",
                    $(e).parents(".add-area").siblings(".question-choice").find("tbody").append(t)
                } else {
                    var l = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td").length;
                    t += '<td name="radio-matrix-choice"><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项' + l + "</li></div></td>",
                    $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").append(t),
                    $(e).parents(".add-area").siblings(".question-choice").find("tr").each(function () {
                        $(this).index() > 0 && $(this).append('<td><input type="radio"></td>')
                    })
                }
                break;
            case "13":
                if ("N" === $(e).parents(".add-area").attr("choice")) {
                    var l = $(e).parents(".add-area").siblings(".question-choice").find("tr").length;
                    t += '<tr><td class="checkbox_array_title" name="checkbox-matrix"> <div class="position-relative" style="width:100%"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">矩阵行' + l + "</div></div></td>";
                    for (var c = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td").length, n = 1; c > n; n++) {
                        t += '<td><input type="checkbox">';
                        var d = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td:eq(" + n + ")").find(".choice");
                        "Y" == d.attr("has_other") && (t += '<input type="text" class="other-content"  style="width: 120px;height: 30px;vertical-align: middle;" >', 1 == d.attr("choice_required") && (t += '<span class="required-content">(必填)</span>')),
                        t += "</td>"
                    }
                    t += "</tr>",
                    $(e).parents(".add-area").siblings(".question-choice").find("tbody").append(t)
                } else {
                    var l = $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").find("td").length;
                    t += '<td name="checkbox-matrix-choice"><div class="position-relative" style="width:100%"><li class="choice edit-area matrix-choice" has_other="N" contenteditable="true" content-default="1">选项' + l + "</li></div></td>",
                    $(e).parents(".add-area").siblings(".question-choice").find("tr:eq(0)").append(t),
                    $(e).parents(".add-area").siblings(".question-choice").find("tr").each(function () {
                        $(this).index() > 0 && $(this).append('<td><input type="checkbox"></td>')
                    })
                }
        }
        elementInit(),
        $(e).parents(".add-area").siblings(".question-choice").hasClass("select-ul") || setUpdateStatus()
    }
    function v(e) {
        require.async(["home:static/js/survey/widget/batchAddChoice.js"],
        function (i) {
            i.show(e, "确定", "取消")
        })
    }
    function q() {
        var e = $(".edit-area-active").parents(".question-choice"),
        i = parseInt($(".edit-area-active").parents(".topic-type-question").find(".question-title").attr("type"), 10);
        if ($("#question-box").append($(".edit-img")), $(".edit-area-active").parents("td").hasClass("radio_array_title") || $(".edit-area-active").parents("td").hasClass("checkbox_array_title")) $(".edit-area-active").parents("tr").remove();
        else if ($(".edit-area-active").hasClass("matrix-choice")) {
            var t = $(".edit-area-active").parents("td").index();
            $(".edit-area-active").parents("tr").siblings("tr").each(function () {
                $(this).find("td:eq(" + t + ")").remove()
            }),
            $(".edit-area-active").parents("td").remove()
        } else {
            var s = $(".edit-area-active").parents(".question-choice").children().length;
            if (s > 1) {
                var a = $(".edit-area-active").parents(".topic-type-question");
                if ("8" === a.find(".question-title").attr("type") && a.find(".question-id").attr("exclusive-options")) {
                    var l = a.find(".question-id").attr("exclusive-options").split(","),
                    o = [];
                    $.each(l,
                    function (e, i) {
                        i !== $(".edit-area-active").parents(".choice").attr("choice_absolute_id") && o.push(i)
                    }),
                    a.find(".question-id").attr("exclusive-options", o.join(","))
                }
                var c = a.find(".question-id").attr("absolute_id");
                redirect_relation && redirect_relation[c] && !$.isEmptyObject(redirect_relation[c]) ? require.async(["home:static/js/survey/widget/sortable_popup.js"],
                function (e) {
                    e.show("该题有关联的逻辑规则，删除选项会导致规则失效，确认删除？", "确定", "取消").then(function () {
                        $(".edit-area-active").parents(".choice").remove(),
                        edit.removeLogicCondition(c, a)
                    })
                }) : $(".edit-area-active").parents(".choice").remove()
            } else showAlert("选项个数不能为0")
        }
        $(".edit-img").hide(),
        (14 === i || 15 === i) && N(e.siblings(".add-area")),
        $(".edit-area-active").parent().hasClass("select_choice") || setUpdateStatus()
    }
    function b() {
        var e, i = !1;
        if ($(".edit-area-active").parents("td").hasClass("radio_array_title") || $(".edit-area-active").parents("td").hasClass("checkbox_array_title") ? (e = $(".edit-area-active").parents("tr"), e.index() > 1 && (i = !0)) : (e = $(".edit-area-active").parents(".choice"), e.index() > 0 && (i = !0)), i) {
            {
                var t = $(".edit-area-active").parents(".topic-type-question");
                t.find(".question-id").attr("absolute_id")
            }
            e.prev().before(e),
            $(".edit-area-active").parent().hasClass("select_choice") || setUpdateStatus()
        }
    }
    function _() {
        var e, i;
        $(".edit-area-active").parents("td").hasClass("radio_array_title") || $(".edit-area-active").parents("td").hasClass("checkbox_array_title") ? (e = $(".edit-area-active").parents("tr"), i = $(".edit-area-active").parents(".question-choice").find("tr")) : (e = $(".edit-area-active").parents(".choice"), i = $(".edit-area-active").parents(".question-choice").find(".choice"));
        var t = e.index();
        if (t < i.length) {
            {
                var s = $(".edit-area-active").parents(".topic-type-question");
                s.find(".question-id").attr("absolute_id")
            }
            e.next().after(e),
            $(".edit-area-active").parent().hasClass("select_choice") || setUpdateStatus()
        }
    }
    function m() {
        require.async(["home:static/js/survey/widget/choice_operate_dialog.js"],
        function (e) {
            e.show("确定", "取消")
        })
    }
    function g(e) {
        var i = '<ul class="question-choice select-ul" >',
        t = $(e).siblings(".question-title").find(".question-id").attr("absolute_id");
        $(e).siblings(".question-choice").find(".choice").each(function () {
            var e = $(this).attr("choice_absolute_id");
            if (i += '<li class="choice select_choice" has_other="N" choice_absolute_id=' + $(this).attr("choice_absolute_id") + '><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true">' + $(this).html() + "</div></div>", redirect_relation && redirect_relation[t] && !redirect_relation[t][0] && redirect_relation[t][e]) {
                var s = "";
                $.each(redirect_relation[t][e],
                function (e) {
                    if (parseInt(e, 10) < 0) {
                        var i = -1 === parseInt(e, 10) ? "提前结束(计入结果)" : "提前结束(不计入结果)";
                        s = '<div><span class="choice_show_logic_show_questions">' + i
                    } else s = '<div><span class="choice_show_logic_show_questions">跳转',
                    s += $('[absolute_id="' + e + '"]').html() + "&nbsp"
                }),
                s += "</span></div>",
                i += s
            }
            i += "</li>"
        }),
        i += "</ul>",
        $(e).siblings(".question-choice").remove(),
        $(e).after(i);
        var s = '<div class="add-area visible-show"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li><li class="finish_edit_select" onclick="edit.finishEditSelect(this)" style="width: 60px;cursor: pointer;">完成编辑</li></ul></div>';
        $(e).siblings(".operate").before(s),
        $(e).siblings(".question-title").attr("type", "6"),
        $(e).remove(),
        elementInit()
    }
    function f(e) {
        $("#question-box").append($(".edit-img"));
        var i = '<select class="question-choice" style="  padding: 0;margin: 15px 0 20px 35px;">';
        $(e).parents(".add-area").siblings(".question-choice").find(".choice").each(function () {
            i += '<option class="choice" has_other="N" choice_absolute_id=' + $(this).attr("choice_absolute_id") + ">" + $(this).find(".edit-area").html() + "</option>"
        }),
        i += '</select><span class="edit-select" onclick="edit.editSelect(this)" style="width: 60%;display: inline-block;line-height: 30px;">编辑选项</span>',
        $(e).parents(".add-area").siblings(".question-choice").remove(),
        $(e).parents(".add-area").siblings(".question-title").after(i),
        $(e).parents(".add-area").siblings(".question-title").attr("type", "7"),
        $(e).parents(".add-area").remove(),
        $(".edit-img").hide(),
        setUpdateStatus()
    }
    function y() {
        $(".upload-form").ajaxSubmit({
            dataType: "json",
            success: function (e) {
                var i = $(".edit-area-active").html();
                i += '<img src="' + e.img_url + '" name="' + e.img_id + '"  style="width:' + e.width + "px; height:" + e.height + 'px;"/>',
                    $(".edit-area-active").html(i),
                    $(".edit-area").find("img").each(function () {
                        new ImgEditSize($(this))
                    }),
                    $(".upload-img").val(""),
                    setUpdateStatus();
                return false;
            }
        })
    }
    function x(e) {
        var i = ["单选题", "选择列表", "多选题", "单行填空题", "多行填空题", "矩阵单选题", "矩阵多选题", "描述说明", "图片单选题", "图片多选题"];
        return i.indexOf(e) >= 0 ? !0 : !1
    }
    function k(e) {
        var i = /^(选项|矩阵行)\d*$/g;
        return null === e.match(i) ? !1 : !0
    }
    function w(e) {
        return 0 == e.length ? ($("html,body").animate({
            scrollTop: $(".title-content").offset().top - 50 + "px"
        },
        500), $(".survey-title .error-tips").text("问卷名称不能为空"), !1) : e.length > 17 ? ($("html,body").animate({
            scrollTop: $(".title-content").offset().top - 50 + "px"
        },
        500), $(".survey-title .error-tips").text("问卷名称不能超过17字"), !1) : !0
    }
    function C() {
        $(".topic-type-question").each(function () {
            if (8 === parseInt($(this).find(".question-title").first().attr("type"), 10)) {
                $(this).find(".exclusive-option-tip").remove();
                var e = $(this).find(".question-id").attr("exclusive-options");
                if (e) {
                    var i = e.split(","),
                    t = $(this);
                    $.each(i,
                    function (e, i) {
                        t.find(".choice[choice_absolute_id=" + i + "] .option-tips").append('<span class="exclusive-option-tip">与其他选项互斥</span>')
                    })
                }
            }
        })
    }
    function I() {
        $(".survey-question-radio-choice").mouseenter(function () {
            $("div, td, li, label").removeClass("edit-area-active"),
            $(this).hasClass("edit-area-active") || $(this).addClass("edit-area-active"),
            $(".survey-question-radio-img-remove").hide(),
            $(this).find(".survey-question-radio-img-remove").show()
        }),
        $(".survey-question-radio-choice").mouseleave(function () {
            $(".survey-question-radio-img-remove").hide()
        }),
        $(".survey-question-checkbox-choice").mouseenter(function () {
            $("div, td, li, label").removeClass("edit-area-active"),
            $(this).hasClass("edit-area-active") || $(this).addClass("edit-area-active"),
            $("div, td, li, label").removeClass("edit-area-active"),
            $(this).hasClass("edit-area-active") || $(this).addClass("edit-area-active"),
            $(".survey-question-checkbox-img-remove").hide(),
            $(this).find(".survey-question-checkbox-img-remove").show()
        }),
        $(".survey-question-checkbox-choice").mouseleave(function () {
            $(".survey-question-checkbox-img-remove").hide()
        })
    }
    function N(e) {
        var i = e.siblings(".question-choice").find(".choice").length;
        50 > i ? e.show() : e.hide(),
        A()
    }
    function A() {
        $(".survey-question-upload-img-wrap").each(function () {
            var e = $(this).siblings(".question-choice").find(".choice").length;
            if (e % 4 == 0) $(this).css("position", "relative").css("left", 0).css("top", 0);
            else {
                var i = $(this).siblings(".question-choice").find(".choice:last"),
                t = i.offset(),
                s = t.left + i.width() - 35 + "px",
                a = t.top - 57 + "px";
                $(this).css("position", "absolute").css("left", s).css("top", a)
            }
        })
    }
    function D() {
        $(".topic-type-question").each(function () {
            var e = parseInt($(this).find(".question-title").first().attr("type"), 10);
            if (6 === e || 7 === e || 8 === e) {
                var i = parseInt($(this).find(".question-id").attr("choice-quote"), 10);
                if (0 === i || isNaN(i)) {
                    if ($(this).find(".choice-quote-tip").length > 0) {
                        $(this).find(".question-title").siblings().remove();
                        var t = "",
                        s = $(this).find(".question-id").attr("absolute_id"),
                        a = choice_absolute_id[s];
                        switch (e) {
                            case 7:
                                t += '<select class="question-choice" style="  padding: 0;margin: 15px 0 20px 35px;">';
                                for (var l = 1; 4 > l; l++) choice_absolute_id[s]++,
                                a = choice_absolute_id[s],
                                t += '<option class="choice" has_other="N" choice_absolute_id=' + a + ">选项" + l + "</option>";
                                t += '</select><span class="edit-select" style="  width: 60%;display: inline-block;line-height: 30px;" onclick="edit.editSelect(this)">编辑选项</span>',
                                t += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
                                break;
                            case 6:
                                t += '<ul class="question-choice"><li class="choice" has_other="N" choice_absolute_id=' + a + ' ><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项1 </div></div><div class="option-tips"></div></li>',
                                choice_absolute_id[s]++,
                                a = choice_absolute_id[s],
                                t += '<li class="choice" has_other="N" choice_absolute_id=' + a + '><input type="radio" name="radio"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项2 </div><div></li><div class="option-tips"></div></ul>',
                                t += '<div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
                                t += '<div class="operate visible-hide" ><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>';
                                break;
                            case 8:
                                t += '<ul class="question-choice"><li class="choice" has_other="N" choice_absolute_id=' + a + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项1 </div></div><div class="option-tips"></div></li>',
                                choice_absolute_id[s]++,
                                a = choice_absolute_id[s],
                                t += '<li class="choice" has_other="N" choice_absolute_id=' + a + '><input type="checkbox" name="checkbox"><div class="position-relative"><div class="edit-area edit-child-element" contenteditable="true" content-default="1">选项2 </div></div><div class="option-tips"></div></li></ul>',
                                t += '<div class="add-area visible-hide"><ul><li class="add-choice" title="增加" onclick="edit.addQuestion(this)"></li><li class="batch-add-choice" title="批量增加" onclick="edit.batchAddChoice(this)"></li></ul></div>',
                                t += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="set-logic" title="逻辑设置" onclick="setLogic(this)"><li class="question-copy" title="复制" onclick="edit.questionCopy(this)"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>'
                        }
                        $(this).find(".question-title").after(t)
                    }
                } else {
                    $(this).find(".question-title").siblings().remove();
                    var o = "";
                    switch (e) {
                        case 6:
                            o = "单选题";
                            break;
                        case 7:
                            o = "下拉选择题";
                            break;
                        case 8:
                            o = "多选题"
                    }
                    var t = '<div class="choice-quote-tip">&lt;' + o + "&gt此题目选项来自于" + $('[absolute_id="' + i + '"]').html() + "题中的选项</div>";
                    t += '<div class="operate visible-hide"><ul><li class="drag-area" title="移动"></li><li class="question-handle" title="操作" onclick="edit.questionHandle(this)"></li><li class="question-delete" title="删除" onclick="edit.questionDelete(this)"></li></ul></div>',
                    $(this).find(".question-title").after(t)
                }
            }
        })
    }
    return {
        sortQuestions: e,
        attachLayer: i,
        updateChoiceShowLogic: t,
        checkQuestionLogic: s,
        saveSurvey: a,
        scrollBox: c,
        visibleHandle: n,
        removeLogicCondition: u,
        questionHandle: h,
        questionCopy: d,
        questionDelete: r,
        addQuestion: p,
        batchAddChoice: v,
        removeChildElement: q,
        upChildElement: b,
        downChildElement: _,
        handleChildElement: m,
        editSelect: g,
        finishEditSelect: f,
        uploadImg: y,
        isDefaultQuestion: x,
        isDefaultChoice: k,
        showExclusiveOptions: C,
        updateByChoiceQuote: D,
        updateQuestionType: I,
        updateUploadItem: N,
        updateUploadAll: A
    }
}(),
showAlert = function (e) {
    require.async(["home:static/js/survey/widget/operate_popup.js"],
    function (i) {
        i.show(e, "确定", null)
    })
};