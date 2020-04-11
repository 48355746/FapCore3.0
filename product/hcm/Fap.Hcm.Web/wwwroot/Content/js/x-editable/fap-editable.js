/**
Address editable input.
Internally value stored as {city: "Moscow", street: "Lenina", building: "15"}

@class address
@extends abstractinput
@final
@example
<a href="#" id="address" data-type="address" data-pk="1">awesome</a>
<script>
$(function(){
    $('#address').editable({
        url: '/post',
        title: 'Enter city, street and building #',
        value: {
            city: "Moscow", 
            street: "Lenina", 
            building: "15"
        }
    });
});
</script>
**/
(function ($) {
    "use strict";

    var Address = function (options) {
        this.init('address', options, Address.defaults);
    };

    //inherit from Abstract input
    $.fn.editableutils.inherit(Address, $.fn.editabletypes.abstractinput);

    $.extend(Address.prototype, {
        /**
        Renders input from tpl

        @method render() 
        **/
        render: function () {
            this.$input = this.$tpl.find('input');
        },

        /**
        Default method to show value in element. Can be overwritten by display option.
        
        @method value2html(value, element) 
        **/
        value2html: function (value, element) {
            if (!value) {
                $(element).empty();
                return;
            }
            var html = $('<div>').text(value.city).html() + ', ' + $('<div>').text(value.street).html() + ' st., bld. ' + $('<div>').text(value.building).html();
            $(element).html(html);
        },

        /**
        Gets value from element's html
        
        @method html2value(html) 
        **/
        html2value: function (html) {
            /*
              you may write parsing method to get value by element's html
              e.g. "Moscow, st. Lenina, bld. 15" => {city: "Moscow", street: "Lenina", building: "15"}
              but for complex structures it's not recommended.
              Better set value directly via javascript, e.g. 
              editable({
                  value: {
                      city: "Moscow", 
                      street: "Lenina", 
                      building: "15"
                  }
              });
            */
            return null;
        },

        /**
         Converts value to string. 
         It is used in internal comparing (not for sending to server).
         
         @method value2str(value)  
        **/
        value2str: function (value) {
            var str = '';
            if (value) {
                for (var k in value) {
                    str = str + k + ':' + value[k] + ';';
                }
            }
            return str;
        },

        /*
         Converts string to value. Used for reading value from 'data-value' attribute.
         
         @method str2value(str)  
        */
        str2value: function (str) {
            /*
            this is mainly for parsing value defined in data-value attribute. 
            If you will always set value by javascript, no need to overwrite it
            */
            return str;
        },

        /**
         Sets value of input.
         
         @method value2input(value) 
         @param {mixed} value
        **/
        value2input: function (value) {
            if (!value) {
                return;
            }
            this.$input.filter('[name="city"]').val(value.city);
            this.$input.filter('[name="street"]').val(value.street);
            this.$input.filter('[name="building"]').val(value.building);
        },

        /**
         Returns value of input.
         
         @method input2value() 
        **/
        input2value: function () {
            return {
                city: this.$input.filter('[name="city"]').val(),
                street: this.$input.filter('[name="street"]').val(),
                building: this.$input.filter('[name="building"]').val()
            };
        },

        /**
        Activates input: sets focus on the first field.
        
        @method activate() 
       **/
        activate: function () {
            this.$input.filter('[name="city"]').focus();
        },

        /**
         Attaches handler to submit form in case of 'showbuttons=false' mode
         
         @method autosubmit() 
        **/
        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    Address.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div class="editable-address"><label><span>City: </span><input type="text" name="city" class="input-small"></label></div>' +
             '<div class="editable-address"><label><span>Street: </span><input type="text" name="street" class="input-small"></label></div>' +
             '<div class="editable-address"><label><span>Building: </span><input type="text" name="building" class="input-mini"></label></div>',

        inputclass: ''
    });

    $.fn.editabletypes.address = Address;

}(window.jQuery));


/**
参照编辑框
Refrence editable input.
Internally value stored as {city: "Moscow", street: "Lenina", building: "15"}

@class address
@extends abstractinput
@final
@example
<a href="#" id="refrence" data-type="refrence" data-pk="1"></a>
<script>
$(function(){
    $('#refrence').editable({
        url: '/post',
        title: '参照名称 #',
        value: {
            code: "Moscow", 
            name: "Lenina", 
        }
    });
});
</script>
**/
(function ($) {
    "use strict";

    var Reference = function (options) {
        this.init('reference', options, Reference.defaults);
        options.reference = options.reference || {};
        this.options.title = options.title || "";
        this.options.reference = $.extend({}, Reference.defaults.reference, options.reference);
    };

    //inherit from Abstract input
    $.fn.editableutils.inherit(Reference, $.fn.editabletypes.abstractinput);

    $.extend(Reference.prototype, {
        /**
        Renders input from tpl
         refoption: {
            tablename: '',//表名
            displaycols: '',//显示列
            valuecolname: '',//值字段
            namecolname:'',//名称字段
            condition:''//条件
        },
        @method render() 
        **/
        render: function () {
            this.setClass();
            this.$input = this.$tpl.find('input');

            //设置弹出参照框的显示头样式
            $.widget("ui.dialog", $.extend({}, $.ui.dialog.prototype, {
                _title: function (title) {
                    var $title = this.options.title || '&nbsp;'
                    if (("title_html" in this.options) && this.options.title_html == true)
                        title.html($title);
                    else title.text($title);
                }
            }))
            this.$input.next().on(ace.click_event, $.proxy(function () {
                //bootbox.alert(JSON.stringify(this.options));
                var tablename = this.options.reference.tablename;
                var colname = this.options.reference.colname;
                var refurl = this.options.reference.reftype;
                var multi = this.options.reference.multi;
                var refid = this.options.reference.refid;
                var component = this.options.reference.component;
                var $input = this.$input;
                var $dialogReference = $('#x-editable-dialogreference-' + tablename + colname);
                if ($dialogReference.length == 0) { //如果不存在，则创建参照对话框
                    var html = "<div id=\"x-editable-dialogreference-" + tablename + colname + "\" class=\"hide\">";
                    html += "   <div class=\"row\">  <div id=\"refContent-" + tablename + colname + "\" class=\"col-lg-12\">";
                    html += "   </div></div>";
                    html += "</div>"
                    this.$input.after($(html));
                    $dialogReference = $('#x-editable-dialogreference-' + tablename + colname);
                }
                var dialog = $dialogReference.removeClass('hide').dialog({
                    modal: true,
                    height: 'auto', width: 600, maxHeight: $(window).innerHeight() - 100, minHeight: 300,
                    title: "<div class='widget-header widget-header-small'><h4 class='smaller'><i class='ace-icon fa fa-search'></i> " + this.options.title + "</h4></div>",
                    title_html: true,
                    open: function () {
                        $("#refContent-" + tablename + colname).html("<h3 class=' smaller lighter grey'><i class='ace-icon fa fa-spinner fa-spin orange bigger-125'></i>正在加载，请稍后...</h3>");
                        var url = basePath + '/Component/' + refurl + '/' + refid;//+ '?frmid=' + tablename + '&ctrlid=' + colname;
                        if (component != '') {
                            url = basePath + '/Component/' + refurl + '/?fid=' + component;
                            if (multi == true) {
                                url = url + "&multi=1";
                            } else {
                                url = url + "&multi=0";
                            }
                        }
                        $("#refContent-" + tablename + colname).load(url);
                    },
                    buttons: [
                        {
                            text: "确认", "class": "btn btn-primary btn-xs",
                            click: function () {
                                var res = GetRefResult('frm-' + tablename);
                                if (res) {
                                    $input.val(res.name);
                                    $input.data("code", res.code);
                                }
                                else { bootbox.alert("请选择一条数据！"); return; }

                                $(this).dialog("close");
                            }
                        }, {
                            text: "取消", "class": "btn btn-xs",
                            click: function () {
                                //$("#refContent-" + tn).html("");
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            }, this));

            //加载前，需要获取参照对话框

            //var $dialogReference = $('#dialog-reference-' + frmid + ctrlid);
            //if ($dialogReference.length == 0) { //如果不存在，则创建参照对话框
            //    var html = "<div id=\"dialog-reference-" + frmid + ctrlid + "\" class=\"hide\">";
            //    html += "   <div class=\"row\">  <div id=\"refContent-" + frmid + ctrlid + "\" class=\"col-lg-12\">";
            //    html += "   </div></div>";
            //    html += "</div>"
            //    $('#fapFormContent-' + frmid).append($(html));
            //    $dialogReference = $('#dialog-reference-' + frmid + ctrlid);
            //}



        },

        /**
        Default method to show value in element. Can be overwritten by display option.
        
        @method value2html(value, element) 
        **/
        value2html: function (value, element) {
            if (!value) {
                $(element).empty();
                return;
            }
            var html = $('<div>').text(value.name).html();//+ ', ' + $('<div>').text(value.code).html();
            $(element).html(html);
        },

        /**
        Gets value from element's html
        
        @method html2value(html) 
        **/
        html2value: function (html) {
            /*
              you may write parsing method to get value by element's html
              e.g. "Moscow, st. Lenina, bld. 15" => {city: "Moscow", street: "Lenina", building: "15"}
              but for complex structures it's not recommended.
              Better set value directly via javascript, e.g. 
              editable({
                  value: {
                      city: "Moscow", 
                      street: "Lenina", 
                      building: "15"
                  }
              });
            */
            return null;
        },

        /**
         Converts value to string. 
         It is used in internal comparing (not for sending to server).
         
         @method value2str(value)  
        **/
        value2str: function (value) {
            var str = '';
            if (value) {

                str = str + value.name + ':' + value.code + ';';

            }
            return str;
        },

        /*
         Converts string to value. Used for reading value from 'data-value' attribute.
         
         @method str2value(str)  
        */
        str2value: function (str) {
            /*
            this is mainly for parsing value defined in data-value attribute. 
            If you will always set value by javascript, no need to overwrite it
            */
            return str;
        },

        /**
         Sets value of input.
         
         @method value2input(value) 
         @param {mixed} value
        **/
        value2input: function (value) {
            if (!value) {
                return;
            }
            this.$input.val(value.name);
            this.$input.data("code", value.code);
        },

        /**
         Returns value of input.
         
         @method input2value() 
        **/
        input2value: function () {
            return {
                code: this.$input.data("code"),
                name: this.$input.val()
            };
        },

        /**
        Activates input: sets focus on the first field.
        
        @method activate() 
        **/
        activate: function () {
            this.$input.focus();
        },

        /**
         Attaches handler to submit form in case of 'showbuttons=false' mode
         
         @method autosubmit() 
        **/
        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    Reference.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div class="input-group">' +
            '<input class="form-control" type="text" />' +
            '<span class="input-group-addon"><i class="ace-icon fa fa-search"></i></span>' +
            '</div>',
        reference: {
            tablename: '',//表名
            colname: '',//列名 
            multi: false,//是否多选
            reftype: '',//参照类型
            refid: 0,//参照id
            component: ''//组件Fid
        },
        inputclass: '',
        title: ''
    });

    $.fn.editabletypes.reference = Reference;

}(window.jQuery));