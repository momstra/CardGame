using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Deck
	{
		public int DeckId { get; set; }
		public int DeckSize { get; set; }

		public virtual ICollection<Card> Cards { get; set; }
	}
}
