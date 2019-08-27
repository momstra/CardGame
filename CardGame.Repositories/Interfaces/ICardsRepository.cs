using System;
using System.Collections.Generic;
using CardGame.Entities;

namespace CardGame.Repositories.Interfaces
{
	public interface ICardsRepository
	{
		bool AddGame(int gameId, Set set);
		void AddPlayer(Player player);
		bool CreateCards(Set set);
		Set CreateSet();
		Player CreatePlayer(string playerId);
		Card GetCard(int cardId);
		Game GetGame(int gameId);
		List<Game> GetGames();
		List<Card> GetHand(string playerId);
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();
		void SaveChanges();
	}
}
