using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CardGame.API.Hubs
{
	[Authorize]
	public class GameHub : Hub<IGameClient>
	{
		public async Task SendMessage(string message)
		{
			await Clients.Caller.ReceiveMessage(message);
		}
	}
}
