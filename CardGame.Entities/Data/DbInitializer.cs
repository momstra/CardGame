using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Entities.Data
{
	public static class DbInitializer
	{
		public static void Initialize(CardsContext context)
		{
			context.Database.EnsureCreated();

			if (context.Deck.Any())
				return;

			string[] colors = { "diamond", "heart", "spades", "clubs" };
			string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

			List<Card> deck = new List<Card>();
			foreach (string color in colors)
			{
				foreach (string rank in ranks)
				{
					Card card = new Card();
					card.Color = color;
					card.Rank = rank;
					deck.Add(card);
				}
			}
			foreach (Card card in deck)
			{
				context.Deck.Add(card);
				//context.RemainingCards.Add(card);
			}
			context.SaveChanges();
		}
	}
}