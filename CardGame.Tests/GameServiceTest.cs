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
	public class GameServiceTest
	{
		private readonly FakeCardsRepository _repository;
		private readonly GameService _gameService;
		private readonly ILogger<GameService> _logger;

		public GameServiceTest()
		{
			var serviceProvider = new ServiceCollection()
				.AddLogging()
				.BuildServiceProvider();
			var factory = serviceProvider.GetService<ILoggerFactory>();
			_logger = factory.CreateLogger<GameService>();

			_repository = new FakeCardsRepository();
			_gameService = new GameService(_repository, _logger);
		}

		[Fact]
		public void CheckGameStatusTest()
		{
			var id = _gameService.CreateGame();
			Assert.False(_gameService.CheckGameStatus(id)); // game should not have been started yet
			_gameService.GetGame(id).GameStarted = true;
			Assert.True(_gameService.CheckGameStatus(id));	// now it should have
		}

		[Fact]
		public void CheckGameExistsTest()
		{
			var id = _gameService.CreateGame();
			Assert.True(_gameService.CheckGameExists(id));		// game with id should exist
			Assert.False(_gameService.CheckGameExists(123));	// game with impossible id should not
		}

		[Fact]
		public void CreateGameTest()
		{
			var id = _gameService.CreateGame();
			Assert.IsType<int>(id);			// CreateGame should return an integer (GameId)
			Assert.InRange(id, 1000, 9999);	// in this range
			var id2 = _gameService.CreateGame();
			Assert.NotNull(_gameService.GetGame(id));	// should still exist
			Assert.NotNull(_gameService.GetGame(id2));  // should exist as well

			Assert.NotEmpty(_gameService.GetGame(id).Deck.Cards); // Card deck should have been created
		}
		
		[Fact]
		public void DrawCardTest()
		{
			int gameId = _gameService.CreateGame();
			Game game = _gameService.GetGame(gameId);
			_gameService.Shuffle(gameId);
			int beforeDraw = game.CardsRemaining.Count;     // number of cards before drawing
			Card card1 = _gameService.DrawCard(gameId);
			int after1stDraw = game.CardsRemaining.Count;	// number of cards after drawing first card
			Card card2 = _gameService.DrawCard(gameId);
			int after2ndDraw = game.CardsRemaining.Count;	// number of cards after drawing another

			Assert.IsAssignableFrom<Card>(_gameService.DrawCard(gameId));	// drawing should return object of type Card
			Assert.NotNull(_gameService.DrawCard(gameId));
			Assert.IsType<Card>(card1);
			Assert.IsType<Card>(card2);
			Assert.NotEqual(card1, card2);					// drawn cards should be different ones
			Assert.True(after1stDraw == beforeDraw - 1);	// number of remaining cards should have decreased after 1st draw
			Assert.True(after2ndDraw == after1stDraw - 1);  // number should have decreased further
			Assert.DoesNotContain(card1, game.CardsRemaining);
			Assert.DoesNotContain(card2, game.CardsRemaining);	// after drawing cards should not be in list anymore
		}

		[Fact]
		public void GetGamesTest()
		{
			int gamesCount = _gameService.GetGames().Count; // there may already be games in list

			int game1Id = _gameService.CreateGame();
			int gamesList1Count = _gameService.GetGames().Count;
			Assert.NotEqual(gamesCount, gamesList1Count);	
			Assert.Equal(gamesCount + 1, gamesList1Count); // there should be one more game in list

			int game2Id = _gameService.CreateGame();
			var gamesList2Count = _gameService.GetGames().Count;
			Assert.NotEqual(gamesList1Count, gamesList2Count);	// gamesList2 should hold on more game
			Assert.Equal(gamesCount + 2, gamesList2Count);			// or two more than original count
		}

		[Fact]
		public void GetPlayers_Game_Test()
		{
			int gameId = _gameService.CreateGame();
			Game game = _gameService.GetGame(gameId);
			var mockPlayer1 = new Mock<Player>();
			var mockPlayer2 = new Mock<Player>();
			var player1 = mockPlayer1.Object;
			var player2 = mockPlayer2.Object;
			player1.UserId = "GetGamePlayer1";
			player2.UserId = "GetGamePlayer2";
			int beforeJoin = _gameService.GetPlayers(gameId).Count;
			game.Players.Add(player1);
			int after1stJoin = _gameService.GetPlayers(gameId).Count;
			Assert.Contains(player1, _gameService.GetPlayers(gameId));	// returned list should hold player1
			Assert.True(beforeJoin == after1stJoin - 1);			// and have increased by one

			game.Players.Add(player2);
			int after2ndJoin = _gameService.GetPlayers(gameId).Count;
			Assert.Contains(player2, _gameService.GetPlayers(gameId));	// returned list should hold player2
			Assert.True(after1stJoin == after2ndJoin - 1);          // and have increased by one

			game.Players.Remove(player1);
			Assert.Contains(player2, _gameService.GetPlayers(gameId));			// returned list should still hold player2
			Assert.DoesNotContain(player1, _gameService.GetPlayers(gameId));    // but not player1 anymore
			Assert.True(_gameService.GetPlayers(gameId).Count == after1stJoin);	// and number should be the same as after 1st join
		}

		[Fact]
		public void GetPlayersIds_Game_Test()
		{
			int gameId = _gameService.CreateGame();
			Game game = _gameService.GetGame(gameId);
			var mockPlayer1 = new Mock<Player>();
			var mockPlayer2 = new Mock<Player>();
			var player1 = mockPlayer1.Object;
			var player2 = mockPlayer2.Object;
			player1.UserId = "GetIdsGamePlayer1";
			player2.UserId = "GetIdsGamePlayer2";
			List<string> playerIds = new List<string>
			{
				"GetIdsGamePlayer1",
				"GetIdsGamePlayer2"
			};
			game.Players.Clear();
			game.Players.Add(player1);
			game.Players.Add(player2);
			var playerIds2 = _gameService.GetPlayersIds(gameId);

			Assert.IsType<List<string>>(playerIds2);	// should have returned a List<string>
			Assert.NotEmpty(playerIds2);                // that should not be empty
			Assert.Equal(playerIds, playerIds2);        // but equal to control list

			game.Players.Remove(player1);
			var playerIds3 = _gameService.GetPlayersIds(gameId);
			Assert.NotEqual(playerIds, playerIds3);        // should not be equal to control list anymore
			playerIds.Remove("GetIdsGamePlayer1");
			Assert.Equal(playerIds, playerIds3);		// should be equal to control list again
		}

		[Fact]
		public void JoinGameTest()
		{
			int gameId = _gameService.CreateGame();
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);
			string player1Id = "JoinPlayer1";
			string player2Id = "JoinPlayer2";
			//var mockPlayer1 = new Mock<Player>();
			//var mockPlayer2 = new Mock<Player>();
			var player1 = new Player(); // mockPlayer1.Object;
			var player2 = new Player(); // mockPlayer2.Object;
			player1.UserId = player1Id;
			player2.UserId = player2Id;
			_repository.AddPlayer(player1);
			_repository.AddPlayer(player2);

			int before = 0;
			if (game.Players != null)
				before = game.Players.Count;
			var game2id = _gameService.JoinGame(player1Id, gameId);	// returns game, which player has been joined to
			int after = game.Players.Count;                     // 
																// 
			Assert.Equal(game, _gameService.GetGame(game2id));		// therefore games should match
			Assert.True(after == before + 1);       // Players count should have increased after joining
			Assert.Equal(player1.GameId, game.GameId);
			Assert.Contains(player1, game.Players); // player1 should be assigned after joining

			game.MaxPlayers = 1;									// make sure MaxPlayers count has been reached
			Assert.Equal(0, _gameService.JoinGame("JoinPlayer2", gameId));	// player2 should not be able to join

			game.MaxPlayers = 2;									// make sure MaxPlayers count has not been reached,
			game.GameStarted = true;								// but game has already started
			Assert.Equal(0, _gameService.JoinGame("JoinPlayer2", gameId));	// player2 should not be able to join

			game.GameStarted = false;									// reset to game has not yet started
			Assert.NotEqual(0, _gameService.JoinGame("JoinPlayer2", gameId));	// player2 should be able to join
		}

		[Fact]
		public void LeaveGameTest()
		{
			int gameId = _gameService.CreateGame();
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);
			string player1Id = "LeavePlayer1";
			//var mockPlayer1 = new Mock<Player>();
			var player1 = new Player(); // mockPlayer1.Object;
			player1.UserId = player1Id;
			_repository.AddPlayer(player1);

			var game2id = _gameService.JoinGame(player1Id, gameId);
			Assert.Equal(game, _gameService.GetGame(game2id));	// make sure player1 is assigned to game
			Assert.Contains(player1, game.Players);

			Assert.True(_gameService.LeaveGame(player1Id, gameId));	// leave returns true if successfull
			Assert.DoesNotContain(player1, game.Players);		// should not contain player1 any longer
		}

		[Fact]
		public void ServeCardsTest()
		{
			int gameId = _gameService.CreateGame();
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);
			game.MinPlayers = 2;
			//var mockPlayer1 = new Mock<Player>();
			//var mockPlayer2 = new Mock<Player>();
			var player1 = new Player();// mockPlayer1.Object;
			var player2 = new Player();// mockPlayer2.Object;
			player1.Hand = new Hand();
			player2.Hand = new Hand();
			game.Players.Add(player1);
			game.Players.Add(player2);
			game.StartingHand = 4;
			_gameService.StartGame(gameId);

			Assert.True(_gameService.ServeCards(gameId));   // serve method returns true when successfull
			Assert.Equal(4, player1.Hand.Cards.Count);
			Assert.Equal(player2.Hand.Cards.Count, player1.Hand.Cards.Count); //both hands should hold 4 cards
			Assert.NotEqual(player1.Hand.Cards, player2.Hand.Cards);
		}

		
		[Fact]
		public void ShuffleTest()
		{
			int gameId = _gameService.CreateGame();
			Game game = _gameService.GetGame(gameId);
			_gameService.Shuffle(gameId);
			Card card1 = game.CardsRemaining[0];    // safe topmost card after shuffling
			game.CardsRemaining.RemoveAt(0);
			int turn = 0;
			bool areDifferent = false;
			do
			{
				_gameService.Shuffle(gameId);
				Card card2 = game.CardsRemaining[0];    // safe topmost card after another shuffle
				game.CardsRemaining.RemoveAt(0);
				if (card1 != card2)						// and make sure both cards differ
				{
					areDifferent = true;
					break;
				}
				turn++;
			} while (turn < 100);	// repeat to prevent "false" false results

			Assert.True(areDifferent);
		}

		[Fact]
		public void StartGameTest()
		{
			var id = _gameService.CreateGame();
			var cards = _gameService.GetGame(id).CardsRemaining;	
			
			Assert.NotEqual(0, id);				// id would be 0 if game creation fails
			Assert.IsType<List<Card>>(cards);	// deck of CardsRemaining should exist,
			Assert.Empty(cards);                // but be empty

			_gameService.GetGame(id).MinPlayers = 0;
			_gameService.StartGame(id);
			Assert.NotEmpty(cards);								// starting game should fill CardsRemaining deck
			Assert.True(_repository.GetGame(id).GameStarted);	// and set GameStarted
		}
	}
}
