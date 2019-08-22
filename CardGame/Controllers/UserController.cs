using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CardGame.Entities;
using CardGame.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace CardGame.API.Controllers
{
    [Route("api/user")]
	[Authorize]
    public class UserController : BaseController
	{
		private readonly IPlayerService _playerService;
		private readonly ILogger _logger;
		private readonly IAuthService _authService;

		public UserController(IPlayerService playerService, ILogger<UserController> logger, IAuthService authService)
		{
			_playerService = playerService;
			_logger = logger;
			_authService = authService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index() => Ok("Hello");
				

		// player creation
		// {user} => PlayerId == username
		// returns JWT for players authorization
		[AllowAnonymous]
		[HttpGet("create/{user}")]
		public IActionResult CreatePlayer([FromRoute]string user)
		{
			if (_playerService.GetPlayer(user) != null)
				return NotFound("Name already in use.");

			var tokenString = _authService.GenerateJWT(user);// _playerService.GenerateJWT(user);
			_playerService.CreatePlayer(user);
			return Ok(new TokenContainer(tokenString));
		}

		// ask for lisk of cards currently in hand
		[HttpGet("hand")]
		public JsonResult GetHand()
		{
			var playerId = _authService.GetUserId(HttpContext);
			var player = _playerService.GetPlayer(playerId);
			if (player == null)
				return null;

			var cards = player.Hand;
			if (cards == null)
				return null;

			List<string> list = new List<string>();
			foreach (Card card in cards)
				list.Add(card.Color + card.Rank);

			return Json(list);
		}


		
	}
}