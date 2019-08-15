using System;

namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		public int DeckId { get; set; }
		public Deck Deck { get; set; }

		public int GameId { get; set; }
		public Game Game { get; set; }

		public int HandId { get; set; }
		public Hand Owner { get; set; }
	}
}
