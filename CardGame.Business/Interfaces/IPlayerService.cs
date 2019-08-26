using CardGame.Entities;
using System.Collections.Generic;

namespace CardGame.Services.Interfaces
{
	public interface IPlayerService
	{
		bool AddCardToHand(int cardId, string playerId);
		Player CreatePlayer(string playerId);
		List<Card> GetHand(string playerId);
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();
		void SetHubId(string playerId, string hubId);
	}
}
