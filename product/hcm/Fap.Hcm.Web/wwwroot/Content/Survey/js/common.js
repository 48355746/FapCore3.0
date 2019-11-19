function inArray(e, d) {
    var n = !1;
    return $.each(d,
    function(d, i) {
        return i === e ? (n = !0, !1) : void 0
    }),
    n
}
// require("common:widget/header/header.js"),
// require("common:widget/qa_header/qa_header.js"),
 require("common:widget/sidebar/sidebar.js"),
jQuery.browser = {},
function() {
    jQuery.browser.msie = !1,
    jQuery.browser.version = 0,
    navigator.userAgent.match(/MSIE ([0-9]+)./) && (jQuery.browser.msie = !0, jQuery.browser.version = RegExp.$1)
} (),
$(function() {
    $("body").on("click", ".cb",
    function(e) {
        e.preventDefault(),
        $(this).hasClass("checked") ? $(this).removeClass("checked") : $(this).addClass("checked")
    }),
    $("body").on("click", ".cb-radio",
    function(e) {
        return e.preventDefault(),
        $(this).hasClass("checked") ? !1 : ($(this).addClass("checked"), $(this).find("input").attr("checked", "true"), $(this).siblings(".cb-radio").removeClass("checked").find("input").removeAttr("checked"), void 0)
    }),
    $(".bce-menu-list-item").on("click",
    function() {
        var e = $(this).parent(".bce-menu-list");
        null !== e && void 0 !== e && (e.hasClass("bce-menu-list-open") ? e.removeClass("bce-menu-list-open") : ($(".bce-menu-list").removeClass("bce-menu-list-open"), e.addClass("bce-menu-list-open")))
    })
});
var alert = function(e, d, n) {
    var i = addShade(),
    l = document.body,
    a = newElem("div", "dialog-body", "bce-dialog"),
    o = newElem("div", null, "bce-dialog-head"),
    t = newElem("div", null, "bce-dialog-title");
    o.appendChild(t);
    var p = newElem("span"),
    c = document.createTextNode(d || "信息提示");
    p.appendChild(c),
    t.appendChild(p);
    var r = newElem("span", null, "close"),
    m = document.createTextNode("×");
    r.appendChild(m),
    t.appendChild(r),
    a.appendChild(o);
    var h = newElem("div", null, "bce-dialog-body-panel"),
    v = newElem("div", "dialog-msg"),
    u = document.createTextNode(e);
    v.appendChild(u),
    h.appendChild(v),
    a.appendChild(h);
    var C = newElem("div", null, "bce-dialog-foot-panel"),
    s = newElem("div", null, "bce-dialog-foot"),
    g = newElem("div", null, "bce-dialog-ok-btn bce-ui-button"),
    b = document.createTextNode("确定");
    g.appendChild(b),
    s.appendChild(g),
    C.appendChild(s),
    a.appendChild(C),
    l.appendChild(a);
    var w = function() {
        l.removeChild(i),
        l.removeChild(a)
    };
    g.onclick = function() {
        w(),
        n && n()
    },
    r.onclick = function() {
        w()
    }
},
alertTip = function(e, d, n) {
    var i = (addShade(), document.body),
    l = newElem("div", "dialog-body", "bce-dialog"),
    a = newElem("div", null, "bce-dialog-head"),
    o = newElem("div", null, "bce-dialog-title");
    a.appendChild(o);
    var t = newElem("span"),
    p = document.createTextNode(d || "信息提示");
    t.appendChild(p),
    o.appendChild(t),
    l.appendChild(a);
    var c = newElem("div", null, "bce-dialog-body-panel"),
    r = newElem("div", "dialog-msg"),
    m = document.createTextNode(e);
    r.appendChild(m),
    c.appendChild(r),
    l.appendChild(c);
    var h = newElem("div", null, "bce-dialog-foot-panel"),
    v = newElem("div", null, "bce-dialog-foot"),
    u = newElem("div", null, "bce-dialog-ok-btn bce-ui-button"),
    C = document.createTextNode("确定");
    u.appendChild(C),
    v.appendChild(u),
    h.appendChild(v),
    l.appendChild(h),
    i.appendChild(l),
    u.onclick = function() {
        n && n()
    }
},
alertRich = function(e, d, n, i, l) {
    var a = addShade(),
    o = document.body,
    t = newElem("div", "dialog-body-rich"),
    p = newElem("div", "dialog-title"),
    c = newElem("span"),
    r = document.createTextNode(d || "信息提示");
    c.appendChild(r),
    p.appendChild(c),
    t.appendChild(p);
    var m = newElem("div", "dialog-content-holder"),
    h = newElem("div", "dialog-alert-icon"),
    v = newElem("div", "dialog-sub-title");
    v.innerHTML = n;
    var u = newElem("div", "dialog-msg");
    u.innerHTML = e;
    var C = newElem("div", "dialog-ok-btn", "dialog-btn"),
    s = document.createTextNode(i || "确定");
    C.appendChild(s),
    m.appendChild(h),
    m.appendChild(v),
    m.appendChild(u),
    m.appendChild(C),
    t.appendChild(m),
    o.appendChild(t);
    var g = function() {
        o.removeChild(a),
        o.removeChild(t)
    };
    C.onclick = function() {
        g(),
        l && l()
    }
},
confirm = function(e, d, n, i) {
    var l = addShade(),
    a = document.body,
    o = newElem("div", "dialog-body", "bce-dialog"),
    t = newElem("div", null, "bce-dialog-head"),
    p = newElem("div", null, "bce-dialog-title");
    t.appendChild(p);
    var c = newElem("span"),
    r = document.createTextNode(d || "信息提示");
    c.appendChild(r),
    p.appendChild(c);
    var m = newElem("span", null, "close"),
    h = document.createTextNode("×");
    m.appendChild(h),
    p.appendChild(m),
    o.appendChild(t);
    var v = newElem("div", null, "bce-dialog-body-panel"),
    u = newElem("div", "dialog-msg"),
    C = document.createTextNode(e);
    u.appendChild(C),
    v.appendChild(u),
    o.appendChild(v);
    var s = newElem("div", null, "bce-dialog-foot-panel"),
    g = newElem("div", null, "bce-dialog-foot"),
    b = newElem("div", null, "bce-dialog-ok-btn bce-ui-button"),
    w = document.createTextNode("确定");
    b.appendChild(w);
    var E = newElem("div", null, "bce-dialog-cancel-btn bce-ui-button"),
    f = document.createTextNode("取消");
    E.appendChild(f),
    g.appendChild(b),
    g.appendChild(E),
    s.appendChild(g),
    o.appendChild(s),
    a.appendChild(o);
    var x = function() {
        a.removeChild(l),
        a.removeChild(o)
    };
    b.onclick = function() {
        x(),
        n && n()
    },
    E.onclick = function() {
        x(),
        i && i()
    },
    m.onclick = function() {
        x()
    }
},
prompt = function(e, d, n, i) {
    var l = addShade(),
    a = document.body,
    o = newElem("div", "dialog-body", "vip"),
    t = newElem("div", "dialog-title"),
    p = newElem("span"),
    c = document.createTextNode(d || "信息提示");
    p.appendChild(c),
    t.appendChild(p),
    o.appendChild(t);
    var r = newElem("div", "dialog-content-holder", "vip"),
    m = newElem("div", "dialog-msg"),
    e = e;
    e += '<p><input type="text" id="prompt-value" style="width:280px;"></p>',
    e += '<p style="position:absolute;"><span id="prompt-msg" class="text-danger"></span></p>',
    e += '<div style="height:10px;">&nbsp;</div>',
    e += '<img src="/static/common/widget/footer/weixin.jpg" style="width:140px;position:absolute;top:60px;right:20px;border-left:solid 1px #ddd;padding-left:10px;">',
    m.innerHTML = e;
    var h = newElem("div", "dialog-ok-btn", "dialog-btn vip"),
    v = document.createTextNode("确认");
    h.appendChild(v),
    r.appendChild(m),
    r.appendChild(h);
    var u = newElem("div", "dialog-cancel-btn", "dialog-btn"),
    C = document.createTextNode("取消");
    u.appendChild(C),
    r.appendChild(u),
    o.appendChild(r),
    a.appendChild(o);
    var s = function() {
        a.removeChild(l),
        a.removeChild(o)
    };
    h.onclick = function() {
        n(s)
    },
    u.onclick = function() {
        s(),
        i && i()
    },
    $("body").on("keydown", "#prompt-value",
    function(e) {
        13 == e.keyCode && n(s)
    }),
    $("#prompt-value")[0].focus()
},
payAlert = function(e, d) {
    var n = addShade(),
    i = document.body,
    l = newElem("div", "dialog-body"),
    a = newElem("div", "dialog-title"),
    o = newElem("span"),
    t = document.createTextNode("支付完成前不要关闭页面");
    o.appendChild(t),
    a.appendChild(o),
    l.appendChild(a);
    var p = newElem("div", "dialog-content-holder"),
    c = newElem("div", "dialog-msg"),
    r = document.createTextNode("请在新打开的页面完成支付后选择");
    c.appendChild(r),
    p.appendChild(c);
    var m = newElem("div", "dialog-ok-btn", "dialog-btn vip"),
    h = document.createTextNode("完成支付");
    m.appendChild(h),
    p.appendChild(m);
    var v = newElem("div", "dialog-cancel-btn", "dialog-btn"),
    u = document.createTextNode("遇到问题");
    v.appendChild(u),
    p.appendChild(v),
    l.appendChild(p),
    i.appendChild(l);
    var C = function() {
        i.removeChild(n),
        i.removeChild(l)
    };
    m.onclick = function() {
        C(),
        e && e()
    },
    v.onclick = function() {
        C(),
        d && d()
    }
},
vip = function(e, d, n) {
    var i = addShade(),
    l = document.body,
    a = newElem("div", "dialog-body", "vip"),
    o = newElem("div", "dialog-title"),
    t = newElem("span"),
    p = document.createTextNode("开通VIP提示");
    t.appendChild(p),
    o.appendChild(t),
    a.appendChild(o);
    var c = newElem("div", "dialog-content-holder", "vip"),
    r = newElem("div", "dialog-msg"),
    m = "<h4>尊敬的用户，您好：</h4>";
    m += '<p class="text-sub">' + e + "</p>",
    m += "<hr>",
    m += '<h4 class="vip-title text-center"><img src="/static/common/img/king.png"/>&nbsp;<span>VIP会员特权<span></h4>',
    m += '<ul class="text-sub vip-desc clearfix">',
    m += "<li>兼容测试支持TOP300以上</li>",
    m += "<li>支持所有机型自定义单选</li>",
    m += "<li>性能测试支持添加多个竞品</li>",
    m += "<li>遍历测试延长时间至15分钟</li>",
    m += "</ul>",
    r.innerHTML = m;
    var h = newElem("div", "dialog-ok-btn", "dialog-btn vip"),
    v = document.createTextNode("是的，我要开通");
    h.appendChild(v);
    var u = newElem("div", "dialog-cancel-btn", "dialog-btn"),
    C = document.createTextNode("不用，重新选择");
    u.appendChild(C),
    c.appendChild(r),
    c.appendChild(h),
    c.appendChild(u),
    a.appendChild(c),
    l.appendChild(a);
    var s = function() {
        l.removeChild(i),
        l.removeChild(a)
    };
    h.onclick = function() {
        s(),
        d && d()
    },
    u.onclick = function() {
        s(),
        n && n()
    }
},
newAlertRich = function(e, d, n, i, l) {
    var a = addShade(),
    o = document.body,
    t = newElem("div", "new-dialog-body-rich"),
    p = newElem("div", "new-dialog-title"),
    c = newElem("span"),
    r = document.createTextNode(d || "信息提示");
    c.appendChild(r),
    p.appendChild(c),
    t.appendChild(p);
    var m = newElem("div", "new-dialog-content-holder"),
    h = newElem("div", "new-dialog-success-icon"),
    v = newElem("div", "new-dialog-sub-title");
    v.innerHTML = n;
    var u = newElem("div", "new-dialog-msg");
    u.innerHTML = e;
    var C = newElem("div", "new-dialog-single-btn", "new-dialog-btn"),
    s = document.createTextNode(i || "确定");
    C.appendChild(s),
    m.appendChild(h),
    m.appendChild(v),
    m.appendChild(u),
    m.appendChild(C),
    t.appendChild(m),
    o.appendChild(t);
    var g = function() {
        o.removeChild(a),
        o.removeChild(t)
    };
    C.onclick = function() {
        g(),
        l && l()
    }
},
newElem = function(e, d, n) {
    var i = document.createElement(e);
    return d && (i.id = d),
    n && (i.className = n),
    i
},
addShade = function() {
    var e = document.body,
    d = newElem("div", "shade");
    return d.style.height = e.outerHeight + "px",
    e.appendChild(d),
    d
},
showImage = function(e) {
    var d = document.body,
    n = addShade(),
    i = newElem("div", "img-body"),
    l = newElem("a", "img-close"),
    a = newElem("div", "img-border"),
    o = newElem("img", "img-img");
    o.src = e,
    i.appendChild(a),
    i.appendChild(l),
    i.appendChild(o),
    d.appendChild(i);
    var t = function() {
        d.removeChild(n),
        d.removeChild(i)
    };
    l.onclick = function() {
        t()
    },
    n.onclick = function() {
        t()
    }
};