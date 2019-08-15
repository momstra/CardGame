using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

		public bool AddGame(int gameId, Deck deck)
		{
			Game game = new Game(gameId);
			if (game != null)
			{
				game.Deck = deck;
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

		public Game GetGame(int gameId)
		{
			if (_context.Games.Find(gameId) != null)
				return _context.Games
					.Include(g => g.Players)
					.Include(g => g.CardsPlayed)
					.Single(g => g.GameId == gameId);

			return null;
		}

		public List<Game> GetGames() => _context.Games.ToList();

		public Player GetPlayer(string name) => _context.Players.Find(name);

		public List<Player> GetPlayers() => _context.Players.ToList();
			   
		public void SaveChanges()
		{
			_context.SaveChanges();
		}
	}
}
