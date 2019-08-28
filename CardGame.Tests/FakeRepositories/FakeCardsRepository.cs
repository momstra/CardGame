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
		

		public bool AddGame(int gameId, Set set)
		{
			Game game = new Game(gameId);
			if (game == null)
				return false;

			game.Set = set;
			_context.Games.Add(game);
			SaveChanges();
			return true;
		}


		public void AddPlayer(Player player)    // obsolete, use CreatePlayer 
		{
			_context.Players.Add(player);
			SaveChanges();
		}

		public bool CreateCards(Set set)
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
					set.Cards.Add(card);
				}
			}

			SaveChanges();

			return true;
		}

		public Set CreateSet()
		{
			Set set = new Set();
			_context.Sets.Add(set);
			SaveChanges();

			return set;
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
					.Include(g => g.Set)
					.First(g => g.GameId == gameId);

			return null;
		}

		public List<Game> GetGames() => _context.Games.ToList();

		public List<Card> GetHand(string playerId) => _context.Players.Find(playerId).Hand.ToList();

		public Player GetPlayer(string playerId) => _context.Players.Find(playerId);

		public List<Player> GetPlayers() => _context.Players.ToList();

		public void RemovePlayer(string playerId)
		{
			Player player = GetPlayer(playerId);

			if(player != null)
			{
				_context.Players.Remove(player);
				SaveChanges();
			}
		}

		public bool RemoveGame(int gameId)
		{
			Game game = GetGame(gameId);
			if (game == null)
				return false;

			_context.Games.Remove(game);
			SaveChanges();
			return true;
		}
	}
}
