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

		// check GameStarted
		// returns true if started
		public bool CheckGameStatus(int gameId) => _repository.GetGame(gameId).GameStarted;
		
		// create new game (and deck)
		// returns GameId on success, otherwise 0
		public int CreateGame(string playerId)
		{
			Random random = new Random();
			int id = random.Next(1000, 9999);

			if (_repository.GetGames().Count >= 8999)
				return 0;

			while (_repository.GetGame(id) != null)
				id = random.Next(1000, 9999);

			_logger.LogInformation($"Deck for game [{id}] is being created...");
			Set deck = _repository.CreateSet();
			_repository.CreateCards(deck);

			_logger.LogInformation($"Game [{id}] is being created...");
			if (_repository.AddGame(id, deck) == true)
			{
				GetGame(id).ActivePlayer = playerId;
				_repository.SaveChanges();
				return id;
			}

			return 0;
		}

		// draw card from CardsRemaining 
		public Card DrawCard(int gameId)
		{
			Game game = _repository.GetGame(gameId);
			if (game.CardsRemaining.Count > 0)
			{
				Card card = game.CardsRemaining[0];
				game.CardsRemaining.RemoveAt(0);
				return card;
			}

			return null;
		}

		// get game by id
		public Game GetGame(int gameId) => _repository.GetGame(gameId);

		// get game by joined player's id
		public Game GetGame(string userId)
		{
			Player player = _repository.GetPlayer(userId);
			if (player.GameId != null)
			{
				Game game = GetGame((int)player.GameId);
				return game;
			}

			return null;
		}

		// get list of all games (ids)
		public List<int> GetGameIdsList()
		{
			var games = _repository.GetGames();
			List<int> gameIds = new List<int>();
			foreach (Game game in games)
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
			var players = _repository.GetGame(gameId).Players;
			List<string> playerIds = new List<string>();
			foreach (Player player in players)
			{
				playerIds.Add(player.UserId);
			}
			return playerIds;
		}
		
		// join player with PlayerId to game with GameId
		// returns GameId of joined game on success, otherwise 0
		public int JoinGame(string playerId, int gameId)
		{
			_logger.LogInformation("Joining player [" + playerId + "] to game [" + gameId + "].");
			Game game = _repository.GetGame(gameId);
			if (game == null)
				return 0;

			if (game.MaxPlayers <= game.Players.Count)
				return 0;

			if (game.GameStarted)
				return 0;

			Player player = _repository.GetPlayer(playerId);
			if (player == null)
				return 0;

			game.Players.Add(player);
			_repository.SaveChanges();
			if (player.Game != game)
				return 0;

			_logger.LogInformation("Player [" + playerId + "] joined game [" + gameId + "]");

			return gameId;
		}

		// remove player with PlayerID from game with GameId
		public bool LeaveGame(string playerId) //, int? gameId = null)
		{
			Player player = _repository.GetPlayer(playerId);
			if (player == null)
				return false;

			if (player.GameId == null)
				return false;

			var gameId = player.GameId;

			Game game = _repository.GetGame((int)gameId);
			if (game == null)
				return false;

			if (game.Players.Remove(player))
			{
				_repository.SaveChanges();
				return true;
			}

			return false;
		}

		// play card
		public bool PlayCard(int gameId, Card card)
		{
			Game game = _repository.GetGame(gameId);
			if (game == null)
				return false;

			game.CardsPlayed.Add(card);

			card.UserId = null;
			_repository.SaveChanges();

			if (!game.CardsPlayed.Contains(card))
				return false;

			return true;
		}

		// serve cards 
		public bool ServeStartingHands(int gameId)
		{
			Game game = GetGame(gameId);
			if (game == null)
				return false;

			if (game.GameStarted != true)
				return false;

			if (game.Players.Count < game.MinPlayers || game.Players.Count > game.MaxPlayers)
				return false;

			for (int i = 0; i < game.StartingHand; i++)
			{
				foreach (Player player in game.Players)
				{
					var card = DrawCard(gameId);
					if (card == null)
						return false;

					player.Hand.Add(card);
				}
			}

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
			Player player = _repository.GetPlayer(playerId);
			if (player == null)
				return 0;

			Game game = player.Game;
			if (game == null)
				return 0;

			if (game.PlayersReady.Contains(player))
				return 0;

			game.PlayersReady.Add(player);
			_repository.SaveChanges();

			if (!game.PlayersReady.Contains(player))	// and make sure it was successful
				return 0;

			if (game.Players.Count < game.MinPlayers)
				return 1;

			if (game.PlayersReady.Count < game.Players.Count)
				return 2;

			if (game.Players.Count < game.MaxPlayers)
				return 3;

			return 4;
		}

		// initiate shuffling for game with GameId
		public void Shuffle(int gameId)
		{
			_repository.GetGame(gameId).CardsRemaining.Clear();
			List<Card> cards = _repository.GetGame(gameId).Set.Cards.ToList();
			
			Shuffle(cards, gameId);
		}

		// shuffle cards from list for game with GameId
		public void Shuffle(List<Card> cards, int gameId)
		{
			if (cards.Count < 1) return;
			Random rand = new Random();
			Card randomCard;
			while (cards.Count() > 0)
			{
				var position = rand.Next(0, cards.Count());
				randomCard = cards[position];
				_repository.GetGame(gameId).CardsRemaining.Add(randomCard);
				cards.RemoveAt(position);
			}
		}

		// start game with GameId
		public bool StartGame(int gameId)
		{
			Game game = _repository.GetGame(gameId);
			if (game == null)
				return false;

			if (game.MinPlayers > game.Players.Count || game.MaxPlayers < game.Players.Count)
				return false;
			
			Shuffle(gameId);
			game.GameStarted = true;
			ServeStartingHands(gameId);
			_repository.SaveChanges();

			return true;
		}
	}
}
