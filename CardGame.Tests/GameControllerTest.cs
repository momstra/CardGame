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
		private readonly IGameService _service;
		private readonly ILogger<GameController> _logger;

		public GameControllerTest()
		{
			var serviceProvider = new ServiceCollection()
				.AddLogging()
				.BuildServiceProvider();
			var factory = serviceProvider.GetService<ILoggerFactory>();
			_logger = factory.CreateLogger<GameController>();

			_service = new GameServiceFake();
			_controller = new GameController(_service, _logger);
		}

		[Fact]
		public void CreatePlayer()
		{
			var id = "TestplayerCreate";
			var before = _service.GetPlayers().Count;
			var okResult = _controller.CreatePlayer(id);
			var after = _service.GetPlayers().Count;
			
			Assert.True(before == after - 1);
		}


	}
}
