using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class Player
	{
		public string UserId { get; set; }
		public string HubId { get; set; }
		
		// players hand 
		public virtual ICollection<Card> Hand { get; set; }

		// game player has joined
		public int? GameId { get; set; }
		public virtual Game Game { get; set; }

		public Player() { }

		public Player(string userId)
		{
			UserId = userId;
			Hand = new List<Card>();
		}
	}
}
