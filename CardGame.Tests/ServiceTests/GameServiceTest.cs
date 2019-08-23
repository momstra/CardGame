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
			var id = _gameService.CreateGame();     // TODO

			// Act
			var before = _gameService.CheckGameStatus(id);
			_gameService.GetGame(id).GameStarted = true;
			var after = _gameService.CheckGameStatus(id);
			
			// Assert
			Assert.False(before);		// game should not have been started yet
			Assert.True(after);			// now it should have
		}

		[Fact]
		public void CheckGameExistsTest()
		{
			var id = _gameService.CreateGame();     // TODO

			// Act
			var existing = _gameService.CheckGameExists(id);
			var nonexisting = _gameService.CheckGameExists(123);

			// Assert
			Assert.True(existing);		// game with id should exist
			Assert.False(nonexisting);	// game with impossible id should not
		}

		[Fact]
		public void CreateGameTest()
		{
			// Act
			var id = _gameService.CreateGame();  
			var id2 = _gameService.CreateGame();  

			// Assert
			Assert.IsType<int>(id);						// CreateGame should return an integer (GameId)
			Assert.InRange(id, 1000, 9999);				// in this range

			Assert.NotNull(_gameService.GetGame(id));	// should still exist
			Assert.NotNull(_gameService.GetGame(id2));  // should exist as well

			Assert.NotEmpty(_gameService.GetGame(id).Deck.Cards); // Card deck should have been created
		}
		
		[Fact]
		public void DrawCardTest()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _gameService.GetGame(gameId);     // TODO
			_gameService.Shuffle(gameId);		// TODO: should be replaced with non-method option

			// Act
			int beforeDraw = game.CardsRemaining.Count;     // number of cards before drawing

			Card card1 = _gameService.DrawCard(gameId);
			int after1stDraw = game.CardsRemaining.Count;	// number of cards after drawing first card

			Card card2 = _gameService.DrawCard(gameId);
			int after2ndDraw = game.CardsRemaining.Count;	// number of cards after drawing another

			// Assert
			Assert.IsAssignableFrom<Card>(_gameService.DrawCard(gameId));	// drawing should return object of type Card
			Assert.NotNull(_gameService.DrawCard(gameId));
			Assert.IsType<Card>(card1);
			Assert.IsType<Card>(card2);
			Assert.NotEqual(card1, card2);						// drawn cards should be different ones
			Assert.True(after1stDraw == beforeDraw - 1);		// number of remaining cards should have decreased after 1st draw
			Assert.True(after2ndDraw == after1stDraw - 1);		// number should have decreased further
			Assert.DoesNotContain(card1, game.CardsRemaining);
			Assert.DoesNotContain(card2, game.CardsRemaining);	// after drawing cards should not be in list anymore
		}

		[Fact]
		public void GetGamesTest()
		{
			// Act
			int gamesCount = _gameService.GetGames().Count;		// there may already be games in list
			int game1Id = _gameService.CreateGame();     // TODO

			int gamesList1Count = _gameService.GetGames().Count;
			int game2Id = _gameService.CreateGame();
			var gamesList2Count = _gameService.GetGames().Count;

			// Assert
			Assert.NotEqual(gamesCount, gamesList1Count);	
			Assert.Equal(gamesCount + 1, gamesList1Count);		// there should be one more game in list at this point
			
			Assert.Equal(gamesCount + 2, gamesList2Count);		// and after second creation two more than original count
		}

		[Fact]
		public void GetPlayers_Game_Test()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _gameService.GetGame(gameId);     // TODO
			string player1Id = "GetGamePlayer1";
			string player2Id = "GetGamePlayer2";
			var player1 = new Player(player1Id);
			var player2 = new Player(player2Id);

			// Act
			var beforeJoin = _gameService.GetPlayers(gameId);

			game.Players.Add(player1);
			var after1stJoin = _gameService.GetPlayers(gameId);

			game.Players.Add(player2);
			var after2ndJoin = _gameService.GetPlayers(gameId);

			game.Players.Remove(player1);
			var afterRemove = _gameService.GetPlayers(gameId);

			// Assert
			Assert.Contains(player1, after1stJoin);                     // returned list should hold player1
			Assert.DoesNotContain(player2, after1stJoin);				// but not player2
			Assert.True(beforeJoin.Count == after1stJoin.Count - 1);    // and count should have increased by one

			Assert.Contains(player1, after2ndJoin);                     // returned list should hold player1
			Assert.Contains(player2, after2ndJoin);                     // and player2
			Assert.DoesNotContain(new Player(), after2ndJoin);			// but not any player
			Assert.True(after1stJoin.Count == after2ndJoin.Count - 1);  // and count should have increased by one

			Assert.Contains(player2, afterRemove);						// returned list should still hold player2
			Assert.DoesNotContain(player1, afterRemove);				// but not player1 anymore
			Assert.True(afterRemove.Count == after1stJoin.Count);		// and number should again be as after 1st join
		}

		[Fact]
		public void GetPlayersIds_Game_Test()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _gameService.GetGame(gameId);     // TODO

			string player1Id = "GetIdsGamePlayer1";
			string player2Id = "GetIdsGamePlayer2";
			var player1 = new Player(player1Id);
			var player2 = new Player(player2Id);

			List<string> playerIds = new List<string>
			{
				player1Id,
				player2Id,
			};

			game.Players.Clear();
			game.Players.Add(player1);
			game.Players.Add(player2);

			// Act
			var playerIds2 = _gameService.GetPlayersIds(gameId);

			game.Players.Remove(player1);
			var playerIds3 = _gameService.GetPlayersIds(gameId);

			// Assert
			Assert.IsType<List<string>>(playerIds2);	// should have returned a List<string>
			Assert.NotEmpty(playerIds2);                // that should not be empty
			Assert.Equal(playerIds, playerIds2);        // but equal to control list

			Assert.NotEqual(playerIds, playerIds3);     // after removing player1 returned list should not be equal to control list anymore
		}

		[Fact]
		public void JoinGameTest()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);

			string player1Id = "JoinPlayer1";
			string player2Id = "JoinPlayer2";
			var player1 = new Player(player1Id);
			var player2 = new Player(player2Id);
			_repository.AddPlayer(player1);
			_repository.AddPlayer(player2);

			int before = 0;
			if (game.Players != null)
				before = game.Players.Count;

			// Act
			var after1stJoin = _gameService.JoinGame(player1Id, gameId);	// returns game, which player has been joined to
			var after1stCount = game.Players.Count;

			game.MaxPlayers = 1;											// MaxPlayer count that has already been reached
			var after2ndJoin = _gameService.JoinGame(player2Id, gameId);

			game.MaxPlayers = 2;											// making sure MaxPlayers count has not been reached,
			game.GameStarted = true;										// but game has already started
			var after3rdJoin = _gameService.JoinGame(player2Id, gameId);

			game.GameStarted = false;										// change game to not started
			var after4thJoin = _gameService.JoinGame(player2Id, gameId);

			// Assert
			Assert.Equal(game.GameId, after1stJoin);		// first join should be successful and return correct gameId
			Assert.True(after1stCount == before + 1);		// Players count should have increased after successful join
			Assert.Equal(player1.GameId, game.GameId);
			Assert.Contains(player1, game.Players);			// player1 should be assigned after successful join
			
			Assert.Equal(0, after2ndJoin);					// player2 should not have been able to join as maxplayers has been reached
			
			Assert.Equal(0, after3rdJoin);					// player2 should not have been able to join as game already started
												
			Assert.NotEqual(0, after4thJoin);				// player2 should have been able to join after reset to not yet started
		}

		[Fact]
		public void LeaveGameTest()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);

			string player1Id = "LeavePlayer1";
			var player1 = new Player(player1Id);
			_repository.AddPlayer(player1);

			game.Players.Add(player1);
			player1.Game = game;
			player1.GameId = gameId;
			
			var list1 = _gameService.GetPlayersIds(gameId);     // TODO: should be replaced with non-method option

			// Act
			var leaving = _gameService.LeaveGame(player1Id);

			// Assert
			Assert.Contains(player1Id, list1);				// making sure player was added to game
			Assert.True(leaving);							// leave returns true if successfull
			Assert.DoesNotContain(player1, game.Players);	// should not contain player1 any longer
		}

		[Fact]
		public void ServeStartingHandsTest()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);
			_gameService.Shuffle(gameId);       // TODO: should be replaced with non-method option
			game.MinPlayers = 2;
			string player1Id = "ServePlayer1";
			string player2Id = "ServePlayer2";
			var player1 = new Player(player1Id);
			var player2 = new Player(player2Id);
			game.Players.Add(player1);
			game.Players.Add(player2);
			int handSize = 10;
			game.StartingHand = handSize;

			// Act
			var start = _gameService.StartGame(gameId);
			var serve = _gameService.ServeStartingHands(gameId);
			int player1Count = player1.Hand.Count;
			int player2Count = player2.Hand.Count;


			// Assert
			Assert.True(serve);								// serve method returns true when successfull
			Assert.Equal(handSize, player1Count);			// both hands should hold defined number of cards
			Assert.Equal(handSize, player2Count); 
			Assert.NotEqual(player1.Hand, player2.Hand);    // hands should not be equal

			foreach (Card card in player1.Hand)				// no two players can hold the same card
				Assert.DoesNotContain(card, player2.Hand);
		}

		
		[Fact]
		public void ShuffleTest()
		{
			int gameId = _gameService.CreateGame();     // TODO
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);

			int turn = 0;								// set up variables for loop
			bool areDifferent = false;

			// Act
			_gameService.Shuffle(gameId);               // shuffle deck

			Card card1 = game.CardsRemaining[0];		// safe topmost card after shuffling
			Card card2 = game.CardsRemaining[1];		

			do											// try multiple times to eliminate chance
			{
				_gameService.Shuffle(gameId);			// reshuffle deck

				if (card1 == game.CardsRemaining[0])	// the 1st cards from both sets should differ (in 98 % of cases)
				{
					if (card2 == game.CardsRemaining[1])// if the 1st cards match, the 2nd ones should differ (in 99.9 % of cases)
					{
						turn++;
						continue;						// if, against all odds, they to match, reshuffle and test again
					}
				}	

				areDifferent = true;
				break;				
			} while (turn < 10);						// ten turns should remove any slight chance for false results

			// Assert
			Assert.True(areDifferent);					// if it turns out false, the method is most definitely fraud
		}

		[Fact]
		public void StartGameTest()
		{
			var gameId = _gameService.CreateGame();     // TODO
			Game game = _repository.GetGames().Find(g => g.GameId == gameId);

			var cards = game.CardsRemaining;
			var beforeCount = cards.Count;
			game.MinPlayers = 0;	

			// Act
			_gameService.StartGame(gameId);

					
			Assert.IsType<List<Card>>(cards);
			Assert.Equal(0, beforeCount);

			Assert.NotEmpty(cards);						// starting game should fill CardsRemaining deck
			Assert.True(game.GameStarted);				// and set GameStarted
		}
	}
}
