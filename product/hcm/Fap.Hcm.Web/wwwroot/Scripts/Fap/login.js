jQuery(function ($) {
    MultiLangHelper.initLang("@currentLang");
    $("#frm-register").validate({
        rules: {
            GrpManager: {
                required: true,
                email: true
            },
            GrpPassword: {
                required: true,
                minlength: 5
            },
            ConfirmPassword: {
                required: true,
                minlength: 5,
                equalTo: "#GrpPassword"
            },
            GrpFullName: {
                required: true
            },


        },
        messages: {
            GrpManager: {
                required: "请输入您的集团账号",
                email: "请检查电子邮件的格式"
            },
            GrpPassword: {
                required: "请输入密码",
                minlength: "密码长度不能小于 5 个字符"
            },
            ConfirmPassword: {
                required: "请输入确认密码",
                minlength: "密码长度不能小于 5 个字符",
                equalTo: "两次密码输入不一致"
            }, GrpFullName: {
                required: "请输入集团全称"
            },

        }, highlight: function (e) {
            $(e).closest('.form-control').next().addClass('red');
        },

        success: function (e) {
            $(e).prev().children(".ace-icon").removeClass('red');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        },


    });
    $('#frm-logon').validate({
        rules: {
            username: {
                required: true
            },
            userpwd: {
                required: true,
                minlength: 1
            }
        },

        messages: {
            username: {
                required: MultiLangHelper.getResName("login_username_required", "请输入你的用户名"),
            },
            userpwd: {
                required: MultiLangHelper.getResName("login_password_required", "请输入你的密码"),
                minlength: MultiLangHelper.getResName("login_password_level", "请指定一个安全密码")
            },

        },
        highlight: function (e) {
            $(e).closest('.form-control').next().addClass('red');
        },

        success: function (e) {
            $(e).prev().children(".ace-icon").removeClass('red');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            error.insertAfter(element.parent());
        },

    });
    //找回密码框切换
    $(document).on('click', '.toolbar a[data-target]', function (e) {
        e.preventDefault();

        var target = $(this).data('target');
        $('.widget-box.visible').removeClass('visible');//hide others
        $(target).addClass('visible');//show target
    });

    $(document).keydown(function (event) {
        var e = event || window.event;
        var k = e.keyCode || e.which;
        if (k == 13) {
            logon();
        }
    });
    //注册集团
    $("#btnregister").on('click', function (e) {
        if (!$('#frm-register').valid()) {
            e.preventDefault();
        } else {
            $("#frm-register").submit();
        }
    })
    $("#btnLogon").on('click', function (e) {
        logon();
    });
    function logon() {
        if (!$('#frm-logon').valid()) {
            e.preventDefault();
        } else {
            var returnUrl = getRequestParameter("ReturnUrl");
            var hash = window.location.hash;
            if (returnUrl !== undefined) {
                $("#frm-logon [name='returnUrl']").val("~"+returnUrl + hash);
            }
            $("#frm-logon").submit();
        }
    }
    $("#btnRecovery").on("click", function () {
        var email = $("#txtemail").val();
        if (email == "") {
            bootbox.alert("请输入公司邮箱！");
            return;
        }
        $.get("/Home/RecoveryPassword?mail=" + email, function (rv) {
            $("#pwdmsg").html(rv);
        })
    })
});
/**
 * 获取请求参数
 * @param key
 * @returns {*}
 */
function getRequestParameter(key) {
    var params = getRequestParameters();
    return params[key];
}

/**
 * 获取请求参数列表
 * @returns {{}}
 */
function getRequestParameters() {
    var arr = (location.search || "").replace(/^\?/, '').split("&");
    var params = {};
    for (var i = 0; i < arr.length; i++) {
        var data = arr[i].split("=");
        if (data.length == 2) {
            params[data[0]] = data[1];
        }
    }
    return params;
}