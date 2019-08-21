using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

using CardGame.API.Controllers;
using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeServices;
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

		public UserControllerTest()
		{
			_service = new PlayerServiceFake();
			_authService = new AuthServiceFake();
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
			

			Assert.True(before == after - 1);	// player count before and after creation should differ
			Assert.Equal(id, ((TokenContainer)okResult.Value).Token);	// result should be "token" which is id for test purposes
		}

		[Fact]
		public void GetHandTest()
		{
			var id = "TestPlayerHand";
			var result = _controller.CreatePlayer(id);
			var okResult = result as OkObjectResult;    // cast from IActionResult
			var token = okResult.Value.ToString();		
			var player = _service.GetPlayer(id);		// get player object
			List<Card> cards = new List<Card>()			// create cards for test hand
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

			List<string> cardsControl = new List<string>();		// create control for comparison
			foreach (Card card in cards)
				cardsControl.Add(card.Color + card.Rank);

			var jsonControl = JsonConvert.SerializeObject(cardsControl);	

			player.Hand = new Hand()					// create test Hand
			{
				HandId = 1,
				Cards = cards,
			};

			var httpContext = new DefaultHttpContext();		// setup fake HttpContext for Authorization
			httpContext.Request.Headers["Authorization"] = "Bearer JWT" + id;

			var controllerContext = new ControllerContext()	// create fake ControllerContext
			{
				HttpContext = httpContext,
			};
			/*var controller = new UserController(_service, _logger)	
			{
				ControllerContext = controllerContext,
			};*/

			_controller.ControllerContext = controllerContext;	// and feed it to UserController

			// Act
			JsonResult jsonResult = _controller.GetHand();

			// Assert
			Assert.Equal(jsonControl, jsonResult.Value);
		}
	}
}
