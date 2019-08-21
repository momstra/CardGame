using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CardGame.Tests.FakeRepositories
{
	public class AuthRepository
	{
		public HttpContext CreateFakeContext(string userId)
		{
			var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
			var httpContext = new DefaultHttpContext()
			{
				User = new ClaimsPrincipal(new ClaimsIdentity(claims))
			};

			return httpContext;
		}
	}
}
