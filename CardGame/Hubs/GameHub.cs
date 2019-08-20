using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using CardGame.Services.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace CardGame.API.Hubs
{
	[Authorize]
	public class GameHub : Hub<IGameClient>
	{
		private readonly IGameService _service;

		public GameHub(IGameService service)
		{
			_service = service;
		}

		public override Task OnConnectedAsync()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			_service.GetPlayer(playerId).HubId = Context.ConnectionId;

			return base.OnConnectedAsync();
		}

		// create new game and join asking player
		// returns new game's GameId on success
		public async Task CreateGame()
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			int gameId = _service.CreateGame();
			if (gameId != 0)
			{
				_service.JoinGame(playerId, gameId);
				await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
			}

			await Clients.All.GameAdded(gameId);
			await Clients.Group(gameId.ToString()).PlayerJoined(playerId);
		}

		public async Task JoinGame(int id)
		{
			string playerId = Context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

			var gameId = _service.JoinGame(playerId, id);
			if (gameId == id)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
				await Clients.Group(gameId.ToString()).PlayerJoined(playerId);
				await Clients.Caller.JoinSuccess(gameId);
			}
		}

		public async Task SendMessage(string message)
		{
			await Clients.All.ReceiveMessage(message);
		}
	}
}
