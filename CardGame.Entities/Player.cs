using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Player
	{
		public string PlayerId { get; set; }
		public string HubId { get; set; }
		
		public virtual ICollection<Card> Hand { get; set; } = new List<Card>();

		// foreign key to Game
		public int? GameId { get; set; }
		public virtual Game Game { get; set; }

		// foreign key to Game.PlayersReady
		public int? PlayersReadyId { get; set; }
		public virtual Game PlayersReady { get; set; }


		public Player() { }

		public Player(string playerId) => PlayerId = playerId;
	}
}
