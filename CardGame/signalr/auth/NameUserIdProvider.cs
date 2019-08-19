using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardGame.API.signalr.auth
{
	public class NameUserIdProvider : IUserIdProvider
	{
		public string GetUserId(HubConnectionContext context)
		{
			return context.User?.Identity?.Name;
		}
	}
}
