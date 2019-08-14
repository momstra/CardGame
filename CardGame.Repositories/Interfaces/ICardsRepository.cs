using System;
using System.Collections.Generic;
using CardGame.Entities;

namespace CardGame.Repositories.Interfaces
{
	public interface ICardsRepository
	{
		bool AddGame(int gameId);
		void AddPlayer(Player player);

		Queue<Card> GetCardsRemaining(int gameId);
		List<Card> GetDeck();
		Game GetGame(int gameId);
		List<Game> GetGames();
		Player GetPlayer(string name);
		List<Player> GetPlayers();
		void SaveChanges();
	}
}
