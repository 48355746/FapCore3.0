//打开新窗口（不被拦截）
var openUrl = function (url) {   
    //弹出即全屏
    var index = layer.open({
        type: 2,
        content: url,
        area: ['320px', '195px'],
        maxmin: true
    });
    layer.full(index);
};
var openNewWindow = function (url,closeCallback) {
    //弹出即全屏
    var width = parseInt($(window).innerWidth() * 0.8) + 'px';
    var height = parseInt($(window).innerHeight() - 60) + 'px';
    var index = layer.open({
        type: 2,
        content: url,
        area: [width, height],
        //btn: '关闭',
        maxmin: true,
        yes: function (index) {
            layer.close(index);
            closeCallback && closeCallback();
        }
    });
};
//依赖layer,使用之前先引入"/Content/layer/layer.js"
var layerWindow = function (url) {
    var index = layer.open({
        type: 2,
        content: url,
        area: ['320px', '195px'],
        maxmin: true
    });
    layer.full(index);
};
//打开模态页面
var bootboxWindow = function (title, url, buttons, data,onBeforeShow) {
    if (buttons === undefined || buttons === null) {
        buttons = {
        };
    }
    var dialog = bootbox.dialog({
        title: title,
        message: '<p><i class="fa fa-spin fa-spinner"></i> Loading...</p>',
        size: "large",
        footer: false,
        buttons: buttons       
    });
    //dialog.on("shown.bs.modal", function () {
    //    if ($.isFunction(onBeforeShow)) {
    //        onBeforeShow();
    //    } 
    //});    
    dialog.init(function () {       
        $.get(url,data, function (ev) {
            dialog.find('.bootbox-body').html(ev);
            if ($.isFunction(onBeforeShow)) {
                onBeforeShow();
            } 
        });
    });
};

//利用window.open实现post方式的参数传递
//page 是要跳转的页面URL
//data 是参数， 字符串或者数组
//isPopup 是否弹出对话框
//popupTitle 弹出对话框的标题
function OpenPostWindow(url, data, isPopup, popupTitle, popupSize) {
    var tempForm = document.createElement("form");
    tempForm.id = "tempForm1";
    tempForm.method = "post";
    tempForm.target = "newPage";
    tempForm.action = url; //在此处设置你要跳转的url  

    if (data) {
        if (typeof(data) === 'string') {
            var hiddInput_data = document.createElement("input");
            hiddInput_data.type = "hidden";
            hiddInput_data.name = "data";
            hiddInput_data.value = data;
            tempForm.appendChild(hiddInput_data);
        } else if (data instanceof Array) {
            for (var i = 0; i < data.length; i++) {
                hiddInput_data = document.createElement("input");
                hiddInput_data.type = "hidden";
                hiddInput_data.name = "data" + i;
                hiddInput_data.value = data[i];
                tempForm.appendChild(hiddInput_data);
            }
        } else if (data instanceof Object) {
            for (var item in data) {
                hiddInput_data = document.createElement("input");
                hiddInput_data.type = "hidden";
                hiddInput_data.name = item;
                hiddInput_data.value = data[item];
                tempForm.appendChild(hiddInput_data);
            }
        }
    }

    var $tempForm = $(tempForm);
    $tempForm.submit(function () {
        if (isPopup) {
            popupTitle = popupTitle || '对话框';
            if (!popupSize) popupSize = {};
            var iWidth = popupSize.width || 720;
            var iHeight = popupSize.height || 720;
            var iTop = (window.screen.availHeight - 30 - iHeight) / 2;
            var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;
            var sunwin = window.open('about:blank', 'newPage', 'height=' + iHeight + 'px, width=' + iWidth + 'px, top=' + iTop + ', left=' + iLeft + ', toolbar=no, menubar=no, scrollbars=yes, location=no, status=yes');
            sunwin.focus();
        } else {
            sunwin = window.open('about:blank');
            sunwin.focus();
        }

    });
    document.body.appendChild(tempForm);
    $tempForm.trigger("submit");
    document.body.removeChild(tempForm);
};
//touch click helper
(function ($) {
    $.fn.tclick = function (onclick) {
        this.bind("touchstart", function (e) { onclick.call(this, e); e.stopPropagation(); e.preventDefault(); });
        this.bind("click", function (e) { onclick.call(this, e); });   //substitute mousedown event for exact same result as touchstart         
        return this;
    };

    $.fn.serializeObject = function () {
        var n = {},
            t = this.serializeArray();
        return $.each(t,
            function () {
                n[this.name] !== undefined ? (n[this.name].push || (n[this.name] = [n[this.name]]), n[this.name].push(this.value || "")) : n[this.name] = this.value || ""
            }),
            n
    };
    //自适应窗口高度
    $.fn.autoWindowHeight = function (adjustment) {
        var height = 0;
        if (adjustment) {
            height = adjustment;
        }
        var offsetWidget = this.offset();
        availableHeight = $(window).height() - offsetWidget.top - height;
        this.css("height", availableHeight + 'px');
    };
    $.fn.doScroll = function (size) {
        var height = 0;
        if (size !== undefined) {
            height = size;
        }
        var offsetWidget = this.offset();
        var availableHeight = $(window).height() - offsetWidget.top - height;
        $(this).ace_scroll({
            //styleClass: 'scroll-left scroll-margin scroll-thin scroll-dark scroll-light no-track scroll-visible'
            styleClass: 'scroll-top',
            size: availableHeight,
            mouseWheelLock: true,
            observeContent: true
        });
        setTimeout(function () {
            $('.scrollable').trigger("mouseenter.ace_scroll");
        }, 0);
    };
})(jQuery);
jQuery.randomUrl = function (url) {
    var getTimestamp = new Date().getTime();
    if (url.indexOf("?") > -1) {
        url = url + "&tamp=" + getTimestamp;
    } else {
        url = url + "?timestamp=" + getTimestamp;
    }
    return url;
};
/**
 * 删除数据的ajax-delete请求
 */
$.delete = function (url, data, callback) {
    if ($.isFunction(data)) {
        callback = data;
        data = undefined;
    }
    $.ajax({
        url: url,
        type: "delete",
        contentType: "application/json",
        dataType: "json",
        data: data,
        success: callback
    });
};
//js 多语
$.lang = function (langkey, langValue) {
   return MultiLangHelper.getResName(langkey, langValue);
};
$.msg = function (message, posotion) {
    $.gritter.add({
        title: $.lang('information', '提示信息'),
        text: message,
        sticky: false,
        time: 1000,
        speed: 500,
        position: posotion || 'gritter-center',
        class_name: 'gritter-success  gritter-light'//gritter-center
    });
};
//千位符格式化
$.toThousands = function (num) {
    return (num || 0).toString().replace(/(\d)(?=(?:\d{3})+$)/g, '$1,');
};
var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i) ? true : false;
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i) ? true : false;
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i) ? true : false;
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i) ? true : false;
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Windows());
    }
};
var HtmlUtil = {
    /*1.用正则表达式实现html转码*/
    htmlDecode : function (value) {
        if (value && (value === '&nbsp;' || value === '&#160;' || (value.length === 1 && value.charCodeAt(0) === 160))) { return ""; }
        return !value ? value : String(value).replace(/&gt;/g, ">").replace(/&lt;/g, "<").replace(/&quot;/g, '"').replace(/&amp;/g, "&");
    },
    htmlEncode : function (value) {
        return !value ? value : String(value).replace(/&/g, "&amp;").replace(/\"/g, "&quot;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }
    
};

//if(isMobile.any()){  
//    alert("是移动浏览器");  
//} 
//$.fn.serializeObject = function () {
//    var o = {};
//    var a = this.serializeArray();
//    $.each(a, function () {
//        if (o[this.name] !== undefined) {
//            if (!o[this.name].push) {
//                o[this.name] = [o[this.name]];
//            }
//            o[this.name].push(this.value || '');
//        } else {
//            o[this.name] = this.value || '';
//        }
//    });
//    return o;
//};
//jquery.validate.js 中文包
/*
 * Translated default messages for the jQuery validation plugin.
 * Locale: ZH (Chinese, 中文 (Zhōngwén), 汉语, 漢語)
 */
//$.extend($.validator.messages, {
//    required: "这是必填字段",
//    remote: "请修正此字段",
//    email: "请输入有效的电子邮件地址",
//    url: "请输入有效的网址",
//    date: "请输入有效的日期",
//    dateISO: "请输入有效的日期 (YYYY-MM-DD)",
//    number: "请输入有效的数字",
//    digits: "只能输入数字",
//    creditcard: "请输入有效的信用卡号码",
//    equalTo: "你的输入不相同",
//    extension: "请输入有效的后缀",
//    maxlength: $.validator.format("最多可以输入 {0} 个字符"),
//    minlength: $.validator.format("最少要输入 {0} 个字符"),
//    rangelength: $.validator.format("请输入长度在 {0} 到 {1} 之间的字符串"),
//    range: $.validator.format("请输入范围在 {0} 到 {1} 之间的数值"),
//    max: $.validator.format("请输入不大于 {0} 的数值"),
//    min: $.validator.format("请输入不小于 {0} 的数值")
//});

//获取jqgrid sql
var getStringForGroup = function (group) {
    var s = "(", index;
    if (group.groups !== undefined) {
        for (index = 0; index < group.groups.length; index++) {
            if (s.length > 1) {
                s += " " + group.groupOp + " ";
            }
            try {
                s += this.getStringForGroup(group.groups[index]);
            } catch (eg) { alert(eg); }
        }
    }

    if (group.rules !== undefined) {
        try {
            for (index = 0; index < group.rules.length; index++) {
                if (s.length > 1) {
                    s += " " + group.groupOp + " ";
                }
                s += this.getStringForRule(group.rules[index]);
            }
        } catch (e) { alert(e); }
    }

    s += ")";

    if (s === "()") {
        return ""; // ignore groups that don't have rules
    }
    return s;
};
var getStringForRule = function (rule) {
    var opUF = "", opC =rule.op,  ret, val,     
    val = rule.data;
    if (opC === 'bw' || opC === 'bn') { val = val + "%"; }
    if (opC === 'ew' || opC === 'en') { val = "%" + val; }
    if (opC === 'cn' || opC === 'nc') { val = "%" + val + "%"; }
    if (opC === 'in' || opC === 'ni') { val = " (" + val + ")"; }
    var operands = { "eq": "=", "ne": "<>", "lt": "<", "le": "<=", "gt": ">", "ge": ">=", "bw": "LIKE", "bn": "NOT LIKE", "in": "IN", "ni": "NOT IN", "ew": "LIKE", "en": "NOT LIKE", "cn": "LIKE", "nc": "NOT LIKE", "nu": "IS NULL", "nn": "ISNOT NULL" };
    opUF = operands[opC];
    ret = rule.field + " " + opUF + " \"" + val + "\""; 
    return ret;
};