using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class Game
	{
		public int GameId { get; set; }
		public bool GameStarted { get; set; }
		public int MinPlayers { get; set; }
		public int MaxPlayers { get; set; }
		public int ActivePlayer { get; set; }
		public virtual ICollection<Player> Players { get; set; }
		public Queue<Card> CardsRemaining { get; set; }
		public Stack<Card> CardsPlayed { get; set; }

		public Game() { }

		public Game(int id, int min = 2, int max = 4)
		{
			GameId = id;
			GameStarted = false;
			MinPlayers = min;
			MaxPlayers = max;
			Players = new List<Player>();
			CardsRemaining = new Queue<Card>();
			CardsPlayed = new Stack<Card>();
		}
	}
}
