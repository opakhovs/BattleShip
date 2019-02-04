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
        for (var i = 0; i < Object.keys(result.coords).length; i++) {
            if (result.coords[i].FieldType == "SHIP") 
                document.getElementById("your_table").rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell_ship');
            else if (result.coords[i].FieldType == "MISS")
                document.getElementById("your_table").rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell_miss');
            else if (result.coords[i].FieldType == "HIT")
                document.getElementById("your_table").rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell_hit');
            else if (result.coords[i].FieldType == "SUNK")
                document.getElementById("your_table").rows[result.coords[i].Vertical].cells[result.coords[i].Horizontal].setAttribute('class', 'battlefield_cell_sunk');
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
            alert(vertical + "" + horizontal);
            gameHub.server.shoot(vertical, horizontal);
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

            $(document).keydown(function (e) {
                if ((e.keyCode == 65 && e.ctrlKey) || ((e.which || e.keyCode) == 116) || (e.keyCode == 27)) {
                    alert("aaaa");
                }
            });

        });
    });
   
});