using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CardGame.API.Controllers;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeServices;
using CardGame.Tests.FakeRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CardGame.Entities;

namespace CardGame.Tests
{
	public class GameControllerTest
	{
		private readonly GameController _controller;
		private readonly AuthRepository _authRepository;
		private readonly FakeServicesRepository _repository;
		private readonly IAuthService _authService;
		private readonly IGameService _gameService;
		private readonly IPlayerService _playerService;

		public GameControllerTest()
		{
			_authRepository = new AuthRepository();
			_repository = new FakeServicesRepository();
			_gameService = new FakeGameService(_repository);
			_authService = new FakeAuthService();
			_playerService = new FakePlayerService();
			_controller = new GameController(_gameService, _authService, new Mock<ILogger<GameController>>().Object);
		}
		
		[Fact]
		public void CreateGameTest()
		{
			string playerId = "TestPlayerCreate";
			var player = new Player(playerId);
			_repository.Players.Add(player);
			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(playerId);  // set up context for controller

			// Act
			var result = _controller.CreateGame();
			var okResult = result as OkObjectResult;
			int gameId = (int)okResult.Value;
			var game = _repository.Games.Find(g => g.GameId == gameId);

			// Assert
			Assert.Equal(gameId, player.GameId);		// player's GameId should be gameId
			Assert.Contains(player, game.Players);		// game's players should contain player
		}

		[Fact]
		public void DrawCardTest()
		{
			string playerId = "TestPlayerDraw";
			var player = new Player(playerId);		// create player
			_repository.Players.Add(player);		// add player to database

			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(playerId);  // set up context for controller

			int gameId = 1;	
			var game = new Game(gameId);		// create game and add player
			game.Players = new List<Player>()
			{
				player,
			};
			player.Game = game;
			player.GameId = gameId;

			Card card1 = new Card()			// create cards for test deck
			{
				Color = "T",
				Rank = "r1"
			};
			Card card2 = new Card()
			{
				Color = "T",
				Rank = "r2"
			};
			List<Card> cards = new List<Card>()		
			{
				card1,
				card2,
			};

			Deck deck = new Deck()			// create deck for test game
			{
				Cards = cards,
			};
			game.Deck = deck;
			game.GameStarted = true;		// "start" test game 
			_repository.Games.Add(game);	// add game to database

			// Act
			var result = _controller.DrawCard();
			var okResult = result as OkObjectResult;
			var cardResult = okResult.Value;

			// Assert
			Assert.Equal(card1.Color + card1.Rank, cardResult);		// return should be Color+Rank from first card in deck
		}

		[Fact]
		public void GetPlayerGameTest()
		{
			string playerId = "TestPlayerGet";
			var player = new Player(playerId);      // create player
			_repository.Players.Add(player);        // add player to database

			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(playerId);  // set up context for controller

			int gameId = 1;
			var game = new Game(gameId);        // create game and add player
			game.Players = new List<Player>()
			{
				player,
			};
			player.Game = game;
			player.GameId = gameId;
			_repository.Games.Add(game);    // add game to database

			// Act
			var result = _controller.GetPlayerGame();
			var okResult = result as OkObjectResult;
			var gameIdResult = okResult.Value;

			// Assert
			Assert.Equal(player.GameId, gameIdResult);	// should return gameId of player's joined game
		}

		[Fact]
		public void JoinGameTest()
		{
			string playerId = "TestPlayerJoin";
			var player = new Player(playerId);      // create player
			_repository.Players.Add(player);        // add player to database

			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(playerId);  // set up context for controller

			int gameId = 1;
			var game = new Game(gameId);        // create game to join
			_repository.Games.Add(game);		// add game to database

			// Act
			var result = _controller.JoinGame(gameId);	// existing game
			var okResult = result as OkObjectResult;
			var gameIdResult = okResult.Value;

			result = _controller.JoinGame(1000000);     // non existing game
			var notFoundResult = result as NotFoundObjectResult;

			// Assert
			Assert.Equal(player.GameId, gameIdResult);  // should return gameId of player's joined game
			Assert.Equal(game, player.Game);			// player's game should be game
			Assert.Contains(player, game.Players);      // game's players list should contain player

			Assert.IsType<NotFoundObjectResult>(notFoundResult);	// non existing gameId should return NotFound
		}
	}
}
