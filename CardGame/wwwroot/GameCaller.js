$(document).ready(function () {
	// get game's player list
	function GetGamePlayer(gameid) {
		var uri = "/api/game/"
	}


	// create new user
	$("#createuser").click(function () {
		var username = $("#username").val();
		var uri = "/api/user/create/" + username;
		$.ajax({
			url: uri,
			success: function (tok) {
				var token = tok.token.toString();
				$("#userform").append('<input type="hidden" id="' + username + '" value="' + token + '" />');
				$("#users").append('<option>' + username + '</option>');
				$("#activeuser").val(username);
				$("#usertoken").val(token);
				$("#usergame").val(" ");
			}
		});
	});

	// on user change get new users game
	$("#changeuser").click(function () {
		var username = $("#users option:selected").text();
		var uri = "/api/user/game";
		$("#activeuser").val(username);
		var token = $('#' + username).val();
		$("#usertoken").val(token);
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (gameid) {
				$("#usergame").val(gameid);
			},
			error: function () {
				$("#usergame").val(" ");
			}
		});
	});

	// create new game
	$("#creategame").click(function () {
		var uri = "/api/game/create";
		var username = $("#activeuser").val();
		var token = $('#' + username).val();
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (gameid) {
				$("#usergame").val(gameid);
				$("#games").append('<option>' + gameid + '</option>');
			}
		});
	});

	// active user join game [game]
	$("#joingame").click(function () {
		var game = $("#games option:selected").text();
		var uri = "/api/user/join/" + game;
		var username = $("#activeuser").val();
		var token = $('#' + username).val();
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (gameid) {
				$("#usergame").val(gameid);
			}
		});
	});

	// active user leave current game
	$("#leavegame").click(function () {
		var game = $("#usergame").val();
		var uri = "/api/user/leave";
		var username = $("#activeuser").val();
		var token = $('#' + username).val();
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function () {
				$("#usergame").val("");
			}
		});
	});
});