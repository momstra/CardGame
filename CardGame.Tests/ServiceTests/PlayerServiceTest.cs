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
		private readonly ILogger<PlayerService> _logger;

		public PlayerServiceTest()
		{
			var serviceProvider = new ServiceCollection()
				.AddLogging()
				.BuildServiceProvider();
			var factory = serviceProvider.GetService<ILoggerFactory>();
			_logger = factory.CreateLogger<PlayerService>();

			_repository = new FakeCardsRepository();
			_service = new PlayerService(_repository, _logger);
		}

		[Fact]
		public void AddCardToHandTest()
		{
			var id = "TestPlayerCardAdd";
			int gameId = 2;
			int cardId = 1;
			Hand hand = _repository.CreateHand();
			var player = _service.CreatePlayer(id);        // get player object
			player.Hand = hand;

			Deck deck = new Deck();
			Card card = new Card()
			{
				CardId = cardId,
			};
			deck.Cards.Add(card);
			_repository.AddGame(gameId, deck);

			_service.AddCardToHand(cardId, hand.HandId);

			Assert.Contains(card, player.Hand.Cards);
		}

		[Fact]
		public void CreatePlayerTest()
		{
			int count = _repository.GetPlayers().Count;
			Player player = _service.CreatePlayer("CreatePlayer");

			Assert.NotEqual(_repository.GetPlayers().Count, count);
			Assert.True(_repository.GetPlayers().Count == count + 1);   // Players count should have increased
			Assert.Equal(_repository.GetPlayers().Find(p => p.UserId == "CreatePlayer"), player);   // player should match saved player
			Assert.Null(_service.CreatePlayer("CreatePlayer")); // player should not be created as id already exists
		}
	}
}
