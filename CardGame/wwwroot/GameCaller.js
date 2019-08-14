$(document).ready(function () {
	$("#createuser").click(function () {
		var username = $("#username").val();
		var uri = "/api/game/user/create/" + username;
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
	$("#changeuser").click(function () {
		var username = $("#users option:selected").text();
		var uri = "/api/game/user/" + username + "/game";
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
	$("#joingame").click(function () {
		var game = $("#games option:selected").text();
		var uri = "/api/game/join/" + game;
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
});