//jQuery.ajaxSetup({ "contentType": "application/json; charset=utf-8" });
var ajaxPost = function (url, data, isReturnJson, callback, isAysnc) {
    if (!data) data = {};
    if (!isAysnc) isAysnc = false;
    var dataType = "text";
    if (isReturnJson) dataType = "json";
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        async: isAysnc,
        cache: false,
        dataType: dataType,
        success: function (data) {
            callback && callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("获取数据出现异常，其原因如下：" + textStatus);
        }
    });
};

var ajaxGet = function (url, data, isReturnJson, callback, isAysnc) {
    if (!data) data = {};
    if (!isAysnc) isAysnc = false;
    var dataType = "text";
    if (isReturnJson) dataType = "json";
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        async: false,
        cache: false,
        dataType: dataType,
        success: function (data) {
            callback && callback(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert("获取数据出现异常，其原因如下：" + textStatus);
        }
    });
};

//获取表的字段
var getColumnList = function (tableName, callback) {
    if (!tableName) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/ColumnList", { tableName: tableName }, true, callback);
};

//获取字段的字典数据
var getDictDataOfColumn = function (tableName, columnName, callback) {
    if (!tableName || !columnName) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/DictDataOfColumn", { tableName: tableName, columnName: columnName }, true, callback);
};

//获取字段的参照数据
var getRefDataOfColumn = function (tableName, columnName, callback) {
    if (!tableName || !columnName) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/RefDataOfColumn", { tableName: tableName, columnName: columnName }, true, callback);
};

//获取字段的参照的表名
var GetRefTableNameOfColumn = function (tableName, columnName, callback) {
    if (!tableName || !columnName) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/GetRefTableNameOfColumn", { tableName: tableName, columnName: columnName }, true, callback);
};

//获取字典数据
var getDictData = function (dictCategory, callback) {
    if (!dictCategory) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/DictData", { dictCategory: dictCategory }, true, callback);
};

//获取参照数据
var getRefData = function (reftable, refid,refcode, refname, callback) {
    if (!reftable || !refid || !refname) return;
    ajaxGet("/api/FlowMgr/FlowTemplate/RefData", { reftable: reftable, refid: refid,refcode:refcode, refname: refname }, true, callback);
};
