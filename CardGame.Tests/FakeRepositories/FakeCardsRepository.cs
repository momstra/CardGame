using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Repositories.Interfaces;
using CardGame.Entities;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Tests.FakeRepositories
{
	public class FakeCardsRepository : ICardsRepository
	{
		private readonly CardsContext _context;

		public FakeCardsRepository()
		{
			var options = new DbContextOptionsBuilder<CardsContext>()
				.UseInMemoryDatabase("TestCardsDatabase").Options;
			_context = new CardsContext(options);
		}


		public void SaveChanges() => _context.SaveChanges();
		

		public bool AddGame(int gameId, Deck deck)
		{
			Game game = new Game(gameId);
			if (game == null)
				return false;

			game.Deck = deck;
			_context.Games.Add(game);
			SaveChanges();
			return true;
		}


		public void AddPlayer(Player player)
		{
			_context.Players.Add(player);
			SaveChanges();
		}

		public bool CreateCards(Deck deck)
		{

			string[] colors = { "diamond", "heart", "spades", "clubs" };
			string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

			foreach (string color in colors)
			{
				foreach (string rank in ranks)
				{
					Card card = new Card
					{
						Color = color,
						Rank = rank
					};
					deck.Cards.Add(card);
				}
			}
			SaveChanges();

			return true;
		}

		public Deck CreateDeck() => new Deck();

		public Game GetGame(int gameId)
		{
			if (_context.Games.Find(gameId) != null)
				return _context.Games
					.Include(g => g.Players)
					.Include(g => g.Deck)
					.First(g => g.GameId == gameId);

			return null;
		}

		public List<Game> GetGames() => _context.Games.ToList();

		public Player GetPlayer(string name) => _context.Players.Find(name);

		public List<Player> GetPlayers() => _context.Players.ToList();
	}
}
