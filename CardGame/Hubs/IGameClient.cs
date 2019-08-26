using System.Threading.Tasks;

namespace CardGame.API.Hubs
{
	public interface IGameClient
	{
		Task AllReady();
		Task AllReadyWaiting();
		Task AwaitingPlayersReady();
		Task AwaitingPlayersToJoin();

		Task GameAdded(int gameId);
		Task GameReady();
		Task GameStarted();
		Task JoinSuccess(int gameId);
		Task LeaveSuccess();
		Task NewUser(string name);
		Task ReceiveMessage(string message);
		Task PlayerJoined(string username);
		Task PlayerLeft(string username);
	}
}
