﻿

$(document).ready(function () {

	var connection;
	var token;
	var username;

	$("#startgame").hide();

	// connect to gamehub
	function ConnectR(token) {
		connection = new signalR.HubConnectionBuilder()
			.withUrl("/hub", { accessTokenFactory: () => token })
			.configureLogging(signalR.LogLevel.Information)
			.build();

		connection.on("AllReady 4:", () => {
			$("#gamestatus").val("Everyone ready, game starting")
		});

		connection.on("AllReadyWaiting", () => {
			$("#gamestatus").val("3: Waiting for players to join")
		});

		connection.on("AwaitingPlayersReady", () => {
			$("#gamestatus").val("2: Waiting for players to get ready")
		});

		connection.on("AwaitingPlayersToJoin", () => {
			$("#gamestatus").val("1: Not enough players, waiting for more to join")
		});

		connection.on("GameAdded", (gameid) => {
			GetGameList();
		});

		connection.on("GameReady", () => {
			$("#startgame").show();
		});

		connection.on("GameStarted", () => {
			GetHand();
		});

		connection.on("JoinSuccess", (gameid) => {
			$("#usergame").val(gameid);
			GetGamePlayer();
		});

		connection.on("LeaveSuccess", () => {
			$("#usergame").val("");
			$("#ingameusers").empty();
		});

		connection.on("PlayerJoined", (username) => {
			GetGamePlayer();
		});

		connection.on("PlayerLeft", (username) => {
			GetGamePlayer();
		});

		connection.on("ReceiveMessage", (message) => {
			$("#answertext").val(message);
		});

		connection.start();
	}

	$("#sendmessage").click(function () {
		var message = $("#textmessage").val();
		connection.invoke("SendMessage", message).catch(err => console.error(err));
	});

	// get list of games
	function GetGameList() {
		var uri = "/api/game/list";
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (json) {
				$("#games").empty();
				$.each(json, function (_key, game) {
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
				$.each(json, function (_key, player) {
					$("#ingameusers").append('<option>' + player + '</option>');
				});
			},
			error: function () {
				$("#ingameusers").empty();
			}
		});
	}

	// retrieve player's hand
	function GetHand() {

		var uri = "/api/user/hand";
		$.ajax({
			url: uri,
			headers: {
				"Authorization": "Bearer " + token
			},
			success: function (json) {
				$.each(json, function (_key, card) {
					$("#cardcontainer").append(card + "<br />");
				});
			}
		});
	}

	// start game manually
	function StartGame() {
		connection.invoke("StartGame");
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
				$("#users").append('<option>' + name + '</option>');
				$("#activeuser").val(name);
				$("#usertoken").val(token);
				$("#usergame").val(" ");
				$("#ingameusers").empty();
				GetGameList();
				ConnectR(token);
			}
		});
	});

	// on user change get new users game
	$("#changeuser").click(function () {
		username = $("#users option:selected").text();
		var uri = "/api/game/get";
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
	});

	// active user join game [game]
	$("#joingame").click(function () {
		var game = $("#games option:selected").text();
		connection.invoke("JoinGame", game);
	});

	// active user leave current game
	$("#leavegame").click(function () {
			connection.invoke("LeaveGame");
	});

	// set player ready
	$("#playerready").click(function () {
		connection.invoke("PlayerReady");
	});

	$("#startgame").click(function () {
		connection.invoke("StartGame");
	});
});