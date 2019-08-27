

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

		// possible return of/occurence at "PlayerReady" 
		connection.on("AllReady", () => {
			$("#gamestatus").val("Everyone ready, game starting")
		});

		// possible return of/occurence at "PlayerReady"
		connection.on("AllReadyWaiting", () => {
			$("#gamestatus").val("Waiting for players to join")
		});

		// possible return of/occurence at "PlayerReady"
		connection.on("AwaitingPlayersReady", () => {
			$("#gamestatus").val("Waiting for players to get ready")
		});

		// possible return of/occurence at "PlayerReady"
		connection.on("AwaitingPlayersToJoin", () => {
			$("#gamestatus").val("Not enough players, waiting for more to join")
		});

		// return of "PlayCard" to whole Group (aka game)
		connection.on("CardPlayed", (json) => {
			ShowCardPlayed(json);
		});

		// return of "PlayCard"
		connection.on("CardPlayedSuccess", () => {
			connection.invoke("GetHand");			// update player's hand
		});

		// return of/occurs at "CreateGame"
		connection.on("GameAdded", (gameid) => {
			connection.invoke("GetGames");			// update list of games
		});

		// occurs when everybody's ready (only for game creator)
		connection.on("GameReady", () => {
			$("#startgame").show();					// when everyone ready, show button
		});

		// return of/occurs at "StartGame"
		connection.on("GameStarted", () => {
			$("#startgame").hide();					// game started, button can be hid again
			connection.invoke("GetHand");			// get player's hand
		});

		// return of "JoinGame"
		connection.on("JoinSuccess", (gameid) => {
			$("#usergame").val(gameid);				// show active game's id
			connection.invoke("GetGamePlayers");	// get list of players in game
		});

		// return of "LeaveGame"
		connection.on("LeaveSuccess", () => {
			$("#usergame").val("");					// remove gameid from display
			$("#ingameusers").empty();				// empty list of former game's players
		});

		// occurs at "JoinGame"
		connection.on("PlayerJoined", (username) => {
			connection.invoke("GetGamePlayers");	// update list of players in game
		});

		// occurs at "LeaveGame"
		connection.on("PlayerLeft", (username) => {
			connection.invoke("GetGamePlayers");	// update list of players in game
		});

		// return of "GetGames
		connection.on("ReceiveGameList", (json) => {
			ShowGames(json);						// display received list of games
		});

		// return of "GetGamePlayers"
		connection.on("ReceiveGamePlayers", (json) => {
			ShowGamePlayers(json);					// display received list of players
		});

		// return of "GetHand"
		connection.on("ReceiveHand", (json) => {
			ShowHand(json);							// display received hand
		});

		// return of "SendMessage"
		connection.on("ReceiveMessage", (message) => {
			$("#answertext").val(message);			
		});

		connection.start()
			.then(function () {
				connection.invoke("GetGames");
			});
	}

	// display list of players in game
	function ShowGamePlayers(json) {
		$("#ingameusers").empty();

		var list = JSON.parse(json);
		$.each(list, function (_key, player) {
			$("#ingameusers").append('<option>' + player + '</option>');
		});
	}

	// display list of games
	function ShowGames(json) {
		$("#games").empty();

		var games = JSON.parse(json);
		$.each(games, function (_key, game) {
			$("#games").append('<option>' + game + '</option>');
		});
	}

	// display player's cards
	function ShowHand(json) {
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

	// displayed last card played
	function ShowCardPlayed(json) {
		$("#table").empty();

		var card = JSON.parse(json);
		var cardstring = card.Color + card.Rank;

		$("#table").text(cardstring);
	}

	// play card when clicked
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
				ConnectR(token);
				$("#userform").append('<input type="hidden" id="' + name + '" value="' + token + '" />');
				$("#users").append('<option>' + name + '</option>');
				$("#activeuser").val(name);
				$("#usertoken").val(token);
				$("#usergame").val(" ");
				$("#ingameusers").empty();
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
				connection.invoke("GetGamePlayers");
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