using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;

namespace CardGame.Tests.FakeServices
{
	public class FakeAuthService : IAuthService
	{
		private readonly FakeServicesRepository _repository;

		public FakeAuthService()
		{
			_repository = new FakeServicesRepository();
		}
		
		public string GenerateJWT(string user)
		{
			return user;
		}

		public string GetUserId(HttpContext context)
		{
			return context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
		}
	}
}
