using System;

namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		public Hand Owner { get; set; }
	}
}
