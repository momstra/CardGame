using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CardGame.Entities;
using CardGame.Repositories.Interfaces;

namespace CardGame.Repositories
{
	public class CardsRepository : ICardsRepository
	{
		private readonly CardsContext _context;

		public CardsRepository(CardsContext context)
		{
			_context = context;
		}

		public bool AddGame(int gameId, Set set)
		{
			Game game = new Game(gameId);
			if (game != null)
			{
				game.Set = set;
				_context.Games.Add(game);
				_context.SaveChanges();
				if (_context.Games.Find(gameId) != null)
					return true;
			}
			return false;
		}

		public void AddPlayer(Player player)	// obsolete, use CreatePlayer 
		{
			_context.Players.Add(player);
			_context.SaveChanges();
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
			if (_context.Games.Find(gameId) == null)
				return null;

			return _context.Games
				.Include(g => g.Players)
				.ThenInclude(p => p.Hand)
				.Include(g => g.CardsRemaining)
				.Include(g => g.Set)
					.ThenInclude(d => d.Cards)
				.Single(g => g.GameId == gameId);
		}

		public List<Game> GetGames() => _context.Games.ToList();

		public List<Card> GetHand(string playerId) => _context.Players.Find(playerId).Hand.ToList();

		public Player GetPlayer(string playerId)
		{
			if (_context.Players.Find(playerId) == null)
				return null;

			return _context.Players
				.Include(p => p.Hand)
				.Single(p => p.UserId == playerId);
		}

		public List<Player> GetPlayers() => _context.Players.ToList();
			   
		public void SaveChanges()
		{
			_context.SaveChanges();
		}
	}
}
