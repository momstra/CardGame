using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using CardGame.Services.Interfaces;


namespace CardGame.API.Hubs
{
	[Authorize]
	public class GameHub : Hub<IGameClient>
	{
		private readonly IGameService _gameService;
		private readonly IPlayerService _playerService;

		public GameHub(IGameService gameService, IPlayerService playerService)
		{
			_gameService = gameService;
			_playerService = playerService;
		}

		// set up hub related player properties
		public override Task OnConnectedAsync()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			_playerService.SetHubId(playerId, Context.ConnectionId);

			return base.OnConnectedAsync();
		}

		// remove player on disconnect
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var player = _playerService.GetPlayer(playerId);

			if (player.Game != null)
				await LeaveGame();//Groups.RemoveFromGroupAsync(Context.ConnectionId, player.GameId.ToString());

			_playerService.RemovePlayer(playerId);

			await base.OnDisconnectedAsync(exception);
		}

		// create new game and join asking player
		// returns new game's GameId on success
		public async Task CreateGame()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			int gameId = _gameService.CreateGame(playerId);
			if (gameId != 0)
			{
				_gameService.JoinGame(playerId, gameId);
				_gameService.GetGame(gameId).ActivePlayer = playerId;
				

				await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
			}

			await Clients.All.GameAdded(gameId);
			await Clients.Group(gameId.ToString()).PlayerJoined(playerId);
			await Clients.Caller.JoinSuccess(gameId);
		}

		public async Task GetHand()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var hand = _playerService.GetHand(playerId);

			if (hand != null)
			{
				await Clients.Caller.ReceiveHand(JsonConvert.SerializeObject(hand));
			}
		}

		public async Task JoinGame(int id)
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var gameId = _gameService.JoinGame(playerId, id);
			if (gameId == id)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
				await Clients.Group(gameId.ToString()).PlayerJoined(playerId);
				await Clients.Caller.JoinSuccess(gameId);
			}
		}

		public async Task LeaveGame()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			int gameId = _gameService.GetGame(playerId).GameId;

			if (_gameService.LeaveGame(playerId))
			{
				await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
				await Clients.Caller.LeaveSuccess();
				await Clients.Group(gameId.ToString()).PlayerLeft(playerId);
			}
		}

		public async Task PlayCard(int cardId)
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			int gameId = _gameService.GetGame(playerId).GameId;

			var card = _playerService.PlayCard(playerId, cardId);
			if (_gameService.PlayCard(gameId, card))
			{
				await Clients.Caller.CardPlayedSuccess();
				await Clients.Group(gameId.ToString()).CardPlayed();
			}

		}


		public async Task PlayerReady()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			int? gameId = _gameService.GetGame(playerId).GameId;
			if (gameId == null)
				return;

			Byte playerReady = _gameService.SetPlayerReady(playerId);
			if (playerReady == 0)
				return;
			
			if (playerReady == 1)									// not yet enough players
				await Clients.Caller.AwaitingPlayersToJoin();

			else if (playerReady == 2)                               // not yet everybody ready
				await Clients.Caller.AwaitingPlayersReady();

			else if (playerReady == 3)                               // all joined ready
			{
				await Clients.Group(gameId.ToString()).AllReadyWaiting();
				var activePlayerId = _gameService.GetGame((int)gameId).ActivePlayer;
				var activePlayer = _playerService.GetPlayer(activePlayerId);
				await Clients.Client(activePlayer.HubId).GameReady();// ActivePlayer is set to game creator
			}

			else if (playerReady == 4)                               // max players reached, all ready
				await Clients.Group(gameId.ToString()).AllReady();
		}


		public async Task SendMessage(string message)
		{
			await Clients.All.ReceiveMessage(message);
		}

		public async Task StartGame()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			int gameId = _gameService.GetGame(playerId).GameId;

			_gameService.StartGame(gameId);

			await Clients.Group(gameId.ToString()).GameStarted();
		}
	}
}
