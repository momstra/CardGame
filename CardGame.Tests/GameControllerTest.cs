using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CardGame.API.Controllers;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CardGame.Tests
{
	public class GameControllerTest
	{
		private readonly GameController _controller;
		private readonly IGameService _gameService;
		private readonly IPlayerService _playerService;
		private readonly ILogger<GameController> _logger;

		public GameControllerTest()
		{
			var serviceProvider = new ServiceCollection()
				.AddLogging()
				.BuildServiceProvider();
			var factory = serviceProvider.GetService<ILoggerFactory>();
			_logger = factory.CreateLogger<GameController>();

			_gameService = new GameServiceFake();
			_playerService = new PlayerServiceFake();
			_controller = new GameController(_gameService, _playerService, _logger);
		}

		[Fact]
		public void CreatePlayer()
		{
			var id = "TestplayerCreate";
			var before = _playerService.GetPlayers().Count;
			var okResult = _controller.CreatePlayer(id);
			var after = _playerService.GetPlayers().Count;
			
			Assert.True(before == after - 1);
		}


	}
}
