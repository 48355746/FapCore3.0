
//JS资源多语， 由系统自动生成， 请勿修改
var MultiLangHelper = (function () {
    var resLang = 'ZhCn';
    var resArray = { 
      'show_all': { 'ZhCn': '显示全部', 'ZhTW': '', 'En': '', 'Ja': '' },
      'move_selected': { 'ZhCn': '移动选中', 'ZhTW': '', 'En': '', 'Ja': '' },
      'filter': { 'ZhCn': '过滤', 'ZhTW': '', 'En': '', 'Ja': '' },
      'move_all': { 'ZhCn': '移动所有', 'ZhTW': '', 'En': '', 'Ja': '' },
      'remove_all': { 'ZhCn': '移除所有', 'ZhTW': '', 'En': '', 'Ja': '' },
      'remove_selected': { 'ZhCn': '移除选中', 'ZhTW': '', 'En': '', 'Ja': '' },
      'empty_list': { 'ZhCn': '空列表', 'ZhTW': '', 'En': '', 'Ja': '' },
      'from': { 'ZhCn': '从', 'ZhTW': '', 'En': '', 'Ja': '' },
      'filtered': { 'ZhCn': '过滤到', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_to_department': { 'ZhCn': '推送给部门', 'ZhTW': '', 'En': '', 'Ja': '' },
      'talent_pool': { 'ZhCn': '加优才库', 'ZhTW': '', 'En': '', 'Ja': '' },
      'resume_filter': { 'ZhCn': '简历筛选', 'ZhTW': '', 'En': '', 'Ja': '' },
      'reserve': { 'ZhCn': '加后备人才库', 'ZhTW': '', 'En': '', 'Ja': '' },
      'add_blacklist': { 'ZhCn': '加黑名单', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_to_interview': { 'ZhCn': '发送面试邮件', 'ZhTW': '', 'En': '', 'Ja': '' },
      'annex': { 'ZhCn': '附件', 'ZhTW': '', 'En': '', 'Ja': '' },
      'view': { 'ZhCn': '查看', 'ZhTW': '', 'En': '', 'Ja': '' },
      'save': { 'ZhCn': '保存', 'ZhTW': '', 'En': '', 'Ja': '' },
      'previous': { 'ZhCn': '上一条', 'ZhTW': '', 'En': '', 'Ja': '' },
      'next': { 'ZhCn': '下一条', 'ZhTW': '', 'En': '', 'Ja': '' },
      'resume': { 'ZhCn': '简历', 'ZhTW': '', 'En': '', 'Ja': '' },
      'assess': { 'ZhCn': '评估', 'ZhTW': '', 'En': '', 'Ja': '' },
      'information': { 'ZhCn': '提示信息', 'ZhTW': '', 'En': '', 'Ja': '' },
      'saveas_common_query': { 'ZhCn': '保存为常用查询', 'ZhTW': '', 'En': '', 'Ja': '' },
      'common_query': { 'ZhCn': '常用查询', 'ZhTW': '', 'En': '', 'Ja': '' },
      'allow_interview': { 'ZhCn': '允许面试', 'ZhTW': '', 'En': '', 'Ja': '' },
      'resume_assess': { 'ZhCn': '简历评估', 'ZhTW': '', 'En': '', 'Ja': '' }
    };
    var Helper = {};
    //初始化语种
    Helper.initLang = function (lang) {
        resLang = lang;
    };
    //获得资源多语
    Helper.getResName = function (code, defaultName) {
        if (resArray[code] === undefined) {
            $.post(basePath +'/Core/Api/MultiLanguage',{ langKey: code, langValue: defaultName }, function (rv) {

            });
            return defaultName;
        }
        return resArray[code][resLang] || defaultName;
    };

    return Helper;
})();