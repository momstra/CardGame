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

		public void AddPlayer(Player player)	// obsolete
		{
			_context.Players.Add(player);
			_context.SaveChanges();
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

			Hand hand = new Hand();
			_context.Hands.Add(hand);
			player.Hand = hand;

			SaveChanges();

			return player;
		}

		public Game GetGame(int gameId)
		{
			if (_context.Games.Find(gameId) != null)
				return _context.Games
					.Include(g => g.Players)
					.Include(g => g.Deck)
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
