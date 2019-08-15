using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Repositories.Interfaces;
using CardGame.Entities;
using Moq;

namespace CardGame.Tests.FakeRepositories
{
	public class FakeCardsRepository : ICardsRepository
	{
		public List<Card> Decks { get; set; }
		public List<Hand> Hands { get; }
		public List<Player> Players { get; set; }
		public Stack<Card> CardsPlayed { get; set; }
		public List<Game> Games { get; }

		public FakeCardsRepository()
		{
			Decks = new List<Card>();
			Hands = new List<Hand>();
			Players = new List<Player>();
			CardsPlayed = new Stack<Card>();
			Games = new List<Game>();
			
		}


		public void SaveChanges()
		{

		}
		

		public bool AddGame(int gameId, Deck deck)
		{
			Game game = new Game(gameId);
			if (game == null)
				return false;

			game.Deck = deck;
			Games.Add(game);
			return true;
		}
		

		public void AddPlayer(Player player) => Players.Add(player);

		public Game GetGame(int gameId) => Games.Find(g => g.GameId == gameId);

		public List<Game> GetGames() => Games;

		public Player GetPlayer(string name) => Players.Find(c => c.UserId == name);

		public List<Player> GetPlayers() => Players;
	}
}
