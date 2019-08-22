using System;

namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		// foreign key to containing deck
		public int DeckId { get; set; }
		public virtual Deck Deck { get; set; }

		// foreign key to card holder
		public string UserId { get; set; }
		public virtual Player Owner { get; set; }
	}
}
