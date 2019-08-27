

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

		connection.on("AllReady", () => {
			$("#gamestatus").val("4: Everyone ready, game starting")
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

		connection.on("CardPlayedSuccess", () => {
			connection.invoke("GetHand");
		});

		connection.on("GameAdded", (gameid) => {
			GetGameList();
		});

		connection.on("GameReady", () => {
			$("#startgame").show();
		});

		connection.on("GameStarted", () => {
			$("#startgame").hide();
			connection.invoke("GetHand");
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

		connection.on("ReceiveHand", (hand) => {
			DisplayHand(hand);
		});

		connection.on("ReceiveMessage", (message) => {
			$("#answertext").val(message);
		});

		connection.start();
	}

	// get list of games, maybe replaced by hub method
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

	// get game's player list, maybe replaced by hub method
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

	function DisplayHand(json) {
		$("#cardcontainer").empty();
		var hand = JSON.parse(json);
		$.each(hand, function (_key, val) {
			var card = JSON.parse(val);
			var cardstring = card.Color + card.Rank;
			var input = '<input type="button" id="'
				+ card.CardId
				+ '" value="'
				+ cardstring
				+ '" class="card" />';
			$("#cardcontainer").append(input);
		});
	}
	
	$("#cardcontainer").on("click", ".card", function (event) {
		var cardid = $(event.target).attr("id");
		connection.invoke("PlayCard", cardid);
		connection.invoke("GetHand");
	});
	

	// create new user, connection to hub is set up at the end, using the returned token
	$("#createuser").on("click", function () {
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

	// on user change get new users game, purely for testing purposes
	$("#changeuser").on("click", function () {
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
	$("#creategame").on("click", function () {
		connection.invoke("CreateGame").catch(err => console.error(err));
	});

	// active user join game [game]
	$("#joingame").on("click", function () {
		var game = $("#games option:selected").text();
		connection.invoke("JoinGame", game);
	});

	// active user leave current game
	$("#leavegame").on("click", function () {
			connection.invoke("LeaveGame");
	});

	// set player ready
	$("#playerready").on("click", function () {
		connection.invoke("PlayerReady");
	});

	// not really used at the moment
	$("#sendmessage").on("click", function () {
		var message = $("#textmessage").val();
		connection.invoke("SendMessage", message).catch(err => console.error(err));
	});

	// starting the game manually before max player count is reached
	$("#startgame").on("click", function () {
		connection.invoke("StartGame");
	});
});