using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CardGame.Tests.FakeRepositories;
using CardGame.Business;
using CardGame.Entities;

namespace CardGame.Tests
{
	public class GameBusinessTest
	{
		private readonly FakeCardsRepository _repository;
		private readonly GameBusiness _business;

		public GameBusinessTest()
		{
			_repository = new FakeCardsRepository();
			_business = new GameBusiness(_repository);
		}

		[Fact]
		public void CreateNewGameTest()
		{
			var id = _business.CreateNewGame();
			Assert.IsType<int>(id);
			Assert.InRange(id, 1000, 9999);
		}
	}
}
