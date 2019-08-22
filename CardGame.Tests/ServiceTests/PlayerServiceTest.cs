using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using CardGame.Tests.FakeRepositories;
using CardGame.Services;
using CardGame.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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

			// build test player
			var player = _service.CreatePlayer(id);		// create player 

			// build test game
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
			Assert.Contains(card, player.Cards);	// card should be in player's hand
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
	}
}
