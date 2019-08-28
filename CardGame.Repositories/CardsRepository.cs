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
				.Include(g => g.PlayersReady)
				.Include(g => g.CardsRemaining)
				.Include(g => g.CardsPlayed)
				.Include(g => g.Set)
					.ThenInclude(d => d.Cards)
				.Single(g => g.GameId == gameId);
		}

		// get list of all registered game objects
		public List<Game> GetGames() => _context.Games.ToList();

		// get card objects assigned to player's hand 
		public List<Card> GetHand(string playerId) => _context.Players.Find(playerId).Hand.ToList();

		// get player object by id
		public Player GetPlayer(string playerId)
		{
			if (_context.Players.Find(playerId) == null)
				return null;

			return _context.Players
				.Include(p => p.Hand)
				.Include(p => p.Game)
				.Single(p => p.PlayerId == playerId);
		}

		// get list of all registered player objects
		public List<Player> GetPlayers() => _context.Players.ToList();

		// remove game from database
		public bool RemoveGame(int gameId)
		{
			Game game = GetGame(gameId);
			if (game == null)
				return false;

			_context.Games.Remove(game);
			SaveChanges();
			return true;
		}

		// remove player from database
		public void RemovePlayer(string playerId)
		{
			Player player = GetPlayer(playerId);
			if (player != null)
			{
				_context.Players.Remove(player);
				SaveChanges();
			}
		}

		// save changes to database   
		public void SaveChanges()
		{
			_context.SaveChanges();
		}
	}
}
