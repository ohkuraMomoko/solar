$(function () {
    forgetEmailFunc.Init();
});

var forgetEmailFunc = new function () {
    var modal = $('#modalForgetEmail');
    var form1 = modal.find('form');
    var form2 = modal.find('div.modal-email--result');
    var self = this;

    self.Init = function () {
        self.FormPage(1);
        form2.find('button').click(function () { self.Hide() });
        form1Setting();
        return self;
    }

    var form1Setting = function () {
        form1.validate(validateParams);
        form1.find('input#inputForgetEmail').rules("add", { phonecheck: true });
        setRecaptchaRequired(form1);
        form1.submit(function () {
            if (!form1.valid()) return false;
            var datta = appendRecaptchaCode(form1, {
                Phone: form1.find('input#inputForgetEmail').val()
            });
            $ajax.Post(forgetEmail_ForgotAcctUrl, datta, function (d) {
                recaptchaResponseReset();
                if (d.result) {
                    form2.find('p.modal-email--result--text span').html(datta.Phone);
                    self.FormPage(2);
                }
                else {
                    showMsg(d.message, 'error');
                    form1.find('input#imagesCodeForgetEmail').focus().val('');
                }
            });
            return false;
        });
    }

    self.FormPage = function (pageNo) {
        switch (pageNo) {
            case 1:
                form2.hide();
                form1.show(500);
                break;
            case 2:
                form1.hide();
                form2.show(500);
                break;
        }
        return self;
    }
    self.Show = function () {
        modal.modal('show');
        return self;
    }
    self.Hide = function () {
        modal.modal('hide');
        return self;
    }
}