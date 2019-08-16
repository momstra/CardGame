﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CardGame.Entities
{
	public class Game
	{
		public int GameId { get; set; }
		public bool GameStarted { get; set; }
		public int MinPlayers { get; set; }
		public int MaxPlayers { get; set; }
		public int ActivePlayer { get; set; }
		public List<Card> CardsRemaining { get; set; }
		public List<Card> CardsPlayed { get; set; }

		public int DeckId { get; set; }
		public virtual Deck Deck { get; set; }
		
		public virtual ICollection<Player> Players { get; set; }

		public Game() { }

		public Game(int id, int min = 2, int max = 4)
		{
			GameId = id;
			GameStarted = false;
			MinPlayers = min;
			MaxPlayers = max;
			CardsRemaining = new List<Card>();
			//CardsPlayed = new Stack<Card>();
		}
	}
}
