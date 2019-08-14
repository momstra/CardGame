using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;


using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Repositories.Interfaces;

namespace CardGame.Services
{
	public class GameService : IGameService
	{
		private readonly ICardsRepository _repository;
		private readonly ILogger _logger;

		public GameService(ICardsRepository repository, ILogger<GameService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public bool CheckGameExists(int gameId)
		{
			if (_repository.GetGame(gameId) != null)
				return true;

			return false;
		}

		public bool CheckGameStatus(int gameId) => _repository.GetGame(gameId).GameStarted;

		public int CreateNewGame()
		{
			Random random = new Random();
			int id = random.Next(1000, 9999);

			if (_repository.GetGames().Count >= 8999)
				return 0;

			while (_repository.GetGame(id) != null)
				id = random.Next(1000, 9999);

			if (_repository.AddGame(id) == true)
				return id;

			return 0;
		}

		public Player CreatePlayer(string playerId)
		{
			if (_repository.GetPlayer(playerId) != null)
				return null;

			Player player = new Player(playerId);
			_repository.AddPlayer(player);
			return player;
		}

		public Card DrawCard(int gameId)
		{
			if (_repository.GetCardsRemaining(gameId).Count > 0)
				return _repository.GetCardsRemaining(gameId).Dequeue();

			return null;
		}

		public Game GetGame(int gameId) => _repository.GetGame(gameId);

		public List<int> GetGamesList()
		{
			var games = _repository.GetGames();
			List<int> gameIds = new List<int>();
			foreach (Game game in games)
				gameIds.Add(game.GameId);

			return gameIds;
		}

		public List<Game> GetGames() => _repository.GetGames();

		public Player GetPlayer(string playerId) => _repository.GetPlayer(playerId);

		public List<Player> GetPlayers() => _repository.GetPlayers();

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

			Player player = _repository.GetPlayers().Find(p => p.UserId == playerId);
			if (player == null)
				return 0;

			//game.Players.Add(player);
			player.GameId = gameId;
			_repository.SaveChanges();
			_logger.LogInformation("Player list: " + game.Players);

			return gameId;
		}

		public void Shuffle(int gameId)
		{
			_repository.GetGame(gameId).CardsRemaining.Clear();
			List<Card> cards = _repository.GetDeck().ToList();
			Shuffle(cards, gameId);
		}

		public void Shuffle(List<Card> cards, int gameId)
		{
			if (cards.Count < 1) return;
			Random rand = new Random();
			Card randomCard;
			while (cards.Count() > 0)
			{
				var position = rand.Next(0, cards.Count());
				randomCard = cards[position];
				_repository.GetCardsRemaining(gameId).Enqueue(randomCard);
				cards.RemoveAt(position);
			}
		}

		public bool StartGame(int gameId)
		{
			Game game = _repository.GetGame(gameId);
			if (game != null)
			{
				if (game.MinPlayers <= game.Players.Count && game.MaxPlayers >= game.Players.Count)
				{
					Shuffle(gameId);
					game.GameStarted = true;
					_repository.SaveChanges();
					return true;
				}
			}
			return false;
		}
	}
}
