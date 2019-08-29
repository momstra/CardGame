using CardGame.Entities;
using CardGame.Services.Rules.Interfaces;

namespace CardGame.Tests.FakeRules
{
	class FakeGameRules : IGameRules
	{
		public bool CheckMoveLegal(Card card1, Card card2)
		{
			return true;
		}

		public bool MoveOnEmptyTable()
		{
			return true;
		}
	}
}
