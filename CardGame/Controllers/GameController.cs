using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CardGame.Entities;
using CardGame.Business;
using CardGame.Business.Interfaces;
using CardGame.Repositories;
using CardGame.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CardGame.API.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
	{
		private readonly IGameBusiness _business;
		private readonly ICardsRepository _repository;
		private CardsContext _cardsContext;
		private IConfiguration _config;

		public GameController(IGameBusiness business, ICardsRepository repo, CardsContext ctx, IConfiguration config)
		{
			_repository = repo;
			_business = business;
			_cardsContext = ctx;
			_config = config;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Ok("Hello");
		}

		[HttpGet("/api/game/user/create/{user}")]
		public IActionResult CreatePlayer([FromRoute]string user)
		{
			if (_repository.GetPlayer(user) != null)
				return NotFound("Name already in use.");
			var tokenString = GenerateJWT(user);
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

		[Authorize]
		[HttpGet("/api/game/create")]
		public IActionResult CreateNewGame()
		{
			int id = _business.CreateNewGame();
			if (id != 0)
				return Ok(id);

			return NotFound();
		}
		
		[HttpGet("/api/game/list")]
		public IActionResult GetGames()
		{
			return Ok(_repository.Games);
		}

		[HttpGet("/api/game/{gameid}/cards/draw")]
		public IActionResult DrawCard([FromRoute] int gameid)
		{

			Card card = _business.DrawCard(gameid);
			if (card != null)
				return Ok(card);

			return NotFound();
		}
	}
}