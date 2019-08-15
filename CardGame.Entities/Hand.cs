using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class Hand
	{
		public int HandId { get; set; }

		public virtual ICollection<Card> Cards { get; set; }

		public Hand() { }

		public Hand(int handId)
		{
			Cards = new List<Card>();
		}
	}
}
