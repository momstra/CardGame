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
			Player player = _repository.Players.Find(p => p.UserId == playerId);
			Card card = _repository.Cards.Find(c => c.CardId == cardId);
			player.Hand.Add(card);
			return true;
		}

		public Player CreatePlayer(string playerId)
		{
			Player player = new Player();
			player.UserId = playerId;
			_repository.Players.Add(player);
			return player;
		}

		public List<Card> GetHand(string playerId)
		{
			return null;
		}

		public Player GetPlayer(string playerId) => _repository.Players.Find(p => p.UserId == playerId);

		public List<Player> GetPlayers() => _repository.Players;

		public void SetHubId(string playerId, string hubId)
		{
			throw new System.NotImplementedException();
		}
	}
}
