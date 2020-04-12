var connection = new signalR.HubConnectionBuilder()
    .withUrl("/OnlineChat")
    .withAutomaticReconnect()
    .build();



$(function () {

    var doctorName = $('#doctorUserName').text();
    var reserveDateTime = $('#reserveDateTime').text();

    connection.start().then(function () {
        console.log("connected!");
        connection.invoke("SendChatRequestToDoctor", doctorName, reserveDateTime).then(function () {
            console.log("Chat request sent...!");
        }).catch(function (err) {
            console.log(err.exception);
        });
        $('#send').attr('disabled', false);
    }).catch(function (err) {
        return console.error(err.toString());
    });
    //Disabling Chat functionality and showing a loading box
    $('#send').prop('disabled', true);
    $('.direct-chat-messages').hide();
    $('#loadingbox').show();

    connection.on("doctorConnected",
        function () {
            $('#loadingbox').hide();
            $('.direct-chat-messages').show();
            $('#send').prop('disabled', false);
            $.playSound('/Chat/just-saying.mp3');
            swal({
                title: "پزشک متصل شد!",
                text: "می توانید شروع به گفتگو کنید!",
                type: "success",
                showCancelButton: false,
                confirmButtonClass: 'btn-success waves-effect waves-light',
                confirmButtonText: 'باشه!'
            });
        });

    $('#send').on('click',
        function () {
            if ($('#messageInput').val().length === 0) {
                return;
            }
            var message = $('#messageInput').val();
            var to = $('#doctorUserName').text();
            $('#messageInput').val('');
            connection.invoke("SendChatMessage", to, message).then(function () {
                insertChat("me", message);
                $('#send').val('');
            }).catch(err => console.log(err.exception));
        });


    connection.on("receiveMessage",
        (message) => {
            insertChat("doctor", message);
        });

    $('#exit').on('click',
        function () {
            swal({
                title: "مطمئنی؟؟",
                text: "با خروج از صفحه، ارتباط شما قطع و به پنل خود بازمیگردید!",
                type: "info",
                confirmButtonClass: 'btn-primary waves-effect waves-light',
                confirmButtonText: 'باشه!',
                closeOnConfirm: false,
                showCancelButton: true
            }, function (isConfirm) {
                if (isConfirm) {
                    connection.invoke("ExitPatient", doctorName)
                        .then(function () {
                            window.history.back();
                        })
                        .catch(err => console.log(err.exception));
                }
            });
        });

    connection.on("doctorExited",
        () => {
            $.playSound('/Chat/error.mp3'); //change it with an error sound
            swal({
                title: "پزشک از گفتگو خارج شد",
                text: "",
                type: "error",
                confirmButtonClass: 'btn-primary waves-effect waves-light',
                confirmButtonText: 'باشه برگردیم به پنل!',
                closeOnConfirm: false,
                showCancelButton: false
            },
                function (isAccepted) {
                    if (isAccepted) {
                        window.history.back();
                    }
                });
        });

    connection.on("disconnected",
        () => {
            $.playSound('/Chat/error.mp3'); //change it with an error sound
            swal({
                title: "قطع ارتباط پزشک",
                text: "ارتباط پزشک قطع شده است و پیام شما را دریافت  نخواهد کرد",
                type: "error",
                confirmButtonClass: 'btn-primary waves-effect waves-light',
                confirmButtonText: 'باشه برگردیم به پنل!',
                closeOnConfirm: false
            },
                function (isAccepted) {
                    if (isAccepted) {
                        window.history.back();
                    }
                });
        });

    connection.on("doctorUnavailable",
        () => {

        });
});

var me = {};
me.profilePic = $('#patientProfilePic').text();
me.avatar = "/../../Administrators/assets/images/patients/" + me.profilePic;

var you = {};
you.profilePic = $('#doctorProfilePic').text();
you.avatar = "/../../Administrators/assets/images/doctors/" + you.profilePic;

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'ب.ظ' : 'ق.ظ';
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

    if (who == "me") {
        var myName = $('#patientName').text();
        control = '<div class="direct-chat-msg">' +
            '<div class="direct-chat-info clearfix" >' +
            '<span class="direct-chat-name pull-right">' + myName + '</span>' +
            '<span class="direct-chat-timestamp pull-left">' + date + '</span>' +
            '</div >' +
            '<img class="direct-chat-img" src="' + me.avatar + '" alt = "message user image" >' +
            '<div class="direct-chat-text" >' +
            text +
            '</div >' +
            '</div >';
    } else {
        var doctorName = $('#doctorName').text();
        control = '<div class="direct-chat-msg right">' +
            '<div class="direct-chat-info clearfix" >' +
            '<span class="direct-chat-name pull-right">' + doctorName + '</span>' +
            '<span class="direct-chat-timestamp pull-left">' + date + '</span>' +
            '</div >' +
            '<img class="direct-chat-img" src="' + you.avatar + '" alt = "message user image" >' +
            '<div class="direct-chat-text" >' +
            text +
            '</div >' +
            '</div >';
    }
    setTimeout(
        function () {
            $("#chatbox").append(control).scrollTop($("#chatbox").prop('scrollHeight'));
        }, time);

}

function resetChat() {
    $("#chatbox").empty();
}


