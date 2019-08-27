using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardGame.Entities
{
	public class Game
	{
		public int GameId { get; set; }
		public bool GameStarted { get; set; }
		public int MinPlayers { get; set; }
		public int MaxPlayers { get; set; }
		public int StartingHand { get; set; }
		public string ActivePlayer { get; set; }
		public string[] PlayersReady { get; set; }

		public int CardsRemainingId { get; set; }
		[ForeignKey("CardsRemainingId")]
		public virtual List<Card> CardsRemaining { get; set; }

		public int CardsPlayedId { get; set; }
		[ForeignKey("CardsPlayedId")]
		public virtual List<Card> CardsPlayed { get; set; }

		public int SetId { get; set; }
		public virtual Set Set { get; set; }
		
		public virtual ICollection<Player> Players { get; set; }

		public Game() { }

		public Game(int id, int min = 2, int max = 4)
		{
			GameId = id;
			GameStarted = false;
			MinPlayers = min;
			MaxPlayers = max;
			StartingHand = 5;
			PlayersReady = new string[0];
			CardsRemaining = new List<Card>();
			Players = new List<Player>();
			CardsPlayed = new List<Card>();
		}
	}
}
