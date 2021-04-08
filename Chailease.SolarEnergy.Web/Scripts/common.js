$(function () {
    // start aos
    AOS.init({
        easing: 'ease-in-sine',
        duration: 600
    });

    // menu =============
    $(".js-contact-btn").click(function () {
        $(this).toggleClass("active");
        $('#contactModal').modal('toggle');
    });

    $('#contactModal').on('hidden.bs.modal', function (e) {
        $(".js-contact-btn").removeClass("active");
    });
    // menu =============
    
    // $(window).on('resize', function () {
    //     checkIsMobile();
    // });

    //hide or show password
    $('.js-hide-password').on('click', function () {
        var $this = $(this),
            $password_field = $this.prev('input');
        ('password' === $password_field.attr('type')) ? $password_field.attr('type', 'text') : $password_field.attr('type', 'password');
        $(this).toggleClass("hide-password-icon");
        $password_field.putCursorAtEnd();
    });
    jQuery.fn.putCursorAtEnd = function () {
        return this.each(function () {
            if (this.setSelectionRange) {
                var len = $(this).val().length * 2;
                this.setSelectionRange(len, len);
            } else {
                $(this).val($(this).val());
            }
        });
    };

    /* 輸入框清除內容按鈕*/
    $(".form-control.has-btn-clear").on("keydown", function () {
        let btn = $(this).parent().find(".js-input-btn-clear");
        btn.fadeIn();
    });

    $(".js-input-btn-clear").on("click", function () {
        $(this).parent().find("input").val("");
        $(this).fadeOut();
    });

    $(".page-scroll").bind("click", function (event) {
        var $anchor = $(this);
        var $body = $("html, body");
        $body.animate({
            scrollTop: $($anchor.data("ancher")).offset().top - 60
        }, 600);
        event.preventDefault();
    });

    var fbid = $("meta[property='fb:app_id']").attr("content");
    var agent = navigator.userAgent;
    $(".js-share-line").click(function () {
        // var link = location.href;
        let link = $(this).data("share");
        console.log("shareLink:" + link);
        if (agent.indexOf("mobi") !== -1) {
            location.href = "line://msg/text/" + encodeURIComponent("中租全民電廠") + "%0D%0A%0D%0A" + encodeURIComponent(link);
        }
        else {
            var t = 510
                , r = 510
                , n = screen.width / 2 - t / 2
                , i = screen.height / 2 - r / 2;
            window.open("https://social-plugins.line.me/lineit/share?url=" + encodeURIComponent(link), "LINE Share", "toolbar=no,scrollbars=no,status=no,resizable=yes,top=" + i + ",left=" + n + ",width=" + t + ",height=" + r);
        }
    });

    $(".js-share-fb").click(function () {
        let link = $(this).data("share");
        console.log("shareLink:" + link);
        var url = "https://www.facebook.com/sharer/sharer.php?u=" + encodeURIComponent(link);
        $("body").append("<a id='shareToFb' href='" + url + "' target='_blank'></a>");
        $("#shareToFb")[0].click();
        $("#shareToFb").remove();
    });

    function checkIsMobile() {
        if ($(window).width() <= 1100) {
            $(".js-contact-link").attr("href", "tel:0226581677");
        } else {
            $(".js-contact-link").removeAttr("href");
        }
    }
    $(window).on('resize', function () {
        checkIsMobile();
    });
    checkIsMobile();

    //GA: 集客/出資會員/步驟0
    $('#modalBuyInfo').on('shown.bs.modal', function (e) {
        dataLayer.push(
            {
                "event": "popupPageView",
                "popPATH": "/AdvancedPurchase/step-info",
                "popTitle": "出資會員-Step0-資料準備提醒"
            }
        );
    });
    $('#modalBuyInfo').on('hidden.bs.modal', function (e) {
        dataLayer.push(
            {
                "event": "popupClosed",
            }
        );
    });
});




