using CardGame.Entities;
using System.Collections.Generic;

namespace CardGame.Services.Interfaces
{
	public interface IPlayerService
	{
		Player CreatePlayer(string playerId);
		string GenerateJWT(string user);
		Player GetPlayer(string playerId);
		List<Player> GetPlayers();

	}
}
