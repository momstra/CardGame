using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Deck
	{
		public int DeckId { get; set; }
		public int DeckSize { get; set; }

		//public int GameId { get; set; }
		public virtual Game Game { get; set; }

		public virtual ICollection<Card> Cards { get; set; }


		public Deck() 
		{
			DeckSize = 52;
			Cards = new List<Card>();
		}
	}
}
