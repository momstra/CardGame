using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CardGame.Services;
using CardGame.Tests.FakeRepositories;

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
			_service = new AuthService(new Mock<IConfiguration>().Object, new Mock<ILogger<AuthService>>().Object);
		}

		[Fact]
		public void GetUserIdTest()
		{
			string userId = "TestUser";
			var httpContext = _authRepository.CreateFakeContext(userId);

			// Act
			var response = _service.GetUserId(httpContext);

			// Assert
			Assert.Equal(userId, response);		// returned "token" is userId for test purposes
		}
	}
}