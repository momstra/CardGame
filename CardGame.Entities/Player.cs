using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class Player
	{
		public string UserId { get; set; }

		public int HandId { get; set; }
		public Hand Hand { get; set; }
		public int GameId { get; set; }
		public Game Game { get; set; }

		public Player() { }

		public Player(string userId)
		{
			UserId = userId;
		}
	}
}
