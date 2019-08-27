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

		public List<String> GetHand(string playerId)
		{
			var hand = _repository.GetPlayer(playerId).Hand;

			if (hand == null)
				return null;

			List<string> list = new List<string>();
			foreach (Card card in hand)
				list.Add(card.Color + card.Rank);

			return list;
		}

		// get player by PlayerId
		public Player GetPlayer(string playerId) => _repository.GetPlayer(playerId);

		// get all players
		public List<Player> GetPlayers() => _repository.GetPlayers();

		public Card PlayCard(string playerId, int cardId)
		{
			var player = GetPlayer(playerId);
			var card = _repository.GetCard(cardId);

			if (card.Owner != player)
				return null;

			if (player.Hand.Remove(card))
			{
				_repository.SaveChanges();
				return card;
			}

			return null;
		}

		public void RemovePlayer(string playerId) => _repository.RemovePlayer(playerId);

		public void SetHubId(string playerId, string hubId)
		{
			GetPlayer(playerId).HubId = hubId;
			_repository.SaveChanges();
		}
	}
}
