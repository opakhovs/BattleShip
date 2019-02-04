$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var gameHub = $.connection.gameHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    gameHub.client.startGame = function () {
        $('#battlefield_start').hide();
        $('#battlefield_rival').removeClass("battlefield_wait");
    };

    gameHub.client.displayResult = function (result) {
        for (var i = 0; i < Object.keys(result.coords).length; i++) {
            if (result.coords[i].FieldType == "SHIP") 
                document.getElementById("your_table").rows[result.coords[i].Vertical - 1].cells[result.coords[i].Horizontal - 1].setAttribute('class', 'battlefield_cell_ship');
            else if (result.coords[i].FieldType == "MISS")
                document.getElementById("your_table").rows[result.coords[i].Vertical - 1].cells[result.coords[i].Horizontal - 1].setAttribute('class', 'battlefield_cell_miss');
            else if (result.coords[i].FieldType == "HIT")
                document.getElementById("your_table").rows[result.coords[i].Vertical - 1].cells[result.coords[i].Horizontal - 1].setAttribute('class', 'battlefield_cell_hit');
            else if (result.coords[i].FieldType == "SUNK")
                document.getElementById("your_table").rows[result.coords[i].Vertical - 1].cells[result.coords[i].Horizontal - 1].setAttribute('class', 'battlefield_cell_sunk');
        }
    };

    gameHub.client.getGuid = function (result) {
        $("#guidField").val(result);
        $('#playButton').removeClass("battlefield_button_disable");
        document.getElementById("playButton").disabled = false;
    }

    $.connection.hub.start().done(function () {
        $('#playButton').click(function () {
            var result = gameHub.server.startGame($("#guidField").val());
            document.getElementById("playButton").disabled = true;
            $('#playButton').addClass("battlefield_button_disable");
            if (result) {
                $('#battlefield_start_fields').addClass("battlefield_wait");
            }
        });
        $(document).ready(function () {
            gameHub.server.connectAndGetTableCoords();
            $(document).ready(function () {
                $("#generateButton").click(function () {
                    if (document.getElementById("playButton").disabled) {
                        if (confirm("Press 'OK' to generate new game, this game wiil be lost"))
                            gameHub.server.getGuid();
                    }
                    else
                        gameHub.server.getGuid();
                });
            });
        });
    });
   
});