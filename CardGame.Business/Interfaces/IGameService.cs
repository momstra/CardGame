using System;
using System.Collections.Generic;
using System.Linq;

using CardGame.Entities;
using CardGame.Repositories.Interfaces;

namespace CardGame.Services.Interfaces
{
	public interface IGameService
	{
		int CreateGame();
		Card DrawCard(int gameId);
		Game GetGame(int gameId);
		Game GetGame(string userId);
		List<Game> GetGames();
		List<int> GetGameIdsList();
		List<Player> GetPlayers(int gameId);
		List<string> GetPlayersIds(int gameId);
		int JoinGame(string playerId, int gameId);
		bool LeaveGame(string playerId);
		bool ServeStartingHands(int gameId);
		Byte SetPlayerReady(string playerId);
		void Shuffle(int gameId);
		void Shuffle(List<Card> cards, int gameId);
		bool StartGame(int gameId);
	}
}
