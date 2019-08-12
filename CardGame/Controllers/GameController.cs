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

namespace CardGame.API.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
	{
		private readonly IGameBusiness _business;
		private readonly ICardsRepository _repository;
		private CardsContext _cardsContext;

		public GameController(IGameBusiness business, ICardsRepository repo, CardsContext ctx)
		{
			_repository = repo;
			_business = business;
			_cardsContext = ctx;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Ok("Hello");
		}

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