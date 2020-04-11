$(function () {
    $('#chatCardBox').hide();
    $('#startChat').click(function () {
        window.location.href = "/OnlineChat/15";
        $('#ruleBox').hide();
        $('#chatBox').hide();
        $("#chatCardBox").fadeIn();
        setTimeout(function () {
            $(".spinner").fadeOut(1000, function () {
                $('#chatCardBox').show();
                $('#chatBox').show();
                $("#spinner-text").fadeOut();
                //-- Print Messages
                insertChat("من", "سلام دکتر", 0);
                insertChat("پزشک", "سلام آقای اکبر پور", 1500);
                insertChat("من", "امروز وقت داری ببینمت؟", 3500);
                insertChat("پزشک", "باشه ساعت چند؟", 7000);
                insertChat("من", "هر موقع تو بگی! من تا ۱۲ شب بیکارم", 9500);
                insertChat("پزشک", "حله", 12000);
            });
        }, 5000);
        
    });
    
});

var me = {};
me.avatar = "/../../Administrators/assets/images/users/avatar-5.jpg";

var you = {};
you.avatar = "/../../Administrators/assets/images/doctors/4f8efedc-c55f-4903-8e85-2e75a4e2b2d8.png";

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

//-- No use time. It is a javaScript effect.
function insertChat(who, text, time) {
    if (time === undefined) {
        time = 0;
    }
    var control = "";
    var date = formatAMPM(new Date());

    if (who == "من") {
        control = '<li style="width:100%">' +
            '<div class="msj macro">' +
            '<div class="avatar"><img class="img-circle" style="width:20%;" src="' + me.avatar + '" /></div>' +
            '<div class="text text-l">' +
            '<p>' + text + '</p>' +
            '<p><small>' + date + '</small></p>' +
            '</div>' +
            '</div>' +
            '</li>';
    } else {
        control = '<li style="width:100%;">' +
            '<div class="msj-rta macro">' +
            '<div class="text text-r">' +
            '<p>' + text + '</p>' +
            '<p><small>' + date + '</small></p>' +
            '</div>' +
            '<div class="avatar-rta" style="padding:0px 0px 0px 10px !important"><img class="img-circle" style="width:20%;" src="' + you.avatar + '" /></div>' +
            '</li>';
    }
    setTimeout(
        function () {
            $("#messageBox").append(control).scrollTop($("#messageBox").prop('scrollHeight'));
        }, time);

}

function resetChat() {
    $("#messageBox").empty();
}

$(".mytext").on("keydown", function (e) {
    if (e.which == 13) {
        var text = $(this).val();
        if (text !== "") {
            insertChat("من", text);
            $(this).val('');
        }
    }
});

$("#clearChat").on("click", function () {
    resetChat();
});

$('body > div > div > div:nth-child(2) > span').click(function () {
    $(".mytext").trigger({ type: 'keydown', which: 13, keyCode: 13 });
});




//-- NOTE: No use time on insertChat.