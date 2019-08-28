using System.Collections.Generic;

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
		public bool TurnCompleted { get; set; }

		public virtual List<Card> CardsPlayed { get; set; } = new List<Card>();
		public virtual List<Card> CardsRemaining { get; set; } = new List<Card>();
		
		public virtual List<Player> PlayersReady { get; set; } = new List<Player>();
		public virtual List<Player> Players { get; set; } = new List<Player>();

		// foreign key to assigned Set
		public int SetId { get; set; }
		public virtual Set Set { get; set; }
		

		public Game() { }

		public Game(int id, int min = 2, int max = 4)
		{
			GameId = id;
			GameStarted = false;
			MinPlayers = min;
			MaxPlayers = max;
			StartingHand = 5;
		}
	}
}
