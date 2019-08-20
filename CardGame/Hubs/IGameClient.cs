using System.Threading.Tasks;

namespace CardGame.API.Hubs
{
	public interface IGameClient
	{
		Task GameAdded(int gameId);
		Task GameStarted();
		Task JoinSuccess(int gameId);
		Task LeaveSuccess();
		Task NewUser(string name);
		Task ReceiveMessage(string message);
		Task PlayerJoined(string username);
		Task PlayerLeft(string username);
	}
}
