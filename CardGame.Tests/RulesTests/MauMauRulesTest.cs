using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using CardGame.Entities;
using CardGame.Services.Rules;
using CardGame.Tests.FakeRepositories;

namespace CardGame.Tests.RulesTests
{
	public class MauMauRulesTest
	{
		private readonly MauMauRules _rules;

		public MauMauRulesTest()
		{
			_rules = new MauMauRules();
		}

		[Fact]
		public void CheckMoveLegal_Test()
		{
			var cardD5 = new Card() { Color = "D", Rank = "5", };	// set up test cards
			var cardDK = new Card() { Color = "D", Rank = "K", };
			var cardC5 = new Card() { Color = "C", Rank = "5", };
			var cardCK = new Card() { Color = "C", Rank = "K", };

			// Act
			var D5_DK = _rules.CheckMoveLegal(cardD5, cardDK);	// is legal
			var D5_C5 = _rules.CheckMoveLegal(cardD5, cardC5);	// is legal
			var D5_CK = _rules.CheckMoveLegal(cardD5, cardCK);	// is not legal
			var C5_DK = _rules.CheckMoveLegal(cardC5, cardDK);	// is not legal
			var C5_CK = _rules.CheckMoveLegal(cardC5, cardCK);	// is legal
			var CK_DK = _rules.CheckMoveLegal(cardCK, cardDK);	// is legal

			// Assert
			Assert.True(D5_DK);     // should be true: D on D
			Assert.True(D5_C5);     // should be true: 5 on 5
			Assert.False(D5_CK);    // should be false, neither Color nor Rank match
			Assert.False(C5_DK);    // should be false, neither Color nor Rank match
			Assert.True(C5_CK);     // should be true: C on C
			Assert.True(CK_DK);		// should be true: K on K
		}

	}
}
