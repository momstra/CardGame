

$(document).ready(function () {

	var connection;

	// connect to gamehub
	function ConnectR(token) {
		connection = new signalR.HubConnectionBuilder()
			.withUrl("/hub", { accessTokenFactory: () => token })
			.configureLogging(signalR.LogLevel.Information)
			.build();

		connection.on("ReceiveMessage", (message) => {
			$("#answertext").val(message);
		});

		connection.start();
	}

	$("#sendmessage").click(function () {
		var message = $("#textmessage").val();
		connection.invoke("SendMessage", message).catch(err => console.error(err));
	});
	// get game's player list
	function GetGamePlayer() {
		var uri = "/api/game/users";
		var username = $("#activeuser").val();
		var token = $('#' + username).val();
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (json) {
				$("#ingameusers").empty();
				$.each(json, function (key, player) {
					$("#ingameusers").append('<option>' + player + '</option>');
				});
			},
			error: function () {
				$("#ingameusers").empty();
			}
		});
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
				$("#ingameusers").empty();
				ConnectR(token);
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
				GetGamePlayer();
				var username = $("#users option:selected").text();
				var token = $('#' + username).val();
				ConnectR(token);
			},
			error: function () {
				$("#usergame").val(" ");
				$("#ingameusers").empty();
				var username = $("#users option:selected").text();
				var token = $('#' + username).val();
				ConnectR(token);
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
				GetGamePlayer();
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
				GetGamePlayer();
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
				$("#ingameusers").empty();
			}
		});
	});
});