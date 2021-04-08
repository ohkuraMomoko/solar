/**
 * $ajax
 */
var $ajax = new function () {
    var ajaxHeaders = {}

    /** 
    * Ajax AddHeader
    * @param {object} headers Ajax Add Header
    * @return {function} self
    */
    this.SetHeaders = function (headers) {
        ajaxHeaders = headers;
        return this;
    }

    this.Get = function (url, succ, loading) {
        var params = ajaxParams(url, "GET", {}, succ, loading);
        send(params, loading);
    }

    /**
    * Ajax Post
    * @param {string} url Url
    * @param {string} data Ajax post data
    * @param {function} succ Ajax success exec function
    * @param {function} err Ajax error exec function
    */
    this.Post = function (url, data, succ, err, loading) {
        var params = ajaxParams(url, "POST", JSON.stringify(data), succ, err, loading);
        send(params, loading);
    }

    this.FormPost = function (url, data, succ, err, loading) {
        var params = ajaxParams(url, "POST", data, succ, err, loading)
        params.processData = false;
        params.contentType = false;
        send(params, loading);
    }

    var ajaxParams = function (url, type, data, succ, err, loading) {
        if (loading == undefined) loading = true;
        return {
            url: url,
            type: type,
            dataType: 'json',
            cache: false,
            //將原本不是xml時會自動將所發送的data轉成字串(String)的功能關掉
            //processData: false,
            contentType: 'application/json',
            data: data,
            success: succ,
            error: function (jqXHR, textStatus, errorThrown) {
                //console.log(jqXHR);
                //console.log(textStatus);
                //console.log(errorThrown);
                if (err)
                    err(jqXHR, textStatus, errorThrown);
                else {
                    var d = JSON.parse(jqXHR.responseText)
                    if (d.isRecaptchaFail) {
                        recaptchaResponseReset();
                        showMsg(d.message, 'error');
                    }
                    else if (d.isLoacl) {
                        //showMsg('Ajax Error - ' + jqXHR.statusText, 'error');
                        showMsg('Ajax Error - ' + d.message, 'error');
                        console.log('Ajax Error - ' + d.message);
                    }
                    else {
                        var aa = $("<a>", { href: d.errpage, target: "_self" });
                        aa[0].click();
                        aa.remove();
                    }
                }
            },
            complete: function (jqXHR, textStatus) {
                if (loading) {
                    setTimeout(function () {
                        $("#loadingContainer").fadeOut(300);
                    }, 500);
                }
            },
            headers: ajaxHeaders
        }
    }

    var send = function (params, loading) {
        if (loading == undefined) loading = true;
        if (loading)
            $("#loadingContainer").fadeIn(100);
        $.ajax(params);
    }

}

var MenuRefresh = function () {

    $('#headerMeun').load(menuUrl, function () {
        menu_height_setting();
        screen_nav_hover();
        small_screen_nav_open();
        small_screen_nav_toggle();
        minimize_header();
    });

}

/*
 * Translated default messages for the jQuery validation plugin.
 * Locale: ZH (Chinese; 中文 (Zhōngwén), 漢語, 漢語)
 * Region: TW (Taiwan)
 */
$.extend($.validator.messages, {
    required: "必須填寫",
    remote: "請修正此欄位",
    email: "請輸入有效的電子郵件",
    url: "請輸入有效的網址",
    date: "請輸入有效的日期",
    dateISO: "請輸入有效的日期 (YYYY-MM-DD)",
    number: "請輸入正確的數值",
    digits: "只可輸入數字",
    creditcard: "請輸入有效的信用卡號碼",
    equalTo: "請重複輸入一次",
    extension: "請輸入有效的後綴",
    maxlength: $.validator.format("最多 {0} 個字"),
    minlength: $.validator.format("最少 {0} 個字"),
    rangelength: $.validator.format("請輸入長度為 {0} 至 {1} 之間的字串"),
    range: $.validator.format("請輸入 {0} 至 {1} 之間的數值"),
    max: $.validator.format("請輸入不大於 {0} 的數值"),
    min: $.validator.format("請輸入不小於 {0} 的數值")
});

$.validator.addMethod("phonecheck", function (value, element, param) { return new RegExp('^09[0-9]{8}$').test(value); }, "手機號碼格式有誤");
$.validator.addMethod("idcheck", function (value, element, param) { return idcheck(value); }, "身分證字號格式錯誤(英文字母限定大寫)");
$.validator.addMethod("pwdcheck", function (value, element, param) {
    if (new RegExp('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,12})').test(value))
        return value.length <= 12;
}, "密碼格式錯誤");
$.validator.addMethod("recommender", function (value, element, param) {
    if (!value) return true;
    //if (new RegExp('^.{0,5}$').test(value)) return false;
    //if (new RegExp('^[0-9]{6}$').test(value)) return true;
    //var result = false;
    $.ajax({
        url: recommenderUrl,
        type: "post",
        data: JSON.stringify({ id: value }),
        dataType: "json",
        async: false,
        contentType: "application/json; charset=utf-8",
        complete: function (data, textStatus) {
            if (data.status == 200)
                result = Boolean(data.responseJSON.result);
            return false;
        }
    });
    return result;
}, "推薦人不存在");
$.validator.addMethod("hasaccount", function (value, element, param) {
    if (!value) return true;
    if (!new RegExp('^[A-Za-z0-9._]*\@[A-Za-z0-9.]+$').test(value)) return true;
    var result = false;
    $.ajax({
        url: hasAccountUrl,
        type: "post",
        data: JSON.stringify({ id: value }),
        dataType: "json",
        async: false,
        contentType: "application/json; charset=utf-8",
        complete: function (data, textStatus) {
            if (data.status == 200) {
                result = !data.responseJSON.result;
            }
        }
    });
    return result;
}, "帳號已存在");
$.validator.addMethod("accountexist", function (value, element, param) {
    if (!value) return true;
    var result = false;
    if (new RegExp('^[A-Za-z0-9._]*\@[A-Za-z0-9]+[.][A-Za-z0-9.]+$').test(value)) {
        $.ajax({
            url: hasAccountUrl,
            type: "post",
            data: JSON.stringify({ id: value }),
            dataType: "json",
            async: false,
            contentType: "application/json; charset=utf-8",
            complete: function (data, textStatus) {
                if (data.status == 200)
                    result = Boolean(data.responseJSON.result);
                return false;
            }
        });
    }
    return result;
}, "帳號不存在");
$.validator.addMethod('filesize', function (value, element, param) { return this.optional(element) || (element.files[0].size <= param) }, "less than 30MB");
$.validator.addMethod('minimumAge', function (value, element, param) {

    var mon = $(param.mon).val();
    var day = $(param.day).val();
    if (value) {
        if (mon) {
            if (day) {
                return new Date(parseInt(value) + param.minAge, parseInt(mon) - 1, parseInt(day)) <= new Date();
            }
        }
    }
    return true;
}, "申請會員年齡不得小於?歲");

var validateParams = {
    ignore: [],
    //errorElement: "p",
    //errorClass: "input-error-info",
    focusInvalid: true,
    showErrors: function (errorMap, errorList) {
        //console.log(errorMap)
        //console.log(this);

        //var errMsgTagClass = 'input-error-info'
        var errMsgTagClass = 'input-error-info-signup'
        var errDivTagClass = 'div.input-group'

        var el = $(this.currentElements).parents(errDivTagClass);
        //el.removeClass('error');
        if (el.next('p.' + errMsgTagClass).length > 0) {
            el.next('p.' + errMsgTagClass).remove();
        }

        $.each(errorList, function (i, v) {
            var e = $(v.element).parents(errDivTagClass);
            //e.addClass('error');
            if (e.next('p.input-error-info').length == 0) {
                var p = $('<p></p>', { class: errMsgTagClass }).html(v.message);
                e.after(p);
            }
        });
    },
    rules: {

    },
    messages: {

    }
}

var showMsg = function (content, type) {
    if (!type)
        type = 'success';
    new Noty({
        type: type,// success, error, warning, information, notification
        layout: 'topRight',
        theme: 'nest',
        text: content,
        timeout: '4000',
        progressBar: true,
        closeWith: ['click'],
        killer: true
    }).show();
}

var imgcompress = function (islog) {
    var start, end, count;

    var self = this;
    var getexectime = function (s) { };
    this.exectime = function (func) { getexectime = func; };

    this.compress = function (file, size, succ, err) {

        if (file) {
            count = 0;
            log('**img procree start**');
            var filesize = file.size;
            log('name:' + file.name + ', size:' + filesize);
            var mb = size * 1024 * 1024;
            if (mb > filesize) {
                end = Date.now()
                succ(file);
                log('**finish:' + (end - start) / 1000 + 's, run count:' + count + '**');
                getexectime((end - start) / 1000);
                return;
            }
            start = Date.now();
            var quality = mb / filesize;

            log('quality:' + quality);
            filecompress(file, size, quality, succ, err);
        }

    }

    var filecompress = function (file, size, quality, succ, err) {
        new ImageCompressor(file, {
            quality: quality,
            success: function (result) {
                count++;
                log('filesize:' + result.size);
                if ((size * 1024 * 1024) > result.size) {
                    end = Date.now();
                    succ(result);
                    log('**finish:' + (end - start) / 1000 + 's, run count:' + count + '**');
                    getexectime((end - start) / 1000);
                }
                else {
                    var newquality = quality * 0.8;
                    log('quality:' + newquality);
                    filecompress(file, size, newquality, succ, err);
                }
            },
            error: function (e) {
                log('errMsg:' + e.message);
                err(e.message);
            },
        });
    }

    var log = function (msg) {
        if (islog)
            console.log(msg);
    }
}

var typeaheadoption = function (select) {
    this.clear = function () {
        $(select).val('');
        currectData = undefined;
    };
    this.setData = function (d) {
        data = d;
    };
    this.getVal = function () {
        return $(select).val();
    };
    this.getKey = function () {
        if (currectData) {
            if (this.getVal() == currectData.value)
                return currectData.key
        }
        return '';
    };

    var data = {};
    var currectData = undefined;
    var findMatches = function (query, process) {
        query = query.replace(/[\[\]]/g, '');//RegExp去除特殊字元
        var matches, substringRegex;
        matches = [];
        substrRegex = new RegExp(query, "i");
        $.each(data, function (i, v) {
            if (substrRegex.test(v.value)) {
                matches.push(v);
            }
        });
        process(matches);
    };

    $(select).typeahead('destroy');
    $(select).typeahead(
        {   // 參數設定
            //hint: true,
            highlight: true,  // 標註關鍵字
            minLength: 1  // 最小關鍵字長度
        },
        {
            name: "value",
            display: "value",
            limit: 8,
            source: findMatches // 建議列表資料
        }).on("typeahead:selected", function (obj, datum, name) {
            //console.log(obj);
            //console.log(datum);
            currectData = datum
        });
}

/**
 * 身分證字號檢核
 * @param {string} id 身分證字號(ex:A123456789)
 */
var idcheck = function (id) {
    if (!(/^[A-Z]{1}[1|2]{1}[\d]{8}$/.test(id))) return false;
    var az = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'W', 'Z', 'I', 'O']
    var au = [1, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1];
    var n1 = (az.indexOf(id[0].toUpperCase()) + 10).toString() + id.substring(1);
    var total = 0;
    for (var i = 0; i <= 10; i++) { total += parseInt(n1[i]) * au[i]; }
    var r = total % 10;
    return r == 0 ? true : false;
}

/**
 * scrolling end less event
 * @param {funciton} getdata event callback function，like:function (callback) {......; callback(); }
 * @param {int} sec callback function settimeout time，Default:0
 * @param {int} remainingHeight 相對高度，當相對高度低於該值，則觸發event，預設:300
 */
var scrollingendless = function (getdata, sec, relativeHeight) {
    var self = this;
    var h = 300;
    var s = 0;
    this.start = function () {
        if (!isNaN(relativeHeight)) {
            if (relativeHeight) h = relativeHeight;
        }
        if (!isNaN(sec)) {
            if (sec) s = sec;
        }
        window.addEventListener('scroll', populate);
        populate();
        return self;
    }

    this.end = function () {
        window.removeEventListener('scroll', populate);
        return self;
    }

    var populate = function () {
        let windowRelativeBottom = document.documentElement.getBoundingClientRect().bottom;
        if (windowRelativeBottom > document.documentElement.clientHeight + h) return;

        self.end()
        getdata(function () {
            setTimeout(function () {
                self.start();
            }, s * 1000);
        });
        return;
    }
}

var templateBind = function (h, v) {
    return h.replace(/{{\w+}}/igm, function (text, key) {
        var vvv = v[text.replace(/[{}]/igm, '')];
        if (vvv)
            return vvv;
        return '';
        //return v[text.replace(/[{}]/igm, '')];
    });
}

// 金額加千位符號
function setSymbol(num) {
    if (num)
        return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return '';
}

var loginExt = new function () {
    var self = this;

    self.open = function () {
        memberModal.modal('show');
        return self;
    }

    self.close = function () {
        memberModal.modal('hide');
        return self;
    }

    var refresh = false;
    self.setPageRefresh = function (isRefresh) {
        refresh = isRefresh;
        return self;
    }

    var callbackfunc = undefined;
    self.setCallback = function (callback) {
        callbackfunc = callback;
        return self;
    }

    var backUrl = '';
    self.setBackUrl = function (back) {
        if (back) backUrl = back;
        return self;
    }

    self.loginSucc = function () {
        if (backUrl) {
            window.location.href = backUrl;
        }
        else if (refresh) {
            //window.location.reload();
            if (window.location.hash === "#login") {
                window.location.href = window.location.href.split('#')[0];
            }
            else {
                window.location.href = window.location.href;
            }
        }
        else {
            if (callbackfunc) {
                callbackfunc();
            }
            MenuRefresh();
            self.refresh();
            self.close();
        }
    }

    self.refresh = function () {
        callbackfunc = undefined;
        backUrl = '';
        refresh = false;
    }
}

// upload event & upload file preview
$(".js-input-file").on('change', function () {
    var filePath = $(this).val();
    var arr = filePath.split("\\");
    var fileName = arr[arr.length - 1];
    $(this).parent().children(".js-file-name").text(fileName);
    previewImg(this);
});
function previewImg(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(input).parent().children(".js-file-preview").css("background-image", "url(" + e.target.result + ")");
        }
        reader.readAsDataURL(input.files[0]);
    }
    else {
        $(input).parent().children(".js-file-preview").css("background-image", "");
    }
}

/**
 * recaptchaResponseReset
 * @param {string} id Reset id recaptcha,id=null Reset All
 */
function recaptchaResponseReset(id) {
    if (id) {
        var lastChar = id[id.length - 1];
        if (isNaN(lastChar))
            grecaptcha.reset()
        else
            grecaptcha.reset(lastChar)
    }
    else {
        var g = $('textarea[name=g-recaptcha-response]');
        $.each(g, function (i, v) {
            var iii = $(v).attr('id');
            var lastChar = iii[iii.length - 1];
            if (isNaN(lastChar))
                grecaptcha.reset()
            else
                grecaptcha.reset(lastChar)
        });
    }
}