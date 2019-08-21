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
			string playerId = "TestPlayerCG";
			var player = new Player(playerId);
			_repository.Players.Add(player);
			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(playerId);  // set up context for controller

			var result = _controller.CreateGame();
			var okResult = result as OkObjectResult;
			int gameId = (int)okResult.Value;
			var game = _repository.Games.Find(g => g.GameId == gameId);

			Assert.Equal(gameId, player.GameId);
			Assert.Contains(player, game.Players);
		}

		[Fact]
		public void DrawCardTest()
		{
			string playerId = "TestPlayerDC";
			var player = new Player(playerId);		// create player
			player.Hand = new Hand();
			player.Hand.Cards = new List<Card>();
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

			var result = _controller.DrawCard();
			var okResult = result as OkObjectResult;
			var cardResult = okResult.Value;

			Assert.Equal("Tr1", cardResult);
		}
	}
}
