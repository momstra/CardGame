using System;

namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		// foreign key to containing deck
		public int SetId { get; set; }
		public virtual Set Set { get; set; }

		// foreign key to card holder
		public string UserId { get; set; }
		public virtual Player Owner { get; set; }
	}
}
