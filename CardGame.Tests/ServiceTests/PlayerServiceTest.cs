﻿using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using CardGame.Entities;
using CardGame.Services;
using CardGame.Tests.FakeRepositories;
using System.Collections.Generic;

namespace CardGame.Tests
{
	public class PlayerServiceTest
	{
		private readonly FakeCardsRepository _repository;
		private readonly PlayerService _service;

		public PlayerServiceTest()
		{
			_repository = new FakeCardsRepository();
			_service = new PlayerService(_repository, new Mock<ILogger<PlayerService>>().Object);
		}

		[Fact]
		public void AddCardToHandTest()
		{
			var id = "TestPlayerCardAdd";
			int gameId = 22;
			int cardId = 1111;
			
			var player = _repository.CreatePlayer(id);	// create test player 
			
			Deck deck = new Deck();						// create deck for test game
			Card card = new Card()						// create a card and add it
			{
				CardId = cardId,
			};
			deck.Cards.Add(card);					
			_repository.AddGame(gameId, deck);			// add game to database

			// Act
			_service.AddCardToHand(cardId, id);

			// Assert
			Assert.Contains(card, player.Hand);	// card should be in player's hand
		}

		[Fact]
		public void CreatePlayerTest()
		{
			int count = _repository.GetPlayers().Count;	// get player count before action
			string id = "CreatePlayer";

			// Act
			Player player = _service.CreatePlayer(id);

			// Assert	
			Assert.True(_repository.GetPlayers().Count == count + 1);					// player count should have increased
			Assert.Equal(_repository.GetPlayers().Find(p => p.UserId == id), player);   // player should match saved player
			Assert.Null(_service.CreatePlayer(id));										// player should not be created as id already exists
		}

		[Fact]
		public void GetHandTest()
		{
			var playerId = "TestPlayerGetHand";
			
			var player = _repository.CreatePlayer(playerId);    // create test player 
			var card = new Card()								// create test card
			{
				Color = "T",
				Rank = "1",
			};

			string cardId = card.Color + card.Rank;				// get test card's value

			List<string> control = new List<string>				// and add to a list as control for what should be returned
			{
				cardId,
			};

			// Act
			var handBefore = _service.GetHand(playerId);		// get hand before adding card

			player.Hand.Add(card);

			var handAfter = _service.GetHand(playerId);			// and after adding it

			// Assert
			Assert.Equal(handBefore.Count, handAfter.Count - 1);// hand count should have increased by one

			Assert.DoesNotContain(cardId, handBefore);			// card should not have been in hand before adding it
			Assert.NotEqual(control, handBefore);
			Assert.Empty(handBefore);							// instead, hand should have been empty

			Assert.Contains(cardId, handAfter);					// card should be in hand after adding it
			Assert.Equal(control, handAfter);
		}

		[Fact]
		public void PlayCardTest()
		{
			string playerId = "PlayCardTestPlayer";
			var player = _repository.CreatePlayer(playerId);

			/*			int cardId = 1;
						Card card = new Card()
						{
							CardId = cardId,
							Color = "T",
							Rank = "2",
						};
			*/

			Deck deck = _repository.CreateDeck();

			_repository.CreateCards(deck);
			var cards = deck.Cards as List<Card>;
			var card = cards[0];
			var cardId = card.CardId;

			player.Hand.Add(card);
			_repository.SaveChanges();

			// Act
			var playedCard = _service.PlayCard(playerId, cardId);

			// Assert
			Assert.Equal(card, playedCard);
			Assert.DoesNotContain(card, player.Hand);
			Assert.Null(card.UserId);
			Assert.Null(card.Owner);
		}
	}
}
