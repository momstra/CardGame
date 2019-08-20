using System.Threading.Tasks;

namespace CardGame.API.Hubs
{
	public interface IGameClient
	{
		Task GameAdded(int gameId);
		Task JoinSuccess(int gameId);
		Task NewUser(string name);
		Task ReceiveMessage(string message);
		Task PlayerJoined(string username);
	}
}
