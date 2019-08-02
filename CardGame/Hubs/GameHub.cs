using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CardGame.API.Hubs
{
	public class GameHub : Hub
	{
		public async Task SendMessage(string message)
		{
			await Clients.Caller.SendAsync("ReceiveMessage", message);
		}
	}
}
