using Microsoft.Extensions.Logging;
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
			
			var player = _repository.CreatePlayer(playerId);      // create test player 
			var card = new Card();

			var beforeCount = player.Hand.Count;
			var beforeContains = player.Hand.Contains(card);

			// Act
			player.Hand.Add(card);
			var hand = _service.GetHand(playerId);

			var afterCount = hand.Count;
			var afterContains = hand.Contains(card);

			// Assert
			Assert.Equal(beforeCount, afterCount - 1);
			Assert.False(beforeContains);
			Assert.True(afterContains);
			Assert.Equal(hand, player.Hand);
		}
	}
}
