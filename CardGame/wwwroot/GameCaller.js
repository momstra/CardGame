

$(document).ready(function () {

	var connection;
	var token;
	var username;

	// connect to gamehub
	function ConnectR(token) {
		connection = new signalR.HubConnectionBuilder()
			.withUrl("/hub", { accessTokenFactory: () => token })
			.configureLogging(signalR.LogLevel.Information)
			.build();

		connection.on("GameAdded", (gameid) => {
			$("#games").append('<option>' + gameid + '</option>');
			//GetGameList();
		});

		connection.on("JoinSuccess", () => {
			$("#usergame").val(gameid);
			GetGamePlayer();
		});

		connection.on("ReceiveMessage", (message) => {
			$("#answertext").val(message);
		});

		connection.on("PlayerJoined", (username) => {
			//$("#ingameusers").append('<option>' + username + '</option>');
			GetGamePlayer();
		});

		connection.start();
	}

	$("#sendmessage").click(function () {
		var message = $("#textmessage").val();
		connection.invoke("SendMessage", message).catch(err => console.error(err));
	});

	// get list of games
	function GetGameList() {
		var uri = "/api/game/users";
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (json) {
				$("#games").empty();
				$.each(json, function (key, game) {
					$("#games").append('<option>' + game + '</option>');
				});
			},
			error: function () {
				$("#games").empty();
			}
		});
	}

	// get game's player list
	function GetGamePlayer() {
		var uri = "/api/game/users";
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
		var name = $("#username").val();
		var uri = "/api/user/create/" + name;
		$.ajax({
			url: uri,
			success: function (tok) {
				token = tok.token.toString();
				$("#userform").append('<input type="hidden" id="' + name + '" value="' + token + '" />');
				$("#activeuser").val(name);
				$("#usertoken").val(token);
				$("#usergame").val(" ");
				$("#ingameusers").empty();
				ConnectR(token);
			}
		});
	});

	// on user change get new users game
	$("#changeuser").click(function () {
		username = $("#users option:selected").text();
		var uri = "/api/user/game";
		$("#activeuser").val(username);
		token = $('#' + username).val();
		$("#usertoken").val(token);
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (gameid) {
				$("#usergame").val(gameid);
				GetGamePlayer();
				username = $("#users option:selected").text();
				token = $('#' + username).val();
				ConnectR(token);
			},
			error: function () {
				$("#usergame").val(" ");
				$("#ingameusers").empty();
				username = $("#users option:selected").text();
				token = $('#' + username).val();
				ConnectR(token);
			}
		});
	});

	// create new game
	$("#creategame").click(function () {
		connection.invoke("CreateGame").catch(err => console.error(err));
		/*var uri = "/api/game/create";
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
		});*/
	});

	// active user join game [game]
	$("#joingame").click(function () {
		var game = $("#games option:selected").text();
		connection.invoke("JoinGame", game);
		/*var uri = "/api/user/join/" + game;
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (gameid) {
				$("#usergame").val(gameid);
				GetGamePlayer();
			}
		});*/
	});

	// active user leave current game
	$("#leavegame").click(function () {
		var uri = "/api/user/leave";
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