using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CardGame.Entities;
using CardGame.Services;
using CardGame.Services.Interfaces;
using CardGame.Repositories;
using CardGame.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CardGame.API.Controllers
{
    [Route("api/game")]
	[Authorize]
    [ApiController]
    public class GameController : Controller
	{
		private readonly IGameService _service;
		private readonly IConfiguration _config;
		private readonly ILogger _logger;

		public GameController(IGameService service, IConfiguration config, ILogger<GameController> logger)
		{
			_service = service;
			_config = config;
			_logger = logger;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index()
		{
			return Ok("Hello");
		}

		[AllowAnonymous]
		[HttpGet("/api/game/user/create/{user}")]
		public IActionResult CreatePlayer([FromRoute]string user)
		{
			if (_service.GetPlayer(user) != null)
				return NotFound("Name already in use.");
			var tokenString = GenerateJWT(user);
			_service.CreatePlayer(user);
			return Ok(new { token = tokenString });
		}

		private string GenerateJWT(string user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new[] { new Claim("Username", user) };

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
			  _config["Jwt:Issuer"],
			  claims,
			  expires: DateTime.Now.AddMinutes(120),
			  signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[HttpGet("/api/game/user/{id}/game")]
		public IActionResult GetPlayerGame([FromRoute]string id)
		{
			Game game = _service.GetGame(id);
			if (game == null)
				return NotFound("");

			return Ok(game.GameId);
		}
		
		[HttpGet("/api/game/create")]
		public IActionResult CreateNewGame()
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == "Username").Value;
			_logger.LogInformation("Player [" + playerId + "] requested to create a new game");

			int gameId = _service.CreateNewGame();
			if (gameId != 0)
			{
				_service.JoinGame(playerId, gameId);
				return Ok(gameId);
			}

			return NotFound();
		}

		[HttpGet("/api/game/list/{id}")]
		public JsonResult GetGame([FromRoute]int id)
		{
			Game game = _service.GetGame(id);
			_logger.LogInformation("Player list: " + game.Players);
			return Json(game);
		}

		[HttpGet("/api/game/list")]
		public JsonResult GetGames()
		{
			return Json(_service.GetGames());
		}

		[HttpGet("/api/game/join/{id}")]
		public IActionResult JoinGame([FromRoute] int id)
		{
			var currentUser = HttpContext.User;
			string playerId = currentUser.Claims.FirstOrDefault(c => c.Type == "Username").Value;
			var gameid = _service.JoinGame(playerId, id);
			if (gameid == id)
				return Ok(gameid);

			return NotFound("Could not join game");
		}

		[HttpGet("/api/game/start/{id}")]
		public IActionResult StartGame([FromRoute] int id)
		{
			Game game = _service.GetGame(id);
			if (game != null)
			{
				if (_service.StartGame(id))
					return Ok();

				return NotFound("Game could not be started");
			}

			return NotFound("Game could not be found");
		}

		[HttpGet("/api/game/{gameid}/cards/draw")]
		public IActionResult DrawCard([FromRoute] int id)
		{

			Card card = _service.DrawCard(id);
			if (card != null)
				return Ok(card);

			return NotFound();
		}
	}
}