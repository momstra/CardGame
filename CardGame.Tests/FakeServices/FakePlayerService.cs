using System.Collections.Generic;

using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;

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
			Player player = _repository.Players.Find(p => p.PlayerId == playerId);
			Card card = _repository.Cards.Find(c => c.CardId == cardId);
			player.Hand.Add(card);
			return true;
		}

		public Player CreatePlayer(string playerId)
		{
			Player player = new Player();
			player.PlayerId = playerId;
			_repository.Players.Add(player);
			return player;
		}

		public List<string> GetHand(string playerId)
		{
			var hand = GetPlayer(playerId).Hand;

			if (hand == null)
				return null;

			List<string> list = new List<string>();
			foreach (Card card in hand)
				list.Add(card.Color + card.Rank);

			return list;
		}

		public Player GetPlayer(string playerId) => _repository.Players.Find(p => p.PlayerId == playerId);

		public List<Player> GetPlayers() => _repository.Players;

		public Card PlayCard(string playerId, int cardId)
		{
			throw new System.NotImplementedException();
		}

		public void RemovePlayer(string playerId)
		{
			throw new System.NotImplementedException();
		}

		public void SetHubId(string playerId, string hubId)
		{
			throw new System.NotImplementedException();
		}
	}
}
