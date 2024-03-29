using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

using CardGame.API.Hubs;

namespace CardGame.Tests
{
	public class GameHubTest
	{
		private HubConnection _mockHub;

		public GameHubTest()
		{
			// Mock Startup
			var webHostBuilder = new WebHostBuilder()
			.ConfigureServices(services =>
			{
				
				services.AddSignalR();
			})
			.Configure(app =>
			{
				app.UseSignalR(routes => routes.MapHub<GameHub>("/game"));
			});

			// Initiate TestServer
			TestServer server = new TestServer(webHostBuilder);

			// Initiate mock hub
			_mockHub = new HubConnectionBuilder()
				.WithUrl("ws://localhost/game",
				o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
				.Build();
		}

		//[Fact]
		public async Task HubIsWorking()
		{
			var message = "GameHub message";
			var answer = string.Empty;

			_mockHub.On<string>("ReceiveMessage", msg =>
			{
				answer = msg;
			});

			await _mockHub.StartAsync();
			await _mockHub.InvokeAsync("SendMessage", message);

			Assert.Equal(message, answer);
		}
	}
}
