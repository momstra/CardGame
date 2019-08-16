using System;

namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		public int DeckId { get; set; }
		public virtual Deck Deck { get; set; }

		public int? HandId { get; set; }
		public virtual Hand Owner { get; set; }
	}
}
