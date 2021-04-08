var settingPasswordmodal = $('#modalSettingPassword');
var settingPasswordform1 = settingPasswordmodal.find('form');
var settingPasswordform2 = settingPasswordmodal.find('div.modal-setting-password--result');
$(function () {
    if (window.location.hash === "#settingpassword") {
        $('#modalSettingPassword').modal('show')
    }
    settingPasswordFormPage(1);

    settingPasswordform1.validate(validateParams);
    settingPasswordform1.find('input#signin-passwordSetting').rules("add", { pwdcheck: true });
    settingPasswordform1.find('input#signin-passwordSetting2').rules("add", { equalTo: "#signin-passwordSetting" });
    settingPasswordform1.submit(function () {
        if (!settingPasswordform1.valid()) return false;
        var email = settingPasswordform1.find('input#loginEmailForget').val();
        var datta = {
            Password: settingPasswordform1.find('input#signin-passwordSetting').val()
        };
        $ajax.Post(settingPasswordUrl, datta, function (d) {
            if (d.result) {
                settingPasswordFormPage(2);
                loginpath = d.url;
            }
            else {
                showMsg(d.message, 'error');
            }
        });
        return false;
    });

    var loginpath = '';
    settingPasswordform2.find('button').click(function () {
        //if (loginpath)
        //    window.location = loginpath;
        //else
        settingPasswordmodal.modal('hide');
        loginExt.open();
    });

});

var settingPasswordFormPage = function (pageNo) {
    switch (pageNo) {
        case 1:
            settingPasswordform2.hide();
            settingPasswordform1.show(500);
            break;
        case 2:
            settingPasswordform1.hide();
            settingPasswordform2.show(500);
            break;
    }
}