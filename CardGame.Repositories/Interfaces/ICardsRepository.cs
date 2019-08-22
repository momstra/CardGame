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
		Card GetCard(int cardId);
		Game GetGame(int gameId);
		List<Game> GetGames();
		List<Card> GetHand(string playerId);
		Player GetPlayer(string name);
		List<Player> GetPlayers();
		void SaveChanges();
	}
}
