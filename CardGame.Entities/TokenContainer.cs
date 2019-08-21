using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Entities
{
	public class TokenContainer
	{
		public string Token { get; set; }

		public TokenContainer(string token)
		{
			Token = token;
		}
	}
}
