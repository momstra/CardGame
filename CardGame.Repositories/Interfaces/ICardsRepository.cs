using System;
using System.Collections.Generic;
using System.Text;
using CardGame.Entities;

namespace CardGame.Repositories.Interfaces
{
	public interface ICardsRepository
	{
		List<Card> Deck { get; }
		List<Hand> Hands { get; }
		List<Player> Players { get; }
		List<Game> Games { get; }

		Game GetGame(int gameId);
		Queue<Card> GetCardsRemaining(int gameId);
		Game AddGame(int gameId);
		void SaveChanges();
	}
}
