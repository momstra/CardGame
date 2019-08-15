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
		Player CreatePlayer(string playerId);
		//Card DrawCard(int gameId);
		Game GetGame(int gameId);
		Game GetGame(string userId);
		List<Game> GetGames();
		List<int> GetGamesList();
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();
		int JoinGame(string playerId, int gameId);
		void Shuffle(int gameId);
		void Shuffle(List<Card> cards, int gameId);
		bool StartGame(int gameId);
	}
}
