using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardGame.API.Hubs
{
	public interface IGameClient
	{
		Task AllReady();
		Task AllReadyWaiting();
		Task AwaitingPlayersReady();
		Task AwaitingPlayersToJoin();
		Task CardPlayed(string json);
		Task CardPlayedSuccess();

		Task GameReady();
		Task GameStarted();
		Task GamesUpdated();
		Task JoinSuccess(int gameId);
		Task LeaveSuccess();
		Task NewUser(string name);
		Task ReceiveGamePlayers(string json);
		Task ReceiveHand(string json);
		Task ReceiveGameList(string json);
		Task ReceiveMessage(string message);
		Task PlayerJoined(string username);
		Task PlayerLeft(string username);
	}
}
