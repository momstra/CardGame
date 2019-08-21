using Microsoft.AspNetCore.Http;

namespace CardGame.Services.Interfaces
{
	public interface IAuthService
	{
		string GetUserId(HttpContext context);
		string GenerateJWT(string user);
	}
}
