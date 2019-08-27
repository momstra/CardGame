using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CardGame.Services;
using CardGame.Tests.FakeRepositories;
using CardGame.Repositories.Interfaces;

namespace CardGame.Tests
{
	public class AuthServiceTest
	{
		private readonly FakeCardsRepository _repository;
		private readonly AuthService _service;
		private readonly AuthRepository _authRepository;

		public AuthServiceTest()
		{
			_repository = new FakeCardsRepository();
			_authRepository = new AuthRepository();
			_service = new AuthService(_repository, new Mock<IConfiguration>().Object, new Mock<ILogger<AuthService>>().Object);
		}

		[Fact]
		public void GetUserIdTest()
		{
			string userId = "TestUser";
			_repository.CreatePlayer(userId);
			var httpContext = _authRepository.CreateFakeContext(userId);

			// Act
			var response1 = _service.GetUserId(httpContext);	// should return playerId httpContext (claim) was created with

			_repository.RemovePlayer(userId);
			var response2 = _service.GetUserId(httpContext);	// should return null as player does not exist anymore

			// Assert
			Assert.Equal(userId, response1);                    // should have returned correct id

			Assert.Null(response2);								// check for player's existence should have failed
		}
	}
}