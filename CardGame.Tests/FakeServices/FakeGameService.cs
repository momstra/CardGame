using System;
using System.Collections.Generic;
using System.Linq;

using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;

namespace CardGame.Tests.FakeServices
{
	public class FakeGameService : IGameService
	{
		private readonly FakeServicesRepository _repository;

		public FakeGameService()
		{
			_repository = new FakeServicesRepository();
		}

		public FakeGameService(FakeServicesRepository repository)
		{
			_repository = repository;
		}

		public int CreateGame(string playerId)
		{
			Random random = new Random();
			var gameId = random.Next(10, 20);
			var game = new Game(gameId);
			game.Set = new Set();
			_repository.Games.Add(game);

			return gameId;
		}

		public Card DrawCard(int gameId)
		{
			Game game = GetGame(gameId);
			if (game == null)
				return null;

			List<Card> cards = game.Set.Cards as List<Card>;
			if (cards == null)
				return null;

			Card card = cards[0];
			cards.RemoveAt(0);

			return card;
		}

		public Game GetGame(int gameId) => _repository.Games.Find(g => g.GameId == gameId);

		public Game GetGame(string userId) => _repository.Players.Find(p => p.PlayerId == userId).Game;

		public List<Game> GetGames() => _repository.Games;

		public List<int> GetGameIdsList()
		{
			var list = new List<int>();
			foreach(Game game in _repository.Games)
				list.Add(game.GameId);

			return list;
		}

		public List<Player> GetPlayers(int gameId) => _repository.Games.Find(g => g.GameId == gameId).Players.ToList();

		public List<string> GetPlayersIds(int gameId)
		{
			var list = new List<string>();
			var players = _repository.Games.Find(g => g.GameId == gameId).Players;
			foreach (Player player in players)
				list.Add(player.PlayerId);

			return list;
		}

		public string GetTurnPlayer(int gameId)
		{
			throw new NotImplementedException();
		}

		public int JoinGame(string playerId, int gameId)
		{
			var game = _repository.Games.Find(g => g.GameId == gameId);
			if (game == null)
				return 0;

			var player = _repository.Players.Find(p => p.PlayerId == playerId);
			if (player == null)
				return 0;

			game.Players.Add(player);
			player.Game = game;
			player.GameId = gameId;

			return game.GameId;
		}

		public bool LeaveGame(string playerId)
		{
			var player = _repository.Players.Find(p => p.PlayerId == playerId);
			if (player == null)
				return false;

			var gameId = player.GameId;

			var game = _repository.Games.Find(g => g.GameId == gameId);
			if (game == null)
				return false;

			var playerList = game.Players as List<Player>;
			if (playerList.Find(p => p.PlayerId == playerId) == null)
				return false;

			player.Game = null;
			player.GameId = null;
			return game.Players.Remove(player);
		}

		public Player MoveToNextPlayer(int gameId)
		{
			throw new NotImplementedException();
		}

		public object PlayCard(int gameId, Card card)
		{
			throw new NotImplementedException();
		}

		public bool RemoveGame(int gameId)
		{
			throw new NotImplementedException();
		}

		public bool ServeStartingHands(int gameId)
		{
			throw new NotImplementedException();
		}

		public byte SetPlayerReady(string playerId)
		{
			throw new NotImplementedException();
		}

		public bool StartGame(int gameId)
		{
			var game = _repository.Games.Find(g => g.GameId == gameId);
			if (game == null)
				return false;

			if (game.MinPlayers > game.Players.Count || game.MaxPlayers < game.Players.Count)
				return false;

			game.GameStarted = true;

			return true;
		}

		// only for Interface implementation
		// Shuffle methods are not needed for Controller testing
		public void Shuffle(int gameId) => throw new NotImplementedException();

		public void Shuffle(List<Card> cards, int gameId) => throw new NotImplementedException();
	}
}
