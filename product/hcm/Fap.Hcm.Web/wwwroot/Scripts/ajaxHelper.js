var AjaxHelper = {};

AjaxHelper.GetData = function(url, jsonStrData, errorMsg, callback) {
    $.ajax({
        url: url,
        data: jsonStrData,
        type: 'GET',
        cache: false,
        async: false,
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            if (callback) callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (!errorMsg || errorMsg == '') errorMsg = "AJAX请求出现出现异常";
            alert(errorMsg + "，其原因如下：" + textStatus);
        }
    });
};

AjaxHelper.PostData = function (url, jsonStrData, errorMsg, callback) {
    $.ajax({
        url: url,
        data: jsonStrData,
        type: 'POST',
        cache: false,
        async: false,
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            if (callback) callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (!errorMsg || errorMsg == '') errorMsg = "AJAX请求出现出现异常";
            alert(errorMsg + "，其原因如下：" + textStatus);
        }
    });
};

AjaxHelper.GetData2 = function(url, data, errorMsg, callback) {
    $.ajax({
        url: url,
        data: data,
        type: 'GET',
        cache: false,
        async: false,
        contentType: "text/html",
        success: function (data) {
            if (callback) callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (!errorMsg || errorMsg == '') errorMsg = "AJAX请求出现出现异常";
            alert(errorMsg + "，其原因如下：" + textStatus);
        }
    });
}

AjaxHelper.PostData2 = function(url, data, errorMsg, callback) {
    $.ajax({
        url: url,
        data: data,
        type: 'POST',
        cache: false,
        async: false,
        contentType: "text/html",
        success: function (data) {
            if (callback) callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (!errorMsg || errorMsg == '') errorMsg = "AJAX请求出现出现异常";
            alert(errorMsg + "，其原因如下：" + textStatus);
        }
    });
}