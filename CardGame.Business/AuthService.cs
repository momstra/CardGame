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
using Microsoft.AspNetCore.Http;

namespace CardGame.Services
{
	public class AuthService : IAuthService
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _config;
		private readonly ICardsRepository _repository;

		public AuthService(ICardsRepository repository, IConfiguration config, ILogger<AuthService> logger)
		{
			_repository = repository;
			_config = config;
			_logger = logger;
		}

		public string GetUserId(HttpContext context)
		{
			var currentUser = context.User;
			if (currentUser.Claims == null)
				return null;

			var playerId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
			if (_repository.GetPlayer(playerId) == null)				// making sure player still exists
				return null;

			return playerId;
		}
		
		// generate JWT with claim for (ClaimTypes.NameIdentifier: {user})
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
	}
}
