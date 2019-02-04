$(function () {

    var gameHub = $.connection.gameHub;
    var isOurTurn;

    gameHub.client.startGame = function (isOurTurn) {
        $('#battlefield_start').hide();
        if (isOurTurn) {
            this.isOurTurn = true;
            $('#battlefield_rival').removeClass("battlefield_wait");
        }
        else {
            this.isOurTurn = false;
        }
    };

    gameHub.client.displayResult = function (result) {
    };

    gameHub.client.changeTurn = function (nextIsOurTurn) {
        if (nextIsOurTurn) {
            $('#battlefield_rival').removeClass("battlefield_wait");
        }
        else {
            $('#battlefield_rival').addClass("battlefield_wait");
        }
    };

    $.connection.hub.start().done(function () {
        $('#playButton').click(function () {
            var result = gameHub.server.startGame($("#guidField").val());
            $('#playButton').removeClass("battlefield_start_button").addClass("battlefield_start_button_disable");
            if (result) {
                $('#battlefield_start_fields').addClass("battlefield_wait");
            }         
        });  

        $("#table_rival").on("click", "td", function () {
            var cellContent = $(this).find(".battlefield_cell_content");
            var vertical = cellContent.data("y");
            var horizontal = cellContent.data("x");
            alert(vertical + "" + horizontal);
            gameHub.server.shoot(vertical, horizontal);
        }); 

        gameHub.client.tempMethod = function () {
            alert("Call of method");
        };

        $(document).ready(function () {
            gameHub.server.connect();
        });
    });
});