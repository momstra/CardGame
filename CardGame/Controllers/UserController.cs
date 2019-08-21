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
		private readonly IGameService _gameService;
		private readonly IPlayerService _playerService;
		private readonly ILogger _logger;

		public UserController(IGameService gameService, IPlayerService playerService, ILogger<GameController> logger)
		{
			_gameService = gameService;
			_playerService = playerService;
			_logger = logger;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return Ok("Hello");
		}

		

		// player creation
		// {user} => PlayerId == username
		// returns JWT for players authorization
		[AllowAnonymous]
		[HttpGet("create/{user}")]
		public IActionResult CreatePlayer([FromRoute]string user)
		{
			if (_playerService.GetPlayer(user) != null)
				return NotFound("Name already in use.");

			var tokenString = _playerService.GenerateJWT(user);
			_playerService.CreatePlayer(user);
			return Ok(new { token = tokenString });
		}

		// ask for lisk of cards currently in hand
		[HttpGet("hand")]
		public JsonResult GetHand()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var cards = _playerService.GetPlayer(playerId).Hand.Cards;
			List<string> list = new List<string>();
			foreach (Card card in cards)
				list.Add(card.Color + card.Rank);

			return Json(list);
		}

		// get asking player's currently joined game
		[HttpGet("game")]
		public IActionResult GetPlayerGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return NotFound("");

			return Ok(game.GameId);
		}

		// join asking player to game with specified GameId
		// {id} => game to join
		// returns GameId of joined game on success
		[HttpGet("join/{id}")]
		public IActionResult JoinGame([FromRoute] int id)
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			var gameid = _gameService.JoinGame(playerId, id);
			if (gameid == id)
				return Ok(gameid);

			return NotFound("Could not join game");
		}

		// asking to remove player from currently joined game
		[HttpGet("leave")]
		public IActionResult LeaveGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			if (_gameService.LeaveGame(playerId))
				return Ok();

			return NotFound();
		}
		


		
	}
}