using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Moq;

using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;
using Microsoft.Extensions.Configuration;

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

		public int CreateGame()
		{
			Random random = new Random();
			var gameId = random.Next(10, 20);
			var game = new Game(gameId);
			_repository.Games.Add(game);
			return gameId;
		}

		public Card DrawCard(int gameId)
		{
			throw new NotImplementedException();
		}

		public Game GetGame(int gameId) => _repository.Games.Find(g => g.GameId == gameId);

		public Game GetGame(string userId) => _repository.Players.Find(p => p.UserId == userId).Game;

		public List<Game> GetGames() => _repository.Games;

		public List<int> GetGamesList()
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
				list.Add(player.UserId);

			return list;
		}

		public int JoinGame(string playerId, int gameId)
		{
			var game = _repository.Games.Find(g => g.GameId == gameId);
			var player = _repository.Players.Find(p => p.UserId == playerId);
			game.Players.Add(player);
			player.Game = game;
			player.GameId = gameId;

			return game.GameId;
		}

		public bool LeaveGame(string playerId, int? gameId = null)
		{
			var game = _repository.Games.Find(g => g.GameId == gameId);
			var player = _repository.Players.Find(p => p.UserId == playerId);
			return game.Players.Remove(player);
		}

		public void Shuffle(int gameId)
		{
			throw new NotImplementedException();
		}

		public void Shuffle(List<Card> cards, int gameId)
		{
			throw new NotImplementedException();
		}

		public bool StartGame(int gameId)
		{
			throw new NotImplementedException();
		}
	}
}
