using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Entities;
using CardGame.Entities.Data;
using CardGame.Repositories.Interfaces;

namespace CardGame.Repositories
{
	public class CardsRepository : ICardsRepository
	{
		private readonly CardsContext _context;

		public CardsRepository(CardsContext context)
		{
			_context = context;
			DbInitializer.Initialize(_context);
		}

		public bool AddGame(int gameId)
		{
			Game game = new Game(gameId);
			if (game != null)
			{
				_context.Games.Add(game);
				_context.SaveChanges();
				if (_context.Games.Find(gameId) != null)
					return true;
			}
			return false;
		}

		public void AddPlayer(Player player)
		{
			_context.Players.Add(player);
			_context.SaveChanges();
		}

		public Queue<Card> GetCardsRemaining(int gameId) => GetGame(gameId).CardsRemaining;

		public List<Card> GetDeck() => _context.Deck.ToList();

		public Game GetGame(int gameId) => _context.Games.Find(gameId);

		public List<Game> GetGames() => _context.Games.ToList();

		public Player GetPlayer(string name) => _context.Players.Find(name);

		public List<Player> GetPlayers() => _context.Players.ToList();
			   
		public void SaveChanges()
		{
			_context.SaveChanges();
		}
	}
}
