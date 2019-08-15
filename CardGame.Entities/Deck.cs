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

			string[] colors = { "diamond", "heart", "spades", "clubs" };
			string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

			foreach (string color in colors)
			{
				foreach (string rank in ranks)
				{
					Card card = new Card
					{
						Color = color,
						Rank = rank
					};
					Cards.Add(card);
				}
			}
		}
	}
}
