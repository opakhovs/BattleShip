$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var gameHub = $.connection.gameHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    gameHub.client.startGame = function () {
        $('#battlefield_start').hide();
        $('#battlefield_rival').removeClass("battlefield_wait");
    };

    $.connection.hub.start().done(function () {
        $('#playButton').click(function () {
            var result = gameHub.server.startGame($("#guidField").val());
            if (result) {
                $('#battlefield_start_fields').addClass("battlefield_wait");
            }
         
        });
        $(document).ready(function () {
            gameHub.server.connect();
        });
    });
});