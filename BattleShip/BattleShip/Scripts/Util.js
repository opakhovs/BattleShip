$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var gameHub = $.connection.gameHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    gameHub.client.startGame = function () {
        $('#battlefield_start').hide();
        $('#battlefield_rival').removeClass("battlefield_wait");
    };

    gameHub.client.getTableCoords = function (result) {
        for (var i = 0; i < 100; i++) {
            if (result.coords[i].FieldType == 1)
                document.getElementById("your_table").rows[result.coords[i].Vertical - 1].cells[result.coords[i].Horizontal - 1].setAttribute('class', 'battlefield_cell_ship');
        }
    };

    $.connection.hub.start().done(function () {
        $('#playButton').click(function () {
            var result = gameHub.server.startGame($("#guidField").val());
            $('#playButton').addClass("battlefield_button_disable");
            if (result) {
                $('#battlefield_start_fields').addClass("battlefield_wait");
            }
         
        });
        $(document).ready(function () {
            gameHub.server.connectAndGetTableCoords();
        });
    });
   
});