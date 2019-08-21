using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Moq;

using CardGame.Entities;
using CardGame.Services;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;
using CardGame.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CardGame.Tests.FakeServices
{
	public class PlayerServiceFake : IPlayerService
	{
		private readonly FakeServicesRepository _repository;

		public PlayerServiceFake()
		{
			_repository = new FakeServicesRepository();
			}

		public Player CreatePlayer(string playerId)
		{
			Player player = new Player();
			player.UserId = playerId;
			player.Hand = new Hand();
			_repository.Players.Add(player);
			return player;
		}

		public Player GetPlayer(string playerId) => _repository.Players.Find(p => p.UserId == playerId);

		public List<Player> GetPlayers() => _repository.Players;
	}
}
