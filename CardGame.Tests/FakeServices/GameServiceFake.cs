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
using CardGame.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CardGame.Tests.FakeServices
{
	class GameServiceFake : IGameService
	{
		private readonly List<Game> _games;
		private readonly List<Player> _players;
		private readonly List<Card> _cards;

		public GameServiceFake()
		{
			_games = new List<Game>();
			_players = new List<Player>();
			_cards = new List<Card>();
		}

		public int CreateGame()
		{
			Random random = new Random();
			var gameId = random.Next(10, 20);
			var mockGame = new Mock<Game>();
			mockGame.Object.GameId = gameId;
			_games.Add(mockGame.Object);
			return gameId;
		}

		public Player CreatePlayer(string playerId)
		{
			var mockPlayer = new Mock<Player>();
			mockPlayer.Object.UserId = playerId;
			_players.Add(mockPlayer.Object);
			return mockPlayer.Object;
		}

		public Card DrawCard(int gameId)
		{
			throw new NotImplementedException();
		}

		public string GenerateJWT(string user)
		{
			return "JWT" + user;
		}

		public Game GetGame(int gameId) => _games.Find(g => g.GameId == gameId);

		public Game GetGame(string userId) => _players.Find(p => p.UserId == userId).Game;

		public List<Game> GetGames() => _games;

		public List<int> GetGamesList()
		{
			var list = new List<int>();
			foreach(Game game in _games)
				list.Add(game.GameId);

			return list;
		}

		public Player GetPlayer(string playerId) => _players.Find(p => p.UserId == playerId);

		public List<Player> GetPlayers() => _players;

		public List<Player> GetPlayers(int gameId) => _games.Find(g => g.GameId == gameId).Players.ToList();

		public List<string> GetPlayersIds(int gameId)
		{
			var list = new List<string>();
			var players = _games.Find(g => g.GameId == gameId).Players;
			foreach (Player player in players)
				list.Add(player.UserId);

			return list;
		}

		public int JoinGame(string playerId, int gameId)
		{
			var game = _games.Find(g => g.GameId == gameId);
			var player = _players.Find(p => p.UserId == playerId);
			game.Players.Add(player);

			return game.GameId;
		}

		public bool LeaveGame(string playerId, int? gameId = null)
		{
			var game = _games.Find(g => g.GameId == gameId);
			var player = _players.Find(p => p.UserId == playerId);
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
