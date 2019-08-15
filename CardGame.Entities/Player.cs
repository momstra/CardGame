using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class Player
	{
		public string UserId { get; set; }

		public int HandId { get; set; }
		public virtual Hand Hand { get; set; }
		public int? GameId { get; set; }
		public virtual Game Game { get; set; }

		public Player() { }

		public Player(string userId)
		{
			UserId = userId;
		}
	}
}
