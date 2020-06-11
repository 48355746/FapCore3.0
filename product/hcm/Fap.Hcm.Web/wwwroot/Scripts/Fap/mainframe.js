//解决bootbox多弹出框focus 死循环问题
$.fn.modal.Constructor.prototype.enforceFocus = function () { };
$(function () {
    //使用说明
    $("#sidebar-shortcuts-large #btnHelper").on(ace.click_event, function () {
        var hash = window.location.hash;
        if (hash == "#") {
            return;
        }
        hash = hash.replace(/^(\#\!)?\#/, '');
        bootboxWindow($.lang("guide", "使用指南"), basePath + "/Component/Guide",
            {
                edit: {
                    label: $.lang("edit", "编辑"),
                    className: "btn-primary btn-link",
                    callback: function () {
                        editGuid();
                    }
                }

            }, { nav: hash });
    })
    $("#sidebar-shortcuts-large #btnChangeRole").on(ace.click_event, function () {
        var dialog = bootbox.dialog({
            message: 'loading',
            buttons: {
                success: {
                    label: $.lang("ok", "确定"),
                    className: "btn-primary",
                    callback: function () {
                        var roleUid = $('[name = "role-radio"]:checked').val();
                        buildMenus(roleUid);
                    }
                },
                cancel: {
                    label: $.lang("cancel", "取消"),
                    className: "btn-default",
                    callback: function () {

                    }
                }
            }
        });
        dialog.init(function () {
            var url = basePath + '/Home/RoleList';
            $.get(url, function (ev) {
                dialog.find('.bootbox-body').html(ev);
            });
        });
    })
    $("#btnClearCache").on(ace.click_event, function () {
        $.post(basePath + "/System/Api/Manage/Setting/ClearCache", function (rv) {
            if (rv.success) {
                $.msg(rv.msg);
            } else {
                bootbox.alert(rv.msg);
            }
        })
    })
    //setTimeout(function () {
    //    getHandling();
    //    getBadge();
    //}, 3000);
});

//获取待处理个数
var getHandling = function () {
    //获取待办数目
    $.ajax({
        url: basePath + "/Common/Home/GetHandling",    //请求的url地址
        dataType: "json",   //返回格式为json
        async: true, //请求是否异步，默认为异步，这也是ajax重要特性
        data: {},    //参数值
        type: "POST",   //请求方式
        beforeSend: function () {
            //请求前的处理
        },
        success: function (data) {
            //请求成功时处理
            var handling = data.Handling;
            var totals = data.Totals;
            var percent = 100.0;
            if (totals > 0) {
                percent = ((totals - handling) * 100.0 / totals).toFixed(2);
            }
            $(".handingcount").text(handling);
            $(".totalcount").text("总任务数:" + totals);
            $(".hanglingtxt").text(handling + '待处理任务');
            $(".pull-right.percent").text(percent + '%');
            $(".progress-bar.totalcount").css({ width: percent + '%' });
        },
        complete: function () {
            //请求完成的处理
        },
        error: function () {
            //请求出错处理
        }
    });
};
//获取菜单徽章
var getBadge = function () {
    $(".badge.badge-warning").each(function () {
        var plugin = $(this).data("plug");
        if (plugin !== '') {
            var badge = $(this);
            $.post(basePath + '/Common/Home/GetBadge', { pluginclass: plugin }, function (data) {
                if (data.count > 0) {
                    badge.text(data.count);
                } else {
                    badge.remove();
                }
            });
        }
    });
};
//加载我的伙伴,菜单，多语言日期控件
(function () {
    buildPartner();
    buildMenus('0');    
})();
function buildPartner() {
    $.get(basePath + "/SelfService/Api/Partner", function (rv) {
        if (rv.success) {
            if (rv.data != null) {
                $("#mypartner").tmpl(rv.data).appendTo(".profile-partner");
            }
        } else {
            $.msg("加载伙伴出错：" + rv.msg);
        }
    })
}
function buildMenus(roleUid) {
    if (roleUid !== "0") {
        $(".user-sysmenu .role").each(function () {
            var $this = $(this);
            if ($this.data("role") === roleUid) {
                $this.find("i").removeClass("fa-circle").addClass("fa-check")
            } else {
                $this.find("i").removeClass("fa-check").addClass("fa-circle");
            }
        });
    }
    $.get(basePath + "/Home/ChangeRole/" + roleUid, function (rv) {
        $(".nav.nav-list.menus").html(rv);
    })
}