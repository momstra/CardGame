using CardGame.Entities;
using System.Collections.Generic;

namespace CardGame.Services.Interfaces
{
	public interface IPlayerService
	{
		bool AddCardToHand(int cardId, int handId);
		Player CreatePlayer(string playerId);
//		string GenerateJWT(string user);
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();

	}
}
