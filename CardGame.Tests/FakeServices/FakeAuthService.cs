using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Moq;

using CardGame.Entities;
using CardGame.Services.Interfaces;
using CardGame.Tests.FakeRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

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
