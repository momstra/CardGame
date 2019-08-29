using System.Collections.Generic;

namespace CardGame.Entities
{
	public class Game
	{
		public int GameId { get; set; }
		public byte GameStatus { get; set; }		// 0 = not started, 1 = running, 10 = ended
		public int MinPlayers { get; set; }			// min number of players needed to start game
		public int MaxPlayers { get; set; }			// may number of players possible for game
		public int StartingHand { get; set; }		// number of cards dealt in the beginning
		public string ActivePlayer { get; set; }	// player whose turn it is
		public bool TurnCompleted { get; set; }		// current player's turn status

		public virtual List<Card> CardsPlayed { get; set; } = new List<Card>();		// cards open on table
		public virtual List<Card> CardsRemaining { get; set; } = new List<Card>();	// cards still in deck
		
		public virtual List<Player> PlayersReady { get; set; } = new List<Player>();// players ready to start
		public virtual List<Player> Players { get; set; } = new List<Player>();		// all players assigned to game

		// foreign key to assigned Set
		public int SetId { get; set; }
		public virtual Set Set { get; set; }		// set of cards for game
		

		public Game() { }

		public Game(int id, int min = 2, int max = 4)
		{
			GameId = id;
			GameStatus = 0;
			MinPlayers = min;
			MaxPlayers = max;
			StartingHand = 5;
			TurnCompleted = false;
		}
	}
}
