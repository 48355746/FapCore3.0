/**
author:wyf
date:2017-09-05
description:

依赖jstree

**/
;
(function ($) {
   
    function initTree($this, options) {
        var ctrl = ["<div class=\"nav-search\"><span  class=\"input-icon comboboxTree\">",
"            <input type=\"text\" name=\"combotreename\"  class=\"nav-search-input\" />",
"            <i class=\"ace-icon fa fa-search nav-search-icon \" ></i>",
"            <input name=\"combotreevalue\"  class=\"hide form-control\" />",
"        </span></div>"].join("");
        var dropContent = ["    <div id=\"menuContent\" style=\"display: none; position: absolute;max-height:450px; z-index: 999999; padding: 5px; background: #fff; border: 1px solid #cccccc;\">",
    "            <div class=\"jstree\"></div>",
    "        </div>"].join("");
        var $ctrl = $(ctrl);
        var $dropContent = $(dropContent);

        $this.addClass("hide");
        $this.after($ctrl, $dropContent);
       
        var $jsTree = $this.parent().find('.jstree');
        var $comboTreeName = $this.parent().find("[name=combotreename]");
        $comboTreeName.attr("placeholder", options.placeholder).css("height",options.height+"px");
        var $comboTreeValue = $this.parent().find("[name=combotreevalue]");
        $jsTree.jstree('destroy', false);
        $jsTree.jstree({
            'core': {
                "check_callback": true,
                'force_text': true,
                "themes": { "stripes": true },
                'data': {
                    'url': function (node) {
                        return options.url;
                    }
                }
            }
        }).bind('select_node.jstree', function (event, data) {
            $comboTreeName.val(data.node.text);
            $comboTreeValue.val(data.node.id);
            if ($.isFunction(options.onValueChange)) {
                options.onValueChange.call($this, data.node.id, data.node.children_d);
            }
            //下拉关闭
            dropClose();
        }).on("after_open.jstree", function () {
            debugger
            $dropContent.trigger("mouseenter.ace_scroll");
        });
        $dropContent.ace_scroll({
            size: 400,
        });
        
        $ctrl.click(function () {
            $dropContent.css({ "min-width": $comboTreeName.outerWidth() + 10 + "px" }).slideDown("fast");
            document.addEventListener("mousedown", onBodyMouseDown, false);
        });
    }
    function dropClose() {
        $("#menuContent").fadeOut("fast");
        document.removeEventListener("mousedown", onBodyMouseDown, false);
    }
    function onBodyMouseDown(event) {
        if (!(event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
            dropClose();
        }
    }
    var methods = {
        init: function (options) {
            return this.each(function () {
                var $this = $(this);
                var settings = $this.data("plugsettings");
                if (typeof (settings) == 'undefined') {
                    var defaults = {
                        placeholder: "search",
                        height:30,
                        url: "",//获取tree的ajax url
                        onValueChange:false//值变化事件
                    };
                    settings = $.extend({}, defaults, options);
                    $this.data("plugsettings", settings);
                } else {
                    settings = $.extend({}, defaults, options);
                }

                initTree($this, settings);
            })
        },
        destroy: function () {
            return this.each(function () {
                var $this = $(this);
                $this.parent().find(".jstree").jstree('destroy', false);
                $this.parent().find(".comboboxTree").remove();
                $this.parent().find("#menuContent").remove();
                $this.removeData("plugsettings")
            });
        }

    }
    $.fn.combotree = function () {
        var method = arguments[0];
        if (methods[method]) {
            method = methods[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof method == 'object' || !method) {
            method = methods.init;
            arguments = Array.prototype.slice.call(arguments);
        } else {
            $.error('Method  ' + method + ' does not exist on jQuery.pluginName');
            return this;
        }
        return method.apply(this, arguments)

    }
})(jQuery)