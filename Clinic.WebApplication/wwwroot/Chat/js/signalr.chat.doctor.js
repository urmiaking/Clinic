$(function () {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/OnlineChat")
        .withAutomaticReconnect()
        .build();

    var patientName = $('#patientUserName').text();

    connection.start().then(function () {
        connection.invoke("SendConnectedNotificationToPatient", patientName).then(function() {
            console.log("notification sent...!");
        }).catch(function(err) {
            console.log(err.exception);
        });
    }).catch(err => console.error(err));

    $('#send').on('click',
        function() {
            var message = $('#messageInput').val();
            var to = $('#patientUserName').text();
            connection.invoke("SendChatMessage", to, message).then(function() {
                insertChat("me", message);
                $('#send').val('');
            }).catch(err => console.log(err.exception));
        });

    connection.on("receiveMessage",
        (message) => {
            insertChat("patient", message);
        });


    connection.on("patientDisconnected",
        () => {
            $.playSound('/Chat/just-saying.mp3'); //change it with an error sound
            swal({
                title: "قطع ارتباط بیمار",
                text: "",
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
});



var me = {};
me.profilePic = $('#doctorProfilePic').text();
me.avatar = "/../../Administrators/assets/images/doctors/" + me.profilePic;

var you = {};
you.profilePic = $('#patientProfilePic').text();
you.avatar = "/../../Administrators/assets/images/patients/" + you.profilePic;

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
        var myName = $('#doctorName').text();
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
        var patientName = $('#patientName').text();
        control = '<div class="direct-chat-msg right">' +
            '<div class="direct-chat-info clearfix" >' +
            '<span class="direct-chat-name pull-right">' + patientName + '</span>' +
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