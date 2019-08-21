using System;
using System.Collections.Generic;
using CardGame.Entities;

namespace CardGame.Repositories.Interfaces
{
	public interface ICardsRepository
	{
		bool AddGame(int gameId, Deck deck);
		void AddPlayer(Player player);
		bool CreateCards(Deck deck);
		Deck CreateDeck();
		Player CreatePlayer(string playerId);
		Game GetGame(int gameId);
		List<Game> GetGames();
		Player GetPlayer(string name);
		List<Player> GetPlayers();
		void SaveChanges();
	}
}
