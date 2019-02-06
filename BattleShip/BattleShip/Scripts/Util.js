$(function () {

    var gameHub = $.connection.gameHub;

    gameHub.client.startGame = function (isOurTurn) {
        $('#battlefield_start').hide();
        if (isOurTurn) {
            $('#battlefield_rival').removeClass("battlefield_wait");
        }
    };

    gameHub.client.displayResult = function (result, my_table) {
        var table_to_change;
        if (my_table == true)
            table_to_change = document.getElementById("your_table");
        else
            table_to_change = document.getElementById("table_rival");
        for (var i = 0; i < Object.keys(result.coords).length; i++) {
            if (result.coords[i].FieldType == "SHIP") 
                table_to_change.rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell battlefield_cell_ship');
            else if (result.coords[i].FieldType == "MISS")
                table_to_change.rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell battlefield_cell_miss');
            else if (result.coords[i].FieldType == "HIT")
                table_to_change.rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell battlefield_cell_hit');
            else if (result.coords[i].FieldType == "SUNK")
                table_to_change.rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell battlefield_cell_sunk');
        }
    };

    gameHub.client.changeTurn = function (nextIsOurTurn) {
        if (nextIsOurTurn) {
            $('#battlefield_rival').removeClass("battlefield_wait");
        }
        else {
            $('#battlefield_rival').addClass("battlefield_wait");
        }
    };

    gameHub.client.showWin = function (isWin) {
        if (isWin == true)
            $("#notification-game-win").removeClass("none");
        else
            $("#notification-game-lost").removeClass("none");
        $('#battlefield_rival').addClass("battlefield_wait");
    }

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

        $("#table_rival").on("click", "td", function () {
            var cellContent = $(this).find(".battlefield_cell_content");
            var vertical = cellContent.data("y");
            var horizontal = cellContent.data("x");
            gameHub.server.shoot(vertical, horizontal);
        }); 

        $(document).ready(function () {

            gameHub.server.connectAndGetTableCoords();

            if (sessionStorage["guid"] != null)
                $("#guidField").val(sessionStorage["guid"]);
            $(".notification_submit_restart").click(function () {
                sessionStorage["guid"] = $("#guidField").val();
                window.location.reload();
                $("#guidField").val(sessionStorage["guid"]);
            });

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