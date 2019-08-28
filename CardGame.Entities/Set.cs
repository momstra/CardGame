using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Set
	{
		public int SetId { get; set; }
		public int Size { get; set; }

		// assigned game
		public virtual Game Game { get; set; }

		// assigned cards
		public virtual List<Card> Cards { get; set; } = new List<Card>();


		public Set(int size = 52) => Size = size;
	}
}
