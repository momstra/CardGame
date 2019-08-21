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
    [Route("api/game")]
	[Authorize]
    //[ApiController]
    public class GameController : BaseController
	{
		private readonly IGameService _gameService;
		private readonly IPlayerService _playerService;
		private readonly ILogger _logger;

		public GameController(IGameService gameService, IPlayerService playerService, ILogger<GameController> logger)
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


		// create new game and join asking player
		// returns new game's GameId on success
		[HttpGet("create")]
		public IActionResult CreateGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			_logger.LogInformation("Player [" + playerId + "] requested to create a new game");

			int gameId = _gameService.CreateGame();
			if (gameId != 0)
			{
				_gameService.JoinGame(playerId, gameId);
				return Ok(gameId);
			}

			return NotFound();
		}

		// player asking to draw card 
		// returns drawn card on success
		[HttpGet("draw")]
		public IActionResult DrawCard()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			Game game = _gameService.GetGame(playerId);
			if (game != null && game.GameStarted)
			{
				Card card = _gameService.DrawCard(game.GameId);
				if (card != null)
					return Ok(card.Color + card.Rank);
			}

			return NotFound();
		}

		/*		__if needed again, implement with care
				// return serialized object for specified game
				// {id} => game to return
				[HttpGet("/api/game/show/{id}")]
				public JsonResult GetGame([FromRoute]int id)
				{
					Game game = _service.GetGame(id);
					_logger.LogInformation("Player list: " + game.Players);
					return Json(game);
				}
		*/

		// asks for player's current game
		// returns serialized game object
		[HttpGet("show")]
		public JsonResult GetGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			Game game = _gameService.GetGame(playerId);

			return Json(game);
		}

		// ask for list of all games
		// returns serialized List<int> of all GameIds
		[HttpGet("list")]
		public JsonResult GetGames()
		{
			return Json(_gameService.GetGamesList());
		}

		// ask for players in current game
		// returns serialized List<string> of PlayerIds
		[HttpGet("users")]
		public JsonResult GetGamePlayers()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			Game game = _gameService.GetGame(playerId);
			return Json(_gameService.GetPlayersIds(game.GameId));
		}

		// player asking to start current game
		[HttpGet("/api/game/start")]
		public IActionResult StartGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			Game game = _gameService.GetGame(playerId);
			if (game != null)
			{
				if (_gameService.StartGame(game.GameId))
					return Ok();

				return NotFound("Game could not be started");
			}

			return NotFound("Game could not be found");
		}
	}
}