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
	public class FakePlayerService : IPlayerService
	{
		private readonly FakeServicesRepository _repository;

		public FakePlayerService()
		{
			_repository = new FakeServicesRepository();
		}

		public bool AddCardToHand(int cardId, string playerId)
		{
			Player player = _repository.Players.Find(p => p.UserId == playerId);
			Card card = _repository.Cards.Find(c => c.CardId == cardId);
			player.Cards.Add(card);
			return true;
		}

		public Player CreatePlayer(string playerId)
		{
			Player player = new Player();
			player.UserId = playerId;
			_repository.Players.Add(player);
			return player;
		}

		public Player GetPlayer(string playerId) => _repository.Players.Find(p => p.UserId == playerId);

		public List<Player> GetPlayers() => _repository.Players;
	}
}
