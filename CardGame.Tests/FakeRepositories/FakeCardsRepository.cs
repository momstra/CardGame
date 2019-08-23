using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using CardGame.Repositories.Interfaces;
using CardGame.Entities;

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


		public void AddPlayer(Player player)    // obsolete, use CreatePlayer 
		{
			_context.Players.Add(player);
			SaveChanges();
		}

		public bool CreateCards(Deck deck)
		{

			string[] colors = { "D", "H", "S", "C" };
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
					_context.Cards.Add(card);
					deck.Cards.Add(card);
				}
			}

			SaveChanges();

			return true;
		}

		public Deck CreateDeck()
		{
			Deck deck = new Deck();
			_context.Decks.Add(deck);

			SaveChanges();

			return deck;
		}

		public Player CreatePlayer(string playerId)
		{
			Player player = new Player(playerId);
			_context.Players.Add(player);

			SaveChanges();

			return player;
		}

		public Card GetCard(int cardId) => _context.Cards.Find(cardId);

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

		public List<Card> GetHand(string playerId) => _context.Players.Find(playerId).Hand.ToList();

		public Player GetPlayer(string name) => _context.Players.Find(name);

		public List<Player> GetPlayers() => _context.Players.ToList();
	}
}
