var forgetmodal = $('#modalForgetPassword');
var forgetform1 = forgetmodal.find('form');
var forgetform2 = forgetmodal.find('div.modal-password--result');
$(function () {
    forgetmodal.on('hidden.bs.modal', function (e) {
        forgetform1.find('p.input-error-info-signup').remove();
        forgetform1.find('input').val('');
        forgetFormPage(1);
        recaptchaResponseReset('#modalForgetPassword');
    });

    forgetFormPage(1);

    forgetform1.validate(validateParams);
    //forgetform1.find('input#loginEmailForget').rules("add", { accountexist: true });
    forgetform1.find('input#otpTelInput1').rules("add", { phonecheck: true });
    setRecaptchaRequired(forgetform1)
    forgetform1.submit(function () {
        if (!forgetform1.valid()) return false;
        var email = forgetform1.find('input#loginEmailForget').val();
        var datta = appendRecaptchaCode(forgetform1, {
            Email: email,
            Phone: forgetform1.find('input#otpTelInput1').val()
        });
        $ajax.Post(forgetPasswordUrl, datta, function (d) {
                recaptchaResponseReset();
                if (d.result) {
                forgetform2.find('p.modal-password--result--text span').html(email);
                forgetFormPage(2);
            }
            else {
                showMsg(d.message, 'error');
            }
        });
        return false;
    });

    forgetform2.find('button').click(function () {
        forgetmodal.modal('hide');
    });

});

var forgetFormPage = function (pageNo) {
    switch (pageNo) {
        case 1:
            forgetform2.hide();
            forgetform1.show(500);
            break;
        case 2:
            forgetform1.hide();
            forgetform2.show(500);
            break;
    }
}