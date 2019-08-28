namespace CardGame.Entities
{
	public class Card
	{
		public int CardId { get; set; }
		public string Color { get; set; }
		public string Rank { get; set; }

		// foreign key to Game.CardsPlayed
		public int? PlayedId { get; set; }
		public virtual Game CardsPlayed { get; set; }

		// foreign key to Game.CardsRemaining
		public int? RemainingId { get; set; }
		public virtual Game CardsRemaining { get; set; }

		// foreign key to containing set 
		public int SetId { get; set; }
		public virtual Set Set { get; set; }

		// foreign key to card holder
		public string UserId { get; set; }
		public virtual Player Owner { get; set; }
	}
}
