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

		[Fact]
		public void StartGameTest()
		{
			var id = _business.CreateNewGame();
			Assert.NotEqual(0, id);
			var game = _repository.GetGame(id);
			var cards = game.CardsRemaining;
			Assert.IsType<Queue<Card>>(cards);
			Assert.Empty(cards);
			_business.StartGame(id);
			Assert.NotEmpty(cards);
			Assert.True(_repository.GetGame(id).GameStarted);
		}
		
		[Fact]
		public void ShuffleTest()
		{
			_business.Shuffle(0);
			Card card1 = _repository.GetCardsRemaining(0).Dequeue();
			int turn = 0;
			bool areDifferent = false;

			do
			{
				_business.Shuffle(0);
				Card card2 = _repository.GetCardsRemaining(0).Dequeue();
				if (card1 != card2)
				{
					areDifferent = true;
					break;
				}

				turn++;
			} while (turn < 100);

			Assert.True(areDifferent);
		}
		
		[Fact]
		public void DrawCardTest()
		{
			_business.Shuffle(0);
			int numberOfCards = _repository.GetCardsRemaining(0).Count;
			Assert.IsAssignableFrom<Card>(_business.DrawCard(0));
			Assert.NotNull(_business.DrawCard(0));
			Assert.NotEqual(numberOfCards, _repository.GetCardsRemaining(0).Count);
		}
	}
}
