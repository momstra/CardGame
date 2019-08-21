using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CardGame.Tests.FakeRepositories
{
	public class AuthRepository
	{
		public ControllerContext CreateFakeControllerContext(string userId)
		{
			var httpContext = CreateFakeContext(userId);
			var controllerContext = new ControllerContext()                 // create fake ControllerContext
			{
				HttpContext = httpContext,
			};

			return controllerContext;
		}

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
