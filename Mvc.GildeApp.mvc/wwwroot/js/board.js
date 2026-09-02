// Keeps the board in step with the judges' phones.
//
// The server does all the arithmetic: every push carries the same board object the
// /board endpoint returns, already totalled and placed. This file only paints it.
(function () {
    "use strict";

    var config = window.gildeBoard;
    if (!config || !window.signalR) {
        return;
    }

    var connectionIndicator = document.getElementById("conn");
    var connectionLabel = connectionIndicator
        ? connectionIndicator.querySelector(".conn-label")
        : null;

    function setConnectionState(state, label) {
        if (!connectionIndicator) {
            return;
        }
        connectionIndicator.setAttribute("data-state", state);
        if (connectionLabel) {
            connectionLabel.textContent = label;
        }
    }

    function flash(element) {
        if (!element) {
            return;
        }
        element.classList.remove("just-changed");
        // Force a reflow so the animation restarts even on a repeated update.
        void element.offsetWidth;
        element.classList.add("just-changed");
    }

    function paintCells(board) {
        board.rows.forEach(function (row) {
            row.cells.forEach(function (cell) {
                if (cell.isSelf) {
                    return;
                }

                var key = row.position + "-" + cell.opponentPosition;
                var target = document.querySelector('[data-cell="' + key + '"]');
                if (!target) {
                    return;
                }

                var next = cell.score === null || cell.score === undefined
                    ? ""
                    : String(cell.score);

                if (target.textContent.trim() !== next) {
                    target.textContent = next;
                    flash(target);
                }

                target.className = "cell cell-" + String(cell.status || "").toLowerCase();
            });
        });
    }

    function paintTotals(board) {
        board.rows.forEach(function (row) {
            var total = document.querySelector('[data-total="' + row.position + '"]');
            if (total && total.textContent.trim() !== String(row.totalScore)) {
                total.textContent = row.totalScore;
                flash(total);
            }

            var place = document.querySelector('[data-place="' + row.position + '"]');
            if (place && place.textContent.trim() !== String(row.placement)) {
                place.textContent = row.placement;
                flash(place);
            }
        });
    }

    function paintMatches(board) {
        board.matches.forEach(function (match) {
            var row = document.querySelector('[data-match="' + match.matchId + '"]');
            if (!row) {
                return;
            }

            row.className = "match match-" + String(match.status || "").toLowerCase();

            var score = row.querySelector(".match-score");
            if (score) {
                var next = match.firstScore + "–" + match.secondScore;
                if (score.textContent.trim() !== next) {
                    score.innerHTML =
                        match.firstScore +
                        '<span class="match-dash">–</span>' +
                        match.secondScore;
                    flash(score);
                }
            }

            var status = row.querySelector(".match-status");
            if (status) {
                status.textContent = match.status;
            }
        });
    }

    function paintSummary(board) {
        var summary = document.getElementById("board-summary");
        if (!summary) {
            return;
        }

        var text = board.playerCount + " players · " +
            board.matchesFinished + " of " + board.matchesTotal + " matches finished";

        if (board.weaponName) {
            text += " · " + board.weaponName + " to " + board.maxScore;
        }

        summary.textContent = text;
    }

    function render(board) {
        if (!board || !board.rows) {
            return;
        }
        paintCells(board);
        paintTotals(board);
        paintMatches(board);
        paintSummary(board);
    }

    var connection = new signalR.HubConnectionBuilder()
        .withUrl(config.hubUrl)
        .withAutomaticReconnect()
        .build();

    connection.on("BoardUpdated", render);

    connection.onreconnecting(function () {
        setConnectionState("connecting", "Reconnecting…");
    });

    connection.onreconnected(function () {
        setConnectionState("live", "Live");
        // Rejoining is required: group membership does not survive a new connection id.
        connection.invoke("JoinTourney", config.tourneyId).catch(function () { });
    });

    connection.onclose(function () {
        setConnectionState("offline", "Offline — refresh to catch up");
    });

    connection.start()
        .then(function () {
            setConnectionState("live", "Live");
            return connection.invoke("JoinTourney", config.tourneyId);
        })
        .catch(function (error) {
            console.error("Could not connect to the board hub", error);
            setConnectionState("offline", "Offline — refresh to catch up");
        });
})();
