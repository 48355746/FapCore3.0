
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
      'resume_assess': { 'ZhCn': '简历评估', 'ZhTW': '', 'En': '', 'Ja': '' },
      'select_row_empty': { 'ZhCn': '请选中数据操作', 'ZhTW': '', 'En': '', 'Ja': '' },
      'save_add': { 'ZhCn': '保存并新增', 'ZhTW': '', 'En': '', 'Ja': '' },
      'ok': { 'ZhCn': '确定', 'ZhTW': '', 'En': '', 'Ja': '' },
      'recruit_website': { 'ZhCn': '招聘网站', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_to_interview_notice': { 'ZhCn': '发送面试通知', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_interview_notice': { 'ZhCn': '发送面试通知', 'ZhTW': '', 'En': '', 'Ja': '' },
      'new_event': { 'ZhCn': '新建事件', 'ZhTW': '', 'En': '', 'Ja': '' },
      'cancel': { 'ZhCn': '取消', 'ZhTW': '', 'En': '', 'Ja': '' },
      'event_change': { 'ZhCn': '更改事件', 'ZhTW': '', 'En': '', 'Ja': '' },
      'event_delete': { 'ZhCn': '删除事件', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_to_interview_invite': { 'ZhCn': '面试邀约', 'ZhTW': '', 'En': '', 'Ja': '' },
      'clear': { 'ZhCn': '清空!', 'ZhTW': '', 'En': '', 'Ja': '' },
      'end_row': { 'ZhCn': '已到达最后一条数据', 'ZhTW': '', 'En': '', 'Ja': '' },
      'interview_assess': { 'ZhCn': '面试评价', 'ZhTW': '', 'En': '', 'Ja': '' },
      'score': { 'ZhCn': '打分', 'ZhTW': '', 'En': '', 'Ja': '' },
      'score': { 'ZhCn': '打分', 'ZhTW': '', 'En': '', 'Ja': '' },
      'score': { 'ZhCn': '打分', 'ZhTW': '', 'En': '', 'Ja': '' },
      'score': { 'ZhCn': '打分', 'ZhTW': '', 'En': '', 'Ja': '' },
      'recommend': { 'ZhCn': '推荐', 'ZhTW': '', 'En': '', 'Ja': '' },
      'my_recommend': { 'ZhCn': '我的推荐', 'ZhTW': '', 'En': '', 'Ja': '' },
      'form_validation_failed': { 'ZhCn': '表单校验失败', 'ZhTW': '', 'En': '', 'Ja': '' },
      'confirm_delete': { 'ZhCn': '确定要删除选中的数据吗?', 'ZhTW': '', 'En': '', 'Ja': '' },
      'submit': { 'ZhCn': '提交', 'ZhTW': '', 'En': '', 'Ja': '' },
      'temporary': { 'ZhCn': '暂存', 'ZhTW': '', 'En': '', 'Ja': '' },
      'flow_diagram': { 'ZhCn': '流程图', 'ZhTW': '', 'En': '', 'Ja': '' },
      'invalid': { 'ZhCn': '作废', 'ZhTW': '', 'En': '', 'Ja': '' },
      'invalid': { 'ZhCn': '作废', 'ZhTW': '', 'En': '', 'Ja': '' },
      'replace_photo': { 'ZhCn': '更换照片', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_to_offer_apply': { 'ZhCn': '发起offer审批', 'ZhTW': '', 'En': '', 'Ja': '' },
      'workflow_designer': { 'ZhCn': '流程设计器', 'ZhTW': '', 'En': '', 'Ja': '' },
      'recall': { 'ZhCn': '撤回', 'ZhTW': '', 'En': '', 'Ja': '' },
      'print': { 'ZhCn': '打印', 'ZhTW': '', 'En': '', 'Ja': '' },
      'close': { 'ZhCn': '关闭', 'ZhTW': '', 'En': '', 'Ja': '' },
      'handler': { 'ZhCn': '办理', 'ZhTW': '', 'En': '', 'Ja': '' },
      'success': { 'ZhCn': '授权成功！', 'ZhTW': '', 'En': '', 'Ja': '' },
      'view_interview_assess': { 'ZhCn': '查看面试评价', 'ZhTW': '', 'En': '', 'Ja': '' },
      'name': { 'ZhCn': '名称', 'ZhTW': '', 'En': '', 'Ja': '' },
      'code': { 'ZhCn': '编码', 'ZhTW': '', 'En': '', 'Ja': '' },
      'chang_photo': { 'ZhCn': '更换照片', 'ZhTW': '', 'En': '', 'Ja': '' },
      'summary': { 'ZhCn': '说明书', 'ZhTW': '', 'En': '', 'Ja': '' },
      'item_preparation': { 'ZhCn': '物品准备', 'ZhTW': '', 'En': '', 'Ja': '' },
      'send_offer_notice': { 'ZhCn': '发送Offer通知', 'ZhTW': '', 'En': '', 'Ja': '' },
      'bill_biz_required': { 'ZhCn': '单据实体和业务实体必选', 'ZhTW': '', 'En': '', 'Ja': '' },
      'check_information': { 'ZhCn': '信息核对', 'ZhTW': '', 'En': '', 'Ja': '' },
      'photo_updated': { 'ZhCn': '照片已更新', 'ZhTW': '', 'En': '', 'Ja': '' },
      'entry': { 'ZhCn': '入职', 'ZhTW': '', 'En': '', 'Ja': '' }
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