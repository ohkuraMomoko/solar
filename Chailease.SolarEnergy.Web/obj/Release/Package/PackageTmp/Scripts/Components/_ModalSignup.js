jQuery(document).ready(function ($) {
    //$('.cd-tabs-navigation a[data-content="login"]').click(() => {
    //});
    $('.cd-tabs-navigation a[data-content="signup"]').click(function () {
        $ajax.Post(componentsInitUrl, {}, function (d) {
            var slt = signForm.find('#sourceSelectSignup');
            slt.html('').append('<option value="" selected="" disabled="">如何得知全民電廠</option>');
            if (d.source) {
                $.each(d.source, function (i, v) { $('<option></option>', { value: v.key }).html(v.value).appendTo(slt) });
            }
        });
    });

    var tabItems = $('.cd-tabs-navigation a'),
        tabContentWrapper = $('.cd-tabs-content');

    tabItems.on('click', function (event) {
        event.preventDefault();
        var selectedItem = $(this);
        if (!selectedItem.hasClass('selected')) {
            var selectedTab = selectedItem.data('content'),
                selectedContent = tabContentWrapper.find('li[data-content="' + selectedTab + '"]'),
                slectedContentHeight = selectedContent.innerHeight();

            tabItems.removeClass('selected');
            selectedItem.addClass('selected');
            selectedContent.addClass('selected').siblings('li').removeClass('selected');
            //animate tabContentWrapper height when content changes
            tabContentWrapper.animate({
                'height': slectedContentHeight
            }, 200);

            //URL: 切換頁籤增加對應url
            if (selectedTab === "login") {
                window.location.hash = "login";
            } else {
                window.location.hash = "signup";
            }

        }
    });

    //hide the .cd-tabs::after element when tabbed navigation has scrolled to the end (mobile version)
    checkScrolling($('.cd-tabs nav'));
    $(window).on('resize', function () {
        checkScrolling($('.cd-tabs nav'));
        tabContentWrapper.css('height', 'auto');
    });
    $('.cd-tabs nav').on('scroll', function () {
        checkScrolling($(this));
    });

    function checkScrolling(tabs) {
        var totalTabWidth = parseInt(tabs.children('.cd-tabs-navigation').width()),
            tabsViewport = parseInt(tabs.width());
        if (tabs.scrollLeft() >= totalTabWidth - tabsViewport) {
            tabs.parent('.cd-tabs').addClass('is-ended');
        } else {
            tabs.parent('.cd-tabs').removeClass('is-ended');
        }
    }

    /* URL: 判斷url是註冊or登入*/
    if (window.location.hash === "#login") {
        $("#modalMember").modal('show');
    } else if (window.location.hash === "#signup") {
        $("#modalMember").modal('show');
        $('.cd-tabs-navigation a[data-content="signup"]').click();
    }

    
    let isMobile = false;
    $('#modalMember').on('shown.bs.modal', function (e) {
        let action;
        if (typeof $(e.relatedTarget).data('action') === "undefined"){
            if ($("#mobileLoginBtn").data("action") === "login") action = "login";
            if ($("#mobileSignBtn").data("action") === "signup") action = "signup";
            isMobile = true;;
        }else {
            action = $(e.relatedTarget).data("action");
        }
        
        //console.log(action);
        if (window.location.hash === "" || window.location.hash === "#") {
            window.location.hash = "login";
            $('.cd-tabs-navigation a[data-content="login"]').click();

            if ($("#loginStep1").css("display") === "none") {
                loginFormPage(1);
            }
            // setTimeout(function () {
            //     $(".cd-tabs-content").animate({ 'height': '474px' }, 200);
            // }, 500);
        }

        if (action === "signup") {
            $("#modalMember").modal('show');
            $('.cd-tabs-navigation a[data-content="signup"]').click();
            // setTimeout(function () {
            //     $(".cd-tabs-content").animate({ 'height': '880px' }, 200);
            // }, 500);

            // GA: 集客/註冊步驟1/PV
            dataLayer.push(
                {
                    "event": "popupPageView",
                    "popPATH": "/signup/userdata",
                    "popTitle": "會員註冊-Step1-基本資料"
                }
            );

        }
        $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
    });

    /* URL:開關MODAL時，加減對應HASH值*/
    $('#modalMember').on('hidden.bs.modal', function (e) {
        window.location.hash = "";
        if (isMobile){
            $("#mobileLoginBtn").data("action", "");
            $("#mobileSignBtn").data("action", "");
        }
        //GA: 集客 / popup關閉時
        dataLayer.push(
            {
                "event": "popupClosed",
            }
        );
    });
});

/***********************************************************************************************************************/

var memberModal = $('#modalMember');
var loginForm = memberModal.find('li[data-content=login] form');
var signForm = $('#signFormStep1');
var signForm2 = $('#signFormStep2');
var signForm3 = $('#signFormStep3');
var signFormSocial = $("#signFormStepSocial");

$(function () {

    memberModal.on('hidden.bs.modal', function (e) {
        memberModal.find('p.input-error-info-signup').remove();
        memberModal.find('input,select').val('');
        memberModal.find('input[type=checkbox]').prop("checked", false);
        signFormPage(1);
        recaptchaResponseReset();
        loginExt.refresh();
        countdownClose();
    });

    memberModal.find('form').submit(function () { return false; });

    //login
    loginForm.validate(validateParams);
    setRecaptchaRequired(loginForm);
    loginForm.submit(function () {
        if (!$(this).valid()) {
            $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            return false
        };
        var email = loginForm.find('#loginEmail').val();
        var pwd = loginForm.find('#loginPassword').val();

        var datta = appendRecaptchaCode(loginForm, {
            Email: email,
            Password: pwd
        });

        var backUrl = getQueryStrByName('Back');
        if (backUrl) datta['Back'] = backUrl;

        $ajax.Post(loginUrl, datta, function (d) {
            recaptchaResponseReset();
            if (d.result) {
                loginExt.setBackUrl(d.BackURL).loginSucc();
                $.ajax({
                    url: getUserInfoUrl,
                    type: "POST",
                    dataType: 'json',
                    cache: false,
                    data: {},
                    success: function (meminfo) {
                        // meminfo: ID , LEVEL , BASEINFO , ACCTION , RETURN_NOTE
                        if (meminfo.ID !== null) {
                            //console.log("login success");
                            //console.log('userID:' + meminfo.ID);

                            // GA DataLayer
                            window.dataLayer = window.dataLayer || [];

                            // GA: 集客/UserId
                            dataLayer.push({ 'userID': meminfo.ID });

                            // GA: 圖靈/UserID
                            dataLayer.push({ "event": "send_UserID", "UserID": meminfo.ID });	

                            // GA: 圖靈/身份別
                            // 會員權限 00:非會員 01:一般會員 02:出資會員(審核中) 03:出資會員(完成實名認證)
                            dataLayer.push({ 'event': 'td_identityCheck', 'td_identity': meminfo.LEVEL });
                        }
                    },
                    error: function (errmsg) {
                        console.log("getUserInfoUrl error");
                    }
                });
            }
            else {
                showMsg(d.message, 'error');
                loginForm.find('#loginPassword').val('');
            }
        });
        return false;
    });

    var new_signForm = $("#signFormStep1Form");
    //sign
    signFormPage(1);
    //sign form1
    new_signForm.find('input#sourceEtcSignup').hide();
    new_signForm.validate(validateParams);
    new_signForm.find('input#signin-password').rules("add", { pwdcheck: true });
    new_signForm.find('input#signin-password2').rules("add", { equalTo: "#signin-password", messages: { equalTo: '請確認是否與輸入密碼相符' } });
    //signForm.find('input#signin-recommender').rules("add", { recommender: true });
    new_signForm.find('input#signin-email').rules("add", { hasaccount: true, messages: { hasaccount: '該帳號已被註冊' } });
    //signForm.find('#modalSignupRawCheakbox').rules("add", { required: true, messages: { required: '' } });
    setRecaptchaRequired(new_signForm);
    //signForm.find('select#sourceSelectSignup').change(function () {
    //    var etc = signForm.find('input#sourceEtcSignup');
    //    if ($(this).val() == 99)
    //        etc.attr('required', 'required').show();
    //    else
    //        etc.removeAttr('required').hide();
    //});
    new_signForm.submit(function () {
        if (!$(this).valid()) {
            $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            return false
        };
        var datta = appendRecaptchaCode(signForm, getSignFormData());
        $ajax.Post(tempRegistUrl, datta, function (d) {
            recaptchaResponseReset();
            if (d.result) {
                signFormPage(2);
                // GA: 集客/註冊步驟2/成功
                dataLayer.push(
                    {
                        "event": "popupPageView",
                        "popPATH": "/signup/otp",
                        "popTitle": "會員註冊-Step2-簡訊驗證"
                    }
                );

                // GA: 圖靈/1.下一步點擊/成功
                dataLayer.push({
                    'event': 'td_normalMemberRegister',
                    'register_step': '1',
                    'register_step_status': 'success'
                });
            }
            else {
                signForm.find('input#imagesCodeSigbup').val('');
                signCaptchaPageRefresh();
                showMsg(d.message, 'error');
                // GA: 圖靈/1.下一步點擊/失敗
                dataLayer.push({
                    'event': 'td_normalMemberRegister',
                    'register_step': '1',
                    'register_step_status': 'fail'
                });
            }
        });
        $('.cd-tabs-content').css('height', 'auto');
        return false;
    });

    var recommender = getQueryStrByName('invite');
    if (recommender) signForm.find('input#signin-recommender').val(recommender);

    //sign form2
    signForm2.validate(validateParams);
    signForm2.find('input#otpTelInput').rules("add", { phonecheck: true });
    signForm2.find('input#otpCodeInput').rules("add", { digits: true });
    signForm2.find('input#otpTelInput').parent().find('div.show-resend-time').hide();  //btn-sendotpcode,show-resend-time
    signForm2.find('input#otpTelInput').parent().find('div.btn-sendotpcode').click(SendVerificationCodeCkick);

    signForm2.submit(function () {
        if (!$(this).valid()) {
            $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            return false
        };
        var message = signForm2.find('input#otpCodeInput');

        $ajax.Post(verifyRegPhoneUrl, { Message: message.val() }, function (d) {
            if (d.result) {
                $ajax.Post(registerUrl, {}, function (r) {
                    if (r.result) {
                        // GA: 集客/註冊步驟3/成功
                        dataLayer.push(
                            {
                                "event": "popupPageView",
                                "popPATH": "/signup/finish",
                                "popTitle": "會員註冊-Step3-完成"
                            }
                        );

                        // GA: 圖靈/2.簡訊驗證完成/成功
                        dataLayer.push({
                            'event': 'td_normalMemberRegister',
                            'register_step': '2',
                            'register_step_status': 'success'
                        });

                        if (r.email == "") {
                            signForm3.find('p.signup-success--text').hide();
                        } else {
                            //需求單20191107409090 註冊完不要發驗證EMAIL
                            //signForm3.find('p.signup-success--text span').html(r.email);
                            signForm3.find('p.signup-success--text').hide();
                        }                        
                        MenuRefresh();
                        signFormPage(3);
                    }
                    else {
                        showMsg(d.message, 'error');

                        // GA: 圖靈/2.簡訊驗證完成/失敗
                        dataLayer.push({
                            'event': 'td_normalMemberRegister',
                            'register_step': '2',
                            'register_step_status': 'fail'
                        });
                    }
                });
            }
            else {
                showMsg(d.message, 'error');
                countdownClose();
            }
        });
        return false;
    });

    $('div.modal-login-forget').click(function () {
        $("#modalMember").modal('hide');
        $('#modalForgetPassword').modal('show');
    });
    $('div.modal-login-email').click(function () {
        $("#modalMember").modal('hide');
        $('#modalForgetEmail').modal('show');
    });
});

var signFormPage = function (pageNo) {
    switch (pageNo) {
        case 1:
            signForm3.hide();
            signForm2.hide();
            signFormSocial.hide();
            signForm.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
        case 2:
            signForm3.hide();
            signForm.hide();
            signFormSocial.hide();
            signForm2.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
        case 3:
            signForm.hide();
            signForm2.hide();
            signFormSocial.hide();
            signForm3.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            // GA: 圖靈/3.一般會員申請完成頁面/成功
            dataLayer.push({
                'event': 'td_normalMemberRegister',
                'register_step': '3',
                'register_step_status': 'success'
            });
            break;
        case 4:
            signForm3.hide();
            signForm.hide();
            signForm2.hide();
            signFormSocial.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
    }
}

var loginStep1 = $("#loginStep1");
var loginStep2B = $("#loginStep2B");
var loginStep2C1 = $("#loginStep2C1");
var loginStep2C2 = $("#loginStep2C2");

var loginFormPage = function (pageNo) {
    switch (pageNo) {
        case 1:
            loginStep2B.hide();
            loginStep2C1.hide();
            loginStep2C2.hide();
            loginStep1.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
        case 2:
            loginStep1.hide();
            loginStep2C1.hide();
            loginStep2C2.hide();
            loginStep2B.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
        case 3:
            loginStep2B.hide();
            loginStep1.hide();
            loginStep2C2.hide();
            loginStep2C1.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
        case 4:
            loginStep2B.hide();
            loginStep2C1.hide();
            loginStep1.hide();
            loginStep2C2.show(500, function () {
                $('.cd-tabs-content').animate({ 'height': $('.cd-tabs-content').find('li[class="selected"]').innerHeight() }, 200);
            });
            break;
    }
}

var getSignFormData = function () {
    return {
        Source: signForm.find('select#sourceSelectSignup').val(),
        Captcha: signForm.find('input#imagesCodeSigbup').val(),
        Email: signForm.find('input#signin-email').val(),
        Password: signForm.find('input#signin-password').val(),
        PasswordConfirm: signForm.find('input#signin-password2').val(),
        Recommender: signForm.find('input#signin-recommender').val(),
        SourceEtc: signForm.find('input#sourceEtcSignup').val(),
        HasNotifyLetter: signForm.find('input#modalHasNotifyLetterCheakbox:checked').val() == 'on' ? true : false
    };
}
var SendVerificationCodeCkick = function () {
    var phoneNo = signForm2.find('input#otpTelInput');
    var sendBtn = $(this);;
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    if (!phoneNo.valid()) return false;
    $ajax.Post(getRegPhoneUrl, { PhoneNo: phoneNo.val() }, function (d) {
        if (d.result) {
            countdowndisplay.show();
            sendBtn.hide().unbind('click');
            startTimer(d.RES_SEC, signForm2.find('span#countdown'), function () {
                countdowndisplay.hide();
                sendBtn.show().click(SendVerificationCodeCkick);
                countdownClose();
            });
        }
        else {
            showMsg(d.message, 'error');
        }
    });
}

var SendVerificationCodeCkickForloginStep2C1 = function () {
    var phoneNo = loginStep2C1.find('input#otpTelInputLogin2C1');
    var sendBtn = $(this);;
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    if (!phoneNo.valid()) return false;
    countdowndisplay.show();
    $ajax.Post(getRegPhoneBindUrl, { PhoneNo: phoneNo.val() }, function (d) {
        if (d.result) {
            if (d.errorCode === "-2") {
               
            } else {
               //showMsg(d.message, 'error');
            }
        }
        else {
            showMsg(d.message, 'error');
        }
        countdowndisplay.show();
        sendBtn.hide().unbind('click');
        startTimer(d.RES_SEC, loginStep2C1.find('span#countdown'), function () {
            countdowndisplay.hide();
            sendBtn.show().click(SendVerificationCodeCkickForloginStep2B);

            countdownByloginStep2C1();
        });
    });
}

function countdownByloginStep2C1() {
    stopTimer();
    var phoneNo = loginStep2C1.find('input#otpTelInputLogin2C1');
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    var sendBtn = phoneNo.parent().find('div.btn-sendotpcode');
    countdowndisplay.hide();
    sendBtn.show().unbind('click').click(SendVerificationCodeCkickForloginStep2C1);
}

var SendVerificationCodeCkickForloginStep2B = function () {
    var phoneNo = loginStep2B.find('input#otpTelInputLogin2B');
    var sendBtn = $(this);;
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    if (!phoneNo.valid()) return false;
    $ajax.Post(getRegPhoneUrl, { PhoneNo: phoneNo.val() }, function (d) {
        if (d.result) {
            
        }
        else {
            showMsg(d.message, 'error');
        }

        countdowndisplay.show();
        sendBtn.hide().unbind('click');
        startTimer(d.RES_SEC, loginStep2B.find('span#countdown'), function () {
            countdowndisplay.hide();
            sendBtn.show().click(SendVerificationCodeCkickForloginStep2B);
            countdownByloginStep2B();
        });
    });
}


function countdownByloginStep2B() {
    stopTimer();
    var phoneNo = loginStep2C1.find('input#otpTelInputLogin2B');
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    var sendBtn = phoneNo.parent().find('div.btn-sendotpcode');
    countdowndisplay.hide();
    sendBtn.show().unbind('click').click(SendVerificationCodeCkickForloginStep2B);
}

/* 驗證碼倒數 */
var timerInterval;
function startTimer(duration, display, finish) {
    var timer = duration, minutes, seconds;
    timerInterval = setInterval(function () {
        minutes = parseInt(timer / 60, 10);
        seconds = parseInt(timer % 60, 10);
        minutes = minutes < 10 ? "0" + minutes : minutes;
        seconds = seconds < 10 ? "0" + seconds : seconds;
        display.text(minutes + ":" + seconds);
        if (--timer < 0) {
            // timer = duration;
            clearInterval(timerInterval);
            if (finish) finish();
        }
    }, 1000);
}
function stopTimer(display) {
    clearInterval(timerInterval);
}

function countdownClose() {
    stopTimer();
    var phoneNo = signForm2.find('input#otpTelInput');
    var countdowndisplay = phoneNo.parent().find('div.show-resend-time');
    var sendBtn = phoneNo.parent().find('div.btn-sendotpcode');
    countdowndisplay.hide();
    sendBtn.show().unbind('click').click(SendVerificationCodeCkick);
}

var setRecaptchaRequired = function (e) {
    setTimeout(function () {
        var id = e.find('textarea[name=g-recaptcha-response]').attr('id');
        if (id)
            $('#' + id).rules("add", { required: true });
        else
            setRecaptchaRequired(e);
    }, 1000);
}
var appendRecaptchaCode = function (e, datta) {
    var id = e.find('textarea[name=g-recaptcha-response]').attr('id');
    if (id) {
        var vval = e.find('#' + id).val();
        datta['g-recaptcha-response'] = vval;
    }
    return datta;
}

function getQueryStrByName(name) {
    var ps = getQueryString();
    if (ps) {
        if (ps[name] != null)
            return ps[name];
    }
    return '';

    //for not ie use
    //var locationUrl = new URL(window.location.href);
    //locationUrl.searchParams.forEach(function (v, paramname) {
    //    if (paramname == name) retutn v;
    //});
}

function getQueryString() {
    var key = false, res = {}, itm = null;
    // get the query string without the ?
    var qs = location.search.substring(1);
    // check for the key as an argument
    if (arguments.length > 0 && arguments[0].length > 1)
        key = arguments[0];
    // make a regex pattern to grab key/value
    var pattern = /([^&=]+)=([^&]*)/g;
    // loop the items in the query string, either
    // find a match to the argument, or build an object
    // with key/value pairs
    while (itm = pattern.exec(qs)) {
        if (key !== false && decodeURIComponent(itm[1]) === key)
            return decodeURIComponent(itm[2]);
        else if (key === false)
            res[decodeURIComponent(itm[1])] = decodeURIComponent(itm[2]);
    }

    return key === false ? res : null;
}