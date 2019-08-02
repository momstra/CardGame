﻿using System;
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
	}
}
