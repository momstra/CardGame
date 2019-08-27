using CardGame.Entities;
using System.Collections.Generic;

namespace CardGame.Services.Interfaces
{
	public interface IPlayerService
	{
		bool AddCardToHand(int cardId, string playerId);
		Player CreatePlayer(string playerId);
		List<string> GetHand(string playerId);
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();
		Card PlayCard(string playerId, int cardId);
		void RemovePlayer(string playerId);
		void SetHubId(string playerId, string hubId);
	}
}
