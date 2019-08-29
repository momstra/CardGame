using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;


using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CardGame.Services
{
	public class GameService : IGameService
	{
		private readonly ICardsRepository _repository;
		private readonly ILogger _logger;
		private readonly IConfiguration _config;

		public GameService(ICardsRepository repository, IConfiguration config, ILogger<GameService> logger)
		{
			_repository = repository;
			_config = config;
			_logger = logger;
		}

		public GameService(ICardsRepository repository, ILogger<GameService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		// check game's existence
		public bool CheckGameExists(int gameId)
		{
			if (_repository.GetGame(gameId) != null)
				return true;

			return false;
		}

		// check Game.GameStatus
		// 0 => not yet started
		// 1 => is running
		// 10 => ended
		public byte CheckGameStatus(int gameId) => _repository.GetGame(gameId).GameStatus;
		
		// create new game (and card set) and return GameId on success (otherwise 0)
		public int CreateGame(string playerId)
		{
			// TODO: maybe use sth more sensible for ids as guessing them shouldn't be a problem with authorization
			Random random = new Random();				// randomize 4 digit GameId
			int id = random.Next(1000, 9999);			//
														//
			if (_repository.GetGames().Count > 8999)	// there are only 9000 different ids
				return 0;								//
														//
			while (_repository.GetGame(id) != null)		// find id not in use
				id = random.Next(1000, 9999);			//

			_logger.LogInformation($"Card set for game [{id}] is being created...");
			Set set = _repository.CreateSet();          // create card set for new game and add it to database
			_repository.CreateCards(set);				// add cards to set (somewhere about here different card set possibilities should be implemented)

			_logger.LogInformation($"Game [{id}] is being created...");
			if (_repository.AddGame(id, set) == true)	// create new game and add it to database
			{
				GetGame(id).ActivePlayer = playerId;	// make creator active player
				_repository.SaveChanges();
				return id;
			}

			return 0;
		}

		// draw card from CardsRemaining 
		public Card DrawCard(int gameId)
		{
			Game game = _repository.GetGame(gameId);	// find game
			if (game.CardsRemaining.Count > 0)			// if cards left in deck
			{
				Card card = game.CardsRemaining[0];		// extract topmost card
				game.CardsRemaining.RemoveAt(0);		// remove it from list
				return card;							// and return it
			}

			return null;
		}

		// end game
		public bool EndGame(Game game)
		{
			game.GameStatus = 10;
			return true;
		}

		// get game by id
		public Game GetGame(int gameId) => _repository.GetGame(gameId);	

		// get game by joined player's id
		public Game GetGame(string playerId)
		{
			Player player = _repository.GetPlayer(playerId);// find player
			if (player.GameId != null)
			{
				Game game = GetGame((int)player.GameId);	// find and return its assigned game
				return game;
			}

			return null;
		}

		// get list of all games (ids)
		public List<int> GetGameIdsList()
		{
			var games = _repository.GetGames();			// get list of all games
			List<int> gameIds = new List<int>();
			foreach (Game game in games)				// and extract each game's id
				gameIds.Add(game.GameId);

			return gameIds;
		}

		// get list of all games (objects)
		public List<Game> GetGames() => _repository.GetGames();
		

		// get list of all players (objects) in game with GameId
		public List<Player> GetPlayers(int gameId) => _repository.GetGame(gameId).Players.ToList();

		// get list of all players (ids) in game with GameId
		public List<string> GetPlayersIds(int gameId)
		{
			var players = _repository.GetGame(gameId).Players;	// get list of assigned players
			List<string> playerIds = new List<string>();
			foreach (Player player in players)					// and extract each player's id
			{
				playerIds.Add(player.PlayerId);
			}
			return playerIds;
		}

		// return player whose turn it is
		public string GetTurnPlayer(int gameId)
		{
			var game = GetGame(gameId);
			if (game == null)
				return null;

			return game.ActivePlayer;
		}

		
		// join player with PlayerId to game with GameId and return joined game's id (otherwise 0)
		public int JoinGame(string playerId, int gameId)
		{
			_logger.LogInformation("Joining player [" + playerId + "] to game [" + gameId + "].");
			Game game = _repository.GetGame(gameId);			// find game
			if (game == null)
				return 0;

			if (game.MaxPlayers <= game.Players.Count)			// make sure max number of players has not yet been reached
				return 0;

			if (game.GameStatus > 0)							// nor has the game already started
				return 0;

			Player player = _repository.GetPlayer(playerId);	// find player
			if (player == null)
				return 0;

			game.Players.Add(player);							// and add it to game
			_repository.SaveChanges();
			if (player.Game != game)
				return 0;

			_logger.LogInformation("Player [" + playerId + "] joined game [" + gameId + "]");

			return gameId;
		}

		// remove player with PlayerID from game
		public bool LeaveGame(string playerId)
		{
			Player player = _repository.GetPlayer(playerId);    // find player

			return LeaveGame(player);							// call proper method
		}

		// remove player from game
		public bool LeaveGame(Player player)
		{ 
			if (player == null)
				return false;

			if (player.GameId == null)							// make sure it is assigned to a game
				return false;

			var gameId = player.GameId;

			Game game = _repository.GetGame((int)gameId);		// find that game
			if (game == null)
				return false;

			if (game.GameStatus == 1)							// if it already started, call relevant method and return its return
				return LeaveRunningGame(player);

			if (game.Players.Remove(player))					// if removal successful save and return true
			{
				_repository.SaveChanges();
				return true;
			}

			return false;
		}

		// remove player from game already started, override to find player beforehand if necessary
		public bool LeaveRunningGame(string PlayerId)
		{
			var player = _repository.GetPlayer(PlayerId);
			return LeaveRunningGame(player);				// call proper method with player object
		}

		// remove player from game already started
		public bool LeaveRunningGame(Player player)
		{
			var game = GetGame(player.PlayerId);	// find game
			if (game == null)
				return false;

			if (game.GameStatus != 1)				// make sure it really is already running
				return LeaveGame(player);           // and return relevant methods return if not
			
			if (player.Hand.Count > 0)				// recycle cards hold by 
			{
				var hand = player.Hand.ToList();	// extract cards to cycle through

				foreach (Card card in hand)
				{
					game.CardsRemaining.Add(card);	// add card to deck
					player.Hand.Remove(card);		// and remove from hand
				}

				var cards = game.CardsRemaining.ToList();	// extract cards from CardsRemaining deck
				ReShuffleCardsRemaining(game);				// ... and shuffle cards back into it
			}

			if (game.ActivePlayer == player.PlayerId)       // move turn if it is removed player's turn
			{
				game.TurnCompleted = true;					// make sure it will be removed
				MoveToNextPlayer(game.GameId);
			}

			if (game.Players.Remove(player))                // if removal successful save and return true
			{
				_repository.SaveChanges();

				if (game.Players.Count < 2)					// if only 1 player left end game
					return EndGame(game);

				return true;
			}

			return false;
		}

		// move turn to next player in line
		public Player MoveToNextPlayer(int gameId)
		{
			var game = GetGame(gameId);
			if (game == null)
				return null;

			if (game.TurnCompleted == false)		// false is default, therefore no check for GameStarted necessary
				return null;

			game.TurnCompleted = false;             // set back for next turn

			var activePlayerPosition = game.Players.FindIndex(p => p.PlayerId == game.ActivePlayer);	// get current players position in list
			var nextPlayerPosition = (activePlayerPosition + 1) % game.Players.Count;	// get position of next player
			var nextPlayer = game.Players[nextPlayerPosition];							// extract next player
			game.ActivePlayer = nextPlayer.PlayerId;									// and make it the new active player

			_repository.SaveChanges();
			return nextPlayer;
		}

		// play card
		public object PlayCard(int gameId, Card card)
		{
			Game game = _repository.GetGame(gameId);	// find game
			if (game == null)
				return null;

			game.CardsPlayed.Add(card);					// add played card to Played cards

			card.PlayerId = null;						// and remove ownership
			_repository.SaveChanges();

			if (!game.CardsPlayed.Contains(card))		// make sure card was added successfully
				return null;

			return new { CardId = card.CardId, Color = card.Color, Rank = card.Rank }; // and return custom object containing relevant info
		}

		// remove game from db
		public bool RemoveGame(int gameId)
		{
			Game game = _repository.GetGame(gameId);
			if (game == null)
				return true;				// TODO: should be changed if failure due to non-existence becomes relevant

			if (game.Players.Count > 0)		// make sure no players are assigned
				return false;

			var result = _repository.RemoveGame(gameId);
			return result;					// remove and return bool
		}

		// serve cards 
		public bool ServeStartingHands(int gameId)
		{
			Game game = GetGame(gameId);				// find game
			if (game == null)
				return false;

			if (game.GameStatus != 1)					// make sure it has been started
				return false;

			if (game.Players.Count < game.MinPlayers || game.Players.Count > game.MaxPlayers)
				return false;							// not really necessary anymore to check here

			for (int i = 0; i < game.StartingHand; i++)	// in turn give one card at a time
			{
				foreach (Player player in game.Players)	// ... to each player
				{
					var card = DrawCard(gameId);
					if (card == null)
						return false;

					player.Hand.Add(card);
				}
			}

			_repository.SaveChanges();
			return true;
		}

		// set asking player ready, returns:
		// 0 => failure
		// 1 => not yet enough players joined
		// 2 => enough players joined but still waiting for others to get ready
		// 3 => enough players joined and all ready, but max count not yet reached
		// 4 => max number of players joined and all ready
		public Byte SetPlayerReady(string playerId)
		{
			Player player = _repository.GetPlayer(playerId);	// find player
			if (player == null)
				return 0;

			Game game = player.Game;							// ... and player's assigned game
			if (game == null)
				return 0;

			if (game.PlayersReady.Contains(player))				// make sure it is not already ready
				return 0;

			game.PlayersReady.Add(player);						// set it ready
			_repository.SaveChanges();

			if (!game.PlayersReady.Contains(player))			// ... and make sure it was successful
				return 0;
																// ... and return relevant return code (see above)
			if (game.Players.Count < game.MinPlayers)
				return 1;

			if (game.PlayersReady.Count < game.Players.Count)
				return 2;

			if (game.Players.Count < game.MaxPlayers)
				return 3;

			return 4;
		}

		// initiate reshuffling RemainingCards
		public void ReShuffleCardsRemaining(Game game)
		{
			var cards = game.CardsRemaining.ToList();	// extract cards from CardsRemaining
			game.CardsRemaining.Clear();                // remove all cards from remaining list

			Shuffle(cards, game.GameId);                // call proper shuffle method
		}

		// initiate shuffling for game with GameId
		public void Shuffle(int gameId)
		{
			_repository.GetGame(gameId).CardsRemaining.Clear();                 // remove all cards from remaining list
			List<Card> cards = _repository.GetGame(gameId).Set.Cards.ToList();  // get a new list of all cards in set

			Shuffle(cards, gameId);                                             // call proper shuffle method
		}

		// initiate shuffling cards from list for game with GameId
		public void Shuffle(List<Card> cards, int gameId)
		{
			var game = _repository.GetGame(gameId);				// find game and call proper method
			Shuffle(cards, game);
		}

		// shuffle cards from list into Game's RemainingCards
		public void Shuffle(List<Card> cards, Game game)
		{ 
			if (cards.Count < 1)								// make sure list is not empty
				return;

			Random rand = new Random();
			Card randomCard;
			while (cards.Count() > 0)							// while still cards in list
			{
				var position = rand.Next(0, cards.Count());		// get random position within
				randomCard = cards[position];					// extract card at position
				game.CardsRemaining.Add(randomCard);			// add it to end of RemainingCards 
				cards.RemoveAt(position);						// and remove it from list
			}
		}

		// start game with GameId
		public bool StartGame(int gameId)
		{
			Game game = _repository.GetGame(gameId);	// find game
			if (game == null)
				return false;

			if (game.MinPlayers > game.Players.Count || game.MaxPlayers < game.Players.Count)
				return false;							// make sure number of players is within range
			
			Shuffle(gameId);							// fill deck in random order
			game.GameStatus = 1;						// ... set game started
			ServeStartingHands(gameId);					// ... and serve beginning hands
			_repository.SaveChanges();

			return true;
		}
	}
}
