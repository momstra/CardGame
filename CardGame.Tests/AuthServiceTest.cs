using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using CardGame.Tests.FakeRepositories;
using CardGame.Services;
using CardGame.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Security.Claims;

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

			var response = _service.GetUserId(httpContext);

			Assert.Equal(userId, response);
		}
	}
}