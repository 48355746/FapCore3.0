"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/onlineHub").build();

//上线通知
connection.on("Online", function (onlineUser) {    
    var fid = onlineUser.empUid;
    $(".profile-partner").find("[data-id='" + fid + "']").find(".user-status").removeClass("status-offline").addClass("status-online"); 
});
connection.on("Offline", function (onlineUser) {
    var fid = onlineUser.empUid;
    $(".profile-partner").find("[data-id='" + fid + "']").find(".user-status").removeClass("status-online").addClass("status-offline");
});
//伙伴申请通知
connection.on("PartnerApplyNotifications", function (applier) {
    $.msg(applier + "申请加您为伙伴");
});
//伙伴审核通知
connection.on("PartnerCheckNotifications", function (applier) {
    if (applier.reqState === "Agree") {
        $.msg(applier.empName + "同意了您的伙伴申请");
        var content = `<div class="profile-activity clearfix" data-id="`+applier.fid+`">
        <div>
            <img class="pull-left" alt="头像" src="~/Component/Photo/`+ applier.empPhoto + `">
                <span class="user bolder" href="#">                
                <span class="user-status status-online" title="online"></span>
                    `+ applier.empName + `
            </span>

            <div class="time">
                `+ applier.deptUidMC + `
            </div>
        </div>
        <div class="tools action-buttons">
            <a href="javascript:void(0)" data-chat="`+ applier.fid + `" data-emp="` + applie.empName +`" class="blue">
                <i class="ace-icon fa fa-comments bigger-125"></i>
            </a>
        </div>
    </div>`;
        $(".profile-partner").append(content);
    } else {
        $.msg(applier.empName + "拒绝了您的伙伴申请");
    }
});
connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});
var chatDialog;
$(".profile-partner").off(ace.click_event, "[data-chat]").on(ace.click_event, "[data-chat]", function () {
    var $this = $(this);
    var empName = $this.data("emp");
    chatDialog = bootbox.dialog({
        title: 'To: ' + empName,
        message: '<div class="chat"></div>',
        footer: false
    });
    chatDialog.init(function () {
        $.get(basePath + "/Content/chat/chat.html", function (ev) {
            chatDialog.find('.bootbox-body').html(ev);
            chatDialog.find("#sendButton").on(ace.click_event, function (event) {
                var user = $this.data("chat");
                var message = chatDialog.find("#userInputMessage").val();
                if (message === "") {
                    return;
                }
                connection.invoke("SendMessage", user, message).catch(function (err) {
                    return console.error(err.toString());
                });
                var chatContent = ` <div class="itemdiv dialogdiv">     
        <div class="body">
            <div class="time">
                <i class="ace-icon fa fa-clock-o"></i>
                <span class="green">`+ moment().locale('zh-cn').format('YYYY-MM-DD HH:mm:ss')+`</span>
            </div>

            <div class="name">
                <a href="#">我</a>
            </div>
            <div class="text">`+ message+`</div>
        </div>
    </div>`;
                chatDialog.find(".dialogs").append(chatContent);
                chatDialog.find("#userInputMessage").val("");
                event.preventDefault();
            });
        });
    });
});
connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    if (chatDialog === undefined) {
        $.msg(user + ":" + msg);
        return;
    }
    var chatContent = ` <div class="itemdiv dialogdiv">     
        <div class="body">
            <div class="time">
                <i class="ace-icon fa fa-clock-o"></i>
                <span class="green">`+ moment().locale('zh-cn').format('YYYY-MM-DD HH:mm:ss') + `</span>
            </div>

            <div class="name">
                <a href="#">`+ user +`</a>
            </div>
            <div class="text">`+ msg + `</div>
        </div>
    </div>`;
    chatDialog.find(".dialogs").append(chatContent);
});