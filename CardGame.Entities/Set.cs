using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Set
	{
		public int SetId { get; set; }
		public int SetSize { get; set; }

		//public int GameId { get; set; }
		public virtual Game Game { get; set; }

		public virtual List<Card> Cards { get; set; }


		public Set() 
		{
			SetSize = 52;
			Cards = new List<Card>();
		}
	}
}
