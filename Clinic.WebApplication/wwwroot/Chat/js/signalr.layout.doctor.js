$(function () {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/OnlineChat")
        .withAutomaticReconnect()
        .build();
    connection.start();

    connection.on("patientRequest",
        (doctorId, patientId, reserveDateTime) => {
            $.playSound('/Chat/just-saying.mp3');
            swal({
                title: "درخواست گفتگوی آنلاین",
                text: "بیماری تقاضای گفتگوی آنلاین را با شما دارد!",
                type: "info",
                showCancelButton: true,
                confirmButtonClass: 'btn-primary waves-effect waves-light',
                confirmButtonText: 'قبول میکنم!',
                closeOnConfirm: false
            },
                function (isAccepted) {
                    if (isAccepted) {
                        window.location.href = "/OnlineChat/" + doctorId + "/" + patientId + "/" + reserveDateTime;
                    }
                });
        });
});