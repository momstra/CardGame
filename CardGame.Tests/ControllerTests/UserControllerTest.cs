using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

using CardGame.API.Controllers;
using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeServices;
using CardGame.Tests.FakeRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CardGame.Tests
{
	public class UserControllerTest
	{
		private readonly UserController _controller;
		private readonly IPlayerService _service;
		private readonly IAuthService _authService;
		private readonly AuthRepository _authRepository;
		private readonly FakeServicesRepository _repository;

		public UserControllerTest()
		{
			_service = new FakePlayerService();
			_authService = new FakeAuthService();
			_authRepository = new AuthRepository();
			_repository = new FakeServicesRepository();
			_controller = new UserController(_service, new Mock<ILogger<UserController>>().Object, _authService);
		}

		[Fact]
		public void CreatePlayerTest()
		{
			var id = "TestplayerCreate";
			var before = _service.GetPlayers().Count;	// count before creation
			var result = _controller.CreatePlayer(id);
			var okResult = result as OkObjectResult;	// cast from IActionResult
			var after = _service.GetPlayers().Count;    // count after creation
			
			Assert.True(before == after - 1);							// player count before and after creation should differ
			Assert.Equal(id, ((TokenContainer)okResult.Value).Token);	// result should be "token" which is id for test purposes
		}

		[Fact]
		public void GetHandTest()
		{
			var id = "TestPlayerHand";
			var result = _controller.CreatePlayer(id);
			var okResult = result as OkObjectResult;  
			var token = ((TokenContainer)okResult.Value).Token;		

			var player = _service.GetPlayer(id);		// get player object
			List<Card> cards = new List<Card>()			// create cards for test player's hand
			{
				new Card()
				{
					Color = "T",
					Rank = "r1"
				},
				new Card()
				{
					Color = "T",
					Rank = "r2"
				},
			};
			player.Hand = cards;						// add cards to player

			List<string> cardsControl = new List<string>();			// create control for comparison
			foreach (Card card in cards)
				cardsControl.Add(card.Color + card.Rank);

			
			_controller.ControllerContext = _authRepository.CreateFakeControllerContext(id);	// set up context for UserController

			// Act
			JsonResult jsonResult = _controller.GetHand();

			// Assert
			Assert.Equal(cardsControl, jsonResult.Value);
			Assert.Contains("Tr1", cardsControl);
			Assert.Contains("Tr2", cardsControl);
			Assert.DoesNotContain("Tr3", cardsControl);
		}
	}
}
