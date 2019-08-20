using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CardGame.Services
{
	public class PlayerService : IPlayerService
	{
		private readonly ICardsRepository _repository;
		private readonly ILogger _logger;
		private readonly IConfiguration _config;

		public PlayerService(ICardsRepository repository, IConfiguration config, ILogger<PlayerService> logger)
		{
			_repository = repository;
			_config = config;
			_logger = logger;
		}

		// for testing
		public PlayerService(ICardsRepository repository, ILogger<PlayerService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		// create new player
		// returns Player object on success, otherwise null
		public Player CreatePlayer(string playerId)
		{
			if (_repository.GetPlayer(playerId) != null)
				return null;

			_logger.LogInformation("Creating player [" + playerId + "] ...");
			Player player = new Player(playerId);
			_repository.AddPlayer(player);
			_logger.LogInformation("Player [" + playerId + "] has been created");
			return player;
		}

		// token creation for player authorization
		// returns JWT with claim for (ClaimTypes.NameIdentifier: {user})
		public string GenerateJWT(string user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user) };

			var token = new JwtSecurityToken(_config["Jwt:Issuer"],
			  _config["Jwt:Issuer"],
			  claims,
			  expires: DateTime.Now.AddMinutes(120),
			  signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		// get player by PlayerId
		public Player GetPlayer(string playerId) => _repository.GetPlayer(playerId);

		// get all players
		public List<Player> GetPlayers() => _repository.GetPlayers();
	}
}
