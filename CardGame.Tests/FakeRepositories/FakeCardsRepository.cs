using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGame.Repositories.Interfaces;
using CardGame.Entities;
using Moq;

namespace CardGame.Tests.FakeRepositories
{
	public class FakeCardsRepository : ICardsRepository
	{
		public List<Card> Deck { get; set; }
		public List<Hand> Hands { get; }
		public List<Player> Players { get; set; }
		public Queue<Card> CardsRemaining { get; set; }
		public Stack<Card> CardsPlayed { get; set; }
		public List<Game> Games { get; }

		public FakeCardsRepository()
		{
			Deck = new List<Card>();
			CardsRemaining = new Queue<Card>();
			Hands = new List<Hand>();
			Players = new List<Player>();
			CardsPlayed = new Stack<Card>();
			Games = new List<Game>();

			for (int i = 1; i < 53; i++)
			{
				Card card = CreateCard(i);
				Deck.Add(card);
			}
		}

		private Card CreateCard(int id)
		{
			var mock = new Mock<Card>().SetupAllProperties();
			Card mockCard = mock.Object;
			mockCard.CardId = id;
			return mockCard;
		}



		public void SaveChanges() {}

		public Game GetGame(int gameId) => Games.Find(g => g.GameId == gameId);

		public Queue<Card> GetCardsRemaining(int gameId) => CardsRemaining;

	}
}
