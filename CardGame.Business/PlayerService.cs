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

		public PlayerService(ICardsRepository repository, ILogger<PlayerService> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public bool AddCardToHand(int cardId, string playerId)
		{
			Player player = _repository.GetPlayer(playerId);
			Card card = _repository.GetCard(cardId);

			player.Hand.Add(card);
			if (card.UserId != playerId)
				return false;

			return true;
		} 
		
		// returns Player object on success, otherwise null
		public Player CreatePlayer(string playerId)
		{
			if (_repository.GetPlayer(playerId) != null)
				return null;

			_logger.LogInformation("Creating player [" + playerId + "] ...");
			Player player = _repository.CreatePlayer(playerId);
			if (player == null)
			{
				_logger.LogInformation("Player creation failed [" + playerId + "]");
				return null;
			}

			_logger.LogInformation("Player [" + playerId + "] has been created");
			return player;
		}

		public List<Card> GetHand(string playerId)
		{
			var player = _repository.GetPlayer(playerId);
			if (player == null)
				return null;

			return player.Hand.ToList();
		}

		// get player by PlayerId
		public Player GetPlayer(string playerId) => _repository.GetPlayer(playerId);

		// get all players
		public List<Player> GetPlayers() => _repository.GetPlayers();

		public void SetHubId(string playerId, string hubId)
		{
			GetPlayer(playerId).HubId = hubId;
			_repository.SaveChanges();
		}
	}
}
