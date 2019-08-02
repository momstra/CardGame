using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CardGame.Tests.FakeRepositories;
using CardGame.Business;
using CardGame.Entities;

namespace CardGame.Tests
{
	public class CardBusinessTest
	{
		private readonly FakeCardsRepository _repository;
		private readonly CardBusiness _business;

		public CardBusinessTest()
		{
			_repository = new FakeCardsRepository();
			_business = new CardBusiness(_repository);
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
