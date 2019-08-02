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

		public List<Card> Deck { get { return _context.Deck.ToList(); } }
		public List<Hand> Hands { get { return _context.Hands.ToList(); } }
		public List<Player> Players { get { return _context.Players.ToList(); } }
		public List<Game> Games { get { return _context.Games.ToList(); } }

		public CardsRepository(CardsContext context)
		{
			_context = context;
			DbInitializer.Initialize(_context);
		}
			   
		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public Game GetGame(int gameId) => _context.Games.Find(gameId);

		public Queue<Card> GetCardsRemaining(int gameId) => GetGame(gameId).CardsRemaining;

		public Game AddGame(int gameId)
		{
			Game game = new Game(gameId);
			if (game != null)
			{
				_context.Games.Add(game);
				_context.SaveChanges();
				return _context.Games.Find(gameId);
			}
			return null;
		}
	}
}
