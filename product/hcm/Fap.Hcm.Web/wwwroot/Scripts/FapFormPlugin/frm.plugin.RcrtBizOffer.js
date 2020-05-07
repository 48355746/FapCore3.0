setTimeout(function () {

    $("[for=ResumeUid]").addClass("btn-link orange").text('简历(点击查看)').on(ace.click_event, function () {
        var resumeUid = $("#ResumeUid").val();
        if (resumeUid === '') {
            $.msg("未发现简历");
            return;
        }
        bootboxWindow($.lang("resume", "简历"), basePath + "/Component/DataForm/" + resumeUid,
            {
                assess: {
                    label: $.lang("view_interview_assess", "查看面试评价"),
                    className: "btn-primary btn-link",
                    callback: function () {
                        bootboxWindow($.lang("view_interview_assess", "查看面试评价"), basePath + "/Recruit/Manage/InterviewAssess", null, { resumeUid: resumeUid  });
                        return false;
                    }
                },
            }, { tn: "RcrtResume", fs: 3 });
    });
}, 0);