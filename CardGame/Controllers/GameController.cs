using CardGame.Entities;
using CardGame.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace CardGame.API.Controllers
{
    [Route("api/game")]
	[Authorize]
    public class GameController : BaseController
	{
		private readonly IGameService _gameService;
		private readonly IAuthService _authService;
		private readonly ILogger _logger;

		public GameController(IGameService gameService, IAuthService authService, ILogger<GameController> logger)
		{
			_gameService = gameService;
			_authService = authService;
			_logger = logger;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index() => Ok("Hello");


		// create game and join asking player
		// returns GameId on success
		[HttpGet("create")]
		public IActionResult CreateGame()
		{
			string playerId = _authService.GetUserId(HttpContext);
			_logger.LogInformation("Player [" + playerId + "] requested to create a new game");

			int gameId = _gameService.CreateGame();
			if (gameId == 0)
				return NotFound("Game could not be created");
		
			if (_gameService.JoinGame(playerId, gameId) == 0)
				return NotFound("Player could not be joined");

			return Ok(gameId);
		}
		
		// returns drawn card on success
		[HttpGet("draw")]
		public IActionResult DrawCard()
		{
			string playerId = _authService.GetUserId(HttpContext);
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return NotFound("Game not found");

			if (!game.GameStarted)
				return NotFound("Game not started");

			Card card = _gameService.DrawCard(game.GameId);
			if (card == null)
				return NotFound("Card not found");
			
			return Ok(card.Color + card.Rank);
		}

		// get asking player's currently joined game's id
		[HttpGet("get")]
		public IActionResult GetPlayerGame()
		{
			string playerId = _authService.GetUserId(HttpContext);
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return NotFound("");

			return Ok(game.GameId);
		}
		
		// {id} => game to join
		// returns GameId of joined game on success
		[HttpGet("join/{id}")]
		public IActionResult JoinGame([FromRoute] int id)
		{
			string playerId = _authService.GetUserId(HttpContext);
			var gameid = _gameService.JoinGame(playerId, id);
			if (gameid != id)
				return NotFound("Could not join game");

			return Ok(gameid);
		}

		// asking to remove player from currently joined game
		[HttpGet("leave")]
		public IActionResult LeaveGame()
		{
			string playerId = _authService.GetUserId(HttpContext);
			if (!_gameService.LeaveGame(playerId))
				return NotFound();

			return Ok();
		}
		
		// returns serialized List<int> of all existing game's ids
		[HttpGet("list")]
		public JsonResult GetGames() => Json(_gameService.GetGamesList());
		
		// returns serialized game object for player's current game
		[HttpGet("show")]
		public JsonResult GetGame()
		{
			string playerId = _authService.GetUserId(HttpContext);
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return null;

			return Json(game);
		}

		// player asking to start current game
		[HttpGet("start")]
		public IActionResult StartGame()
		{
			string playerId = _authService.GetUserId(HttpContext);
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return NotFound("Game could not be found");

			if (!_gameService.StartGame(game.GameId))
				return NotFound("Game could not be started");

			return Ok();
		}

		// returns serialized List<string> of current game's joined player's ids
		[HttpGet("users")]
		public JsonResult GetGamePlayers()
		{
			string playerId = _authService.GetUserId(HttpContext);
			Game game = _gameService.GetGame(playerId);
			if (game == null)
				return null;

			return Json(_gameService.GetPlayersIds(game.GameId));
		}
	}
}