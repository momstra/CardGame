using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;
using CardGame.Entities;
using CardGame.Services;
using CardGame.Tests.FakeRepositories;

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

			_repository = new FakeCardsRepository("GameServiceTestDb");
			_gameService = new GameService(_repository, _logger);
		}

		#region Helper methods
		public Game CreateGame()
		{
			Set set = _repository.CreateSet();
			if (!_repository.CreateCards(set))
				return null;

			Random random = new Random();
			int gameId = random.Next(1000, 9999);
			
			while (_repository.GetGame(gameId) != null)
				gameId = random.Next(1000, 9999);

			if (!_repository.AddGame(gameId, set))
				return null;

			return _repository.GetGames().Find(g => g.GameId == gameId);
		}

		public Player CreatePlayer(string playerId)
		{
			Player player = _repository.CreatePlayer(playerId);

			return player;
		}

		public List<Card> Shuffle(Game game)
		{
			foreach (Card card in game.Set.Cards)
				game.CardsRemaining.Add(card);

			return game.CardsRemaining;
		}
		#endregion Helper methods


		#region Tests
		[Fact]
		public void CheckGameStatus_Test()
		{
			var game = CreateGame();		// create test game using helper method
			var gameId = game.GameId;

			// Act
			var before = _gameService.CheckGameStatus(gameId);

			game.GameStatus = 1;		// change game status to started

			var after = _gameService.CheckGameStatus(gameId);
			
			// Assert
			Assert.Equal(0, before);			// returned status before starting should be 0
			Assert.Equal(1, after);				// returned status after starting should be 1
		}

		[Fact]
		public void CheckGameExists_Test()
		{
			var game = CreateGame();		// create test game using helper method
			var gameId = game.GameId;

			// Act
			var existing = _gameService.CheckGameExists(gameId);
			var nonexisting = _gameService.CheckGameExists(123);

			// Assert
			Assert.True(existing);			// game with gameId should exist
			Assert.False(nonexisting);		// game with random id should not
		}

		[Fact]
		public void CreateGame_Test()
		{
			int beforeCount = _repository.GetGames().Count;		// games count beforehand
			string playerid1 = "CreatePlayer1";					// id needed to set active player
			string playerid2 = "CreatePlayer2";

			// Act
			var gameId1 = _gameService.CreateGame(playerid1);	// create game (playerid to simulate asking player)
			var game1 = _repository.GetGame(gameId1);			// retrieve game
			int after1stCount = _repository.GetGames().Count;	// save games count

			var gameId2 = _gameService.CreateGame(playerid2);	// same for 2nd game
			var game2 = _repository.GetGame(gameId2);
			int after2ndCount = _repository.GetGames().Count;

			// Assert
			Assert.IsType<int>(gameId1);						// CreateGame should return an created game's GameId,
			Assert.InRange(gameId1, 1000, 9999);				// which should be in this range
			Assert.Equal(beforeCount, after1stCount - 1);		// and be added to database

			Assert.IsType<int>(gameId2);						// same for the 2nd game created
			Assert.InRange(gameId2, 1000, 9999);
			Assert.Equal(beforeCount, after2ndCount - 2);	

			Assert.NotEqual(gameId1, gameId2);					// both game's ids should differ
			Assert.NotEqual(game1, game2);						// as should the games themselves

			Assert.NotNull(game1);								// and they should not be null
			Assert.NotNull(game2);					

			Assert.NotEmpty(game1.Set.Cards);					// and their sets should not be empty
			Assert.NotEmpty(game2.Set.Cards);

			Assert.Equal(playerid1, game1.ActivePlayer);		// and game creator set as active player
			Assert.Equal(playerid2, game2.ActivePlayer);
		}
		
		[Fact]
		public void DrawCard_Test()
		{
			var game = CreateGame();							// create game using helper method
			var gameId = game.GameId;
			Shuffle(game);										// add cards using helper method

			// Act
			int beforeDraw = game.CardsRemaining.Count;			// number of cards before drawing

			Card card1 = _gameService.DrawCard(gameId);
			int after1stDraw = game.CardsRemaining.Count;		// number of cards after drawing first card

			Card card2 = _gameService.DrawCard(gameId);
			int after2ndDraw = game.CardsRemaining.Count;		// number of cards after drawing another

			// Assert
			Assert.IsAssignableFrom<Card>(_gameService.DrawCard(gameId));	// drawing should return object of type Card

			Assert.IsType<Card>(card1);							// returned objects should be cards
			Assert.IsType<Card>(card2);

			Assert.NotNull(card1);								// and they should not be null
			Assert.NotNull(card1);

			Assert.NotEqual(card1, card2);                      // drawn cards should be different ones

			Assert.True(after1stDraw == beforeDraw - 1);		// number of remaining cards should have decreased after 1st draw
			Assert.True(after2ndDraw == after1stDraw - 1);      // number should have decreased further

			Assert.DoesNotContain(card1, game.CardsRemaining);	// after drawing cards should not be in list anymore
			Assert.DoesNotContain(card2, game.CardsRemaining);
		}

		[Fact]
		public void GetGames_Test()
		{
			int gamesCount = _gameService.GetGames().Count;     // get games count of before (there may already be games in list)

			int gameId1 = 1;
			while (_repository.GetGame(gameId1) != null)		// make sure 1st new game does not replace an existing one
				gameId1++;

			int gameId2 = gameId1 + 1;
			while (_repository.GetGame(gameId2) != null)        // make sure 2nd new game does not replace an existing one either
				gameId2++;

			// Act
			var game1 = CreateGame();                           // create 1st test game using helper method
			int game1AddedCount = _gameService.GetGames().Count;

			var game2 = CreateGame();                           // create 2nd test game using helper method
			int game2AddedCount = _gameService.GetGames().Count;

			// Assert
			Assert.Equal(gamesCount + 1, game1AddedCount);		// after adding 1st game there should be one more game than originally
			Assert.Equal(gamesCount + 2, game2AddedCount);      // and two more after adding the 2nd game
		}

		[Fact]
		public void GetPlayers_Game_Test()
		{
			var game = CreateGame();							// create test game using helper method
			var gameId = game.GameId;

			string player1Id = "GetGamePlayer1";
			var player1 = CreatePlayer(player1Id);				// create test player using helper method

			string player2Id = "GetGamePlayer2";
			var player2 = CreatePlayer(player2Id);				// create test player using helper method

			var beforeJoin = _gameService.GetPlayers(gameId);	// get game's assigned players before "joining"

			// Act
			game.Players.Add(player1);
			var after1stJoin = _gameService.GetPlayers(gameId);	// players after 1st assignment

			game.Players.Add(player2);
			var after2ndJoin = _gameService.GetPlayers(gameId); // players after 2nd assignment

			game.Players.Remove(player1);
			var afterRemove = _gameService.GetPlayers(gameId);  // players after removing player1

			// Assert
			Assert.Contains(player1, after1stJoin);             // returned list should hold player1
			Assert.DoesNotContain(player2, after1stJoin);		// but not player2
			Assert.True(beforeJoin.Count == after1stJoin.Count - 1);	// and count should have increased by one

			Assert.Contains(player1, after2ndJoin);             // returned list should hold player1
			Assert.Contains(player2, after2ndJoin);             // and player2
			Assert.DoesNotContain(new Player(), after2ndJoin);	// but not any player
			Assert.True(after1stJoin.Count == after2ndJoin.Count - 1);	// and count should have increased by one

			Assert.Contains(player2, afterRemove);				// returned list should still hold player2
			Assert.DoesNotContain(player1, afterRemove);		// but not player1 anymore
			Assert.True(afterRemove.Count == after1stJoin.Count);	// and number should again be as after 1st join
		}

		[Fact]
		public void GetPlayersIds_Game_Test()
		{
			var game = CreateGame();                            // create test game using helper method
			var gameId = game.GameId;

			string player1Id = "GetIdsGamePlayer1";             // creating test players using helper method
			var player1 = CreatePlayer(player1Id);
			string player2Id = "GetIdsGamePlayer2";
			var player2 = CreatePlayer(player2Id);

			List<string> playerIdsControl = new List<string>	// creating control list 
			{
				player1Id,
				player2Id,
			};

			game.Players.Clear();								// making sure no players are assigned already
			game.Players.Add(player1);							// and assign both players
			game.Players.Add(player2);

			// Act
			var playerIdsBefore = _gameService.GetPlayersIds(gameId);	// should return list holding both player's ids

			game.Players.Remove(player1);								// remove player1 from game
			var playerIdsAfter = _gameService.GetPlayersIds(gameId);	// should return new list holding just player2's id

			// Assert
			Assert.IsType<List<string>>(playerIdsBefore);		// returned object should be lists of strings,
			Assert.IsType<List<string>>(playerIdsAfter);

			Assert.NotEmpty(playerIdsBefore);					// which should not be empty
			Assert.NotEmpty(playerIdsAfter);
			Assert.NotEqual(playerIdsBefore, playerIdsAfter);	// nor exactly matching each other

			Assert.Equal(playerIdsControl, playerIdsBefore);	// before removing player1 returned list should be holding the same values as control

			Assert.NotEqual(playerIdsControl, playerIdsAfter);	// after removing player1 returned list should not be equal to control list anymore
		}

		[Fact]
		public void GetTurnPlayer_Test()
		{
			var game = CreateGame();			// creating test game using helper method
			var gameId = game.GameId;

			string player1Id = "TurnPlayer1";   // creating test players using helper method
			string player2Id = "TurnPlayer2";
			var player1 = CreatePlayer(player1Id);
			var player2 = CreatePlayer(player2Id);
			
			game.Players.Add(player1);			// assigning player1
			player1.Game = game;
			player1.GameId = gameId;

			game.Players.Add(player2);			// assigning player2
			player2.Game = game;
			player2.GameId = gameId;

			game.ActivePlayer = player1Id;		// setting player1 as active player

			// Act
			var firstTurn = _gameService.GetTurnPlayer(gameId);

			game.ActivePlayer = player2Id;		// change active player
			var secondTurn = _gameService.GetTurnPlayer(gameId);

			game.ActivePlayer = player1Id;		// change active player back
			var thirdTurn = _gameService.GetTurnPlayer(gameId);

			// Assert
			Assert.Equal(player1Id, firstTurn); // should return player1's id
			Assert.Equal(player2Id, secondTurn);// active player changed, should return player2's id
			Assert.Equal(player1Id, thirdTurn); // active player changed back, should return player1's id

			Assert.Null(_gameService.GetTurnPlayer(123456)); // non-existent game, should return null
		}

		[Fact]
		public void JoinGame_Test()
		{
			var game = CreateGame();            // creating test game using helper method
			var gameId = game.GameId;

			string player1Id = "JoinPlayer1";   // creating test players using helper method
			string player2Id = "JoinPlayer2";
			var player1 = CreatePlayer(player1Id);
			var player2 = CreatePlayer(player2Id);

			int before = 0;						// setting player count before actions
			if (game.Players != null)
				before = game.Players.Count;

			// Act
			var after1stJoin = _gameService.JoinGame(player1Id, gameId);	// returns game player has been joined to
			var after1stCount = game.Players.Count;

			game.MaxPlayers = 1;						// MaxPlayer count that has already been reached
			var after2ndJoin = _gameService.JoinGame(player2Id, gameId);

			game.MaxPlayers = 2;                        // making sure MaxPlayers count has not been reached,
			game.GameStatus = 1;						// but game has already started
			var after3rdJoin = _gameService.JoinGame(player2Id, gameId);

			game.GameStatus = 0;						// change game to not started
			var after4thJoin = _gameService.JoinGame(player2Id, gameId);

			// Assert
			Assert.Equal(game.GameId, after1stJoin);	// first join should be successful and return correct gameId
			Assert.True(after1stCount == before + 1);	// Players count should have increased after successful join
			Assert.Equal(player1.GameId, game.GameId);
			Assert.Equal(player1.Game, game);
			Assert.Contains(player1, game.Players);		// player1 should be assigned after successful join
			
			Assert.Equal(0, after2ndJoin);				// player2 should not have been able to join as maxplayers has been reached
			
			Assert.Equal(0, after3rdJoin);				// player2 should not have been able to join as game already started
												
			Assert.NotEqual(0, after4thJoin);			// player2 should have been able to join after reset to not yet started
		}

		[Fact]
		public void LeaveGame_Test()
		{
			var game = CreateGame();					// creating game with helper method
			var gameId = game.GameId;
			game.GameStatus = 0;						// making sure it is not started

			string player1Id = "Leave1Player1";
			var player1 = CreatePlayer(player1Id);      // creating test player using helper method

			game.Players.Add(player1);					// assigning test player 
			player1.Game = game;
			player1.GameId = gameId;

			List<Player> players = new List<Player>();	// create control
			foreach (Player player in game.Players)
				players.Add(player);

			// Act
			var leaving = _gameService.LeaveGame(player1Id);

			// Assert
			Assert.Contains(player1, players);				// making sure player was added to game
			Assert.True(leaving);							// leave returns true if successfull
			Assert.DoesNotContain(player1, game.Players);   // should not contain player1 any longer
		}

		[Fact]
		public void LeaveRunningGame_Test()
		{
			var game = CreateGame();					// creating test game using helper method
			var gameId = game.GameId;
			Shuffle(game);                              // fill cards using helper method

			string player1Id = "LeaveRunningPlayer1";   // creating test players using helper method
			string player2Id = "LeaveRunningPlayer2";
			string player3Id = "LeaveRunningPlayer3";
			var player1 = CreatePlayer(player1Id);
			var player2 = CreatePlayer(player2Id);
			var player3 = CreatePlayer(player3Id);

			game.Players.Add(player1);					// assigning players
			player1.Game = game;
			player1.GameId = gameId;
			game.Players.Add(player2);
			player2.Game = game;
			player2.GameId = gameId;
			game.Players.Add(player3);
			player3.Game = game;
			player3.GameId = gameId;

			List<Player> players = new List<Player>();	// create control
			foreach (Player player in game.Players)
				players.Add(player);

			int handSize = 5;

			foreach (Player player in players)			// assigning cards to players
			{
				for (var i = 0; i < handSize; i++)
				{
					var card = game.CardsRemaining[0];
					game.CardsRemaining.Remove(card);
					player.Hand.Add(card);
				}
			}

			var cardsRemainingCount1 = game.CardsRemaining.Count;	// getting count of cards still in deck

			game.GameStatus = 1;									// setting game started
			game.ActivePlayer = player2Id;							// making it player2's turn
			var activePlayer1 = game.ActivePlayer.ToString();

			// Act
			_gameService.LeaveRunningGame(player2);
			var cardsRemainingCount2 = game.CardsRemaining.Count;   // getting count of cards in deck after player2 leaving
			var activePlayer2 = game.ActivePlayer.ToString();		// getting active player after old active player leaving
		
			_gameService.LeaveRunningGame(player3);
			var activePlayer3 = game.ActivePlayer.ToString();       // getting active player after player2 & player3 left
			var gameStatus3 = game.GameStatus.ToString();           // getting game status after player2 & player3 left

			// Assert
			Assert.Equal(cardsRemainingCount2, cardsRemainingCount1 + handSize);    // leaving player's cards should be added back to remaining ones
			Assert.NotEqual(activePlayer1, activePlayer2);          // leaving player should not be active player anymore
			Assert.NotEqual("", activePlayer2);                     // active player should be set

			Assert.Equal("10", gameStatus3);                         // game should have ended as only one player is left
			Assert.Equal(player1Id, activePlayer3);					// active player should be last player assigned
			}

		[Fact]
		public void MoveToNextPlayer_Test()
		{
			var game = CreateGame();
			var gameId = game.GameId;

			string player1Id = "NextPlayer1";
			string player2Id = "NextPlayer2";
			var player1 = new Player(player1Id);
			var player2 = new Player(player2Id);
			_repository.AddPlayer(player1);
			_repository.AddPlayer(player2);

			game.Players.Add(player1);
			player1.Game = game;
			player1.GameId = gameId;

			game.Players.Add(player2);
			player2.Game = game;
			player2.GameId = gameId;

			game.ActivePlayer = player1Id;
			game.TurnCompleted = false;

			// Act
			var firstTry = _gameService.MoveToNextPlayer(gameId);		// should not work

			game.TurnCompleted = true;									
			var secondTry = _gameService.MoveToNextPlayer(gameId);		// should work as turn completed
			var activeAfterSecondTry = game.ActivePlayer.ToString();    // should have changed
			var turnAfterSecondTry = game.TurnCompleted.ToString();		// should be false

			var thirdTry = _gameService.MoveToNextPlayer(gameId);       // should not work

			game.TurnCompleted = true;
			var fourthTry = _gameService.MoveToNextPlayer(gameId);      // should work as turn completed again
			var activeAfterFourthTry = game.ActivePlayer.ToString();    // should have changed again
			var turnAfterFourthTry = game.TurnCompleted.ToString();     // should be false again

			// Assert
			Assert.Null(firstTry);										// MoveToNextTurn returns null if unsuccessful

			Assert.IsType<Player>(secondTry);							// .. or the player whose turn it is next
			Assert.Equal(player2Id, activeAfterSecondTry);              // should have moved from player1 to player2
			Assert.Equal("False", turnAfterSecondTry);                  // TurnCompleted should have been set back to false

			Assert.Null(thirdTry);

			Assert.IsType<Player>(fourthTry);
			Assert.Equal(player1Id, activeAfterFourthTry);              // should have moved back to player1
			Assert.Equal("False", turnAfterFourthTry);					// TurnCompleted should have been set back
		}

		[Fact]
		public void PlayCard_Test()
		{
			var game = CreateGame();					// set up test game
			string playerId = "TestPlayerPlay";
			var player = CreatePlayer(playerId);		// set up test player

			Card card = game.Set.Cards[0];				// get a card
			player.Hand.Add(card);
			_repository.SaveChanges();					// make player owner of card

			var playedBefore = game.CardsPlayed.Find(c => c.CardId == card.CardId);
			var ownerBefore = card.Player.ToString();

			// Act 
			var cardPlayed = _gameService.PlayCard(game.GameId, card);
			var playedAfter = game.CardsPlayed.Find(c => c.CardId == card.CardId);

			// Assert
			Assert.Contains(card.CardId.ToString(), cardPlayed.ToString());
			Assert.Null(playedBefore);                  // shouldn't have been in played cards at the beginning
			
			Assert.Equal(card, playedAfter);			// should have been last card played after being played
			Assert.Contains(card, game.CardsPlayed);	// and therefore contained in CardsPlayed

			Assert.NotNull(ownerBefore);				// card should have been owned before
			Assert.Null(card.Player);					// and not owned by anyone afterwards
		}

		[Fact]
		public void RemoveGame_Test()
		{
			var game = CreateGame();
			var gameId = game.GameId;

			string player1Id = "RemovePlayer1";              // create and add player to game
			var player1 = CreatePlayer(player1Id);
			game.Players.Add(player1);
			var playersCount = _gameService.GetPlayers(gameId).Count;

			// Act
			var firstTry = _gameService.RemoveGame(gameId); // should be false, players not empty

			game.Players.Clear();
			var secondTry = _gameService.RemoveGame(gameId);// should work

			// Assert
			Assert.Equal(1, playersCount);					// one player should have been assigned 
			Assert.False(firstTry);							// therefore could not be removed
			Assert.True(secondTry);							// Players empty, therefore should work
			Assert.Null(_gameService.GetGame(gameId));		// should not exist any more
		}


		[Fact]
		public void ServeStartingHands_Test()
		{
			var game = CreateGame();
			var gameId = game.GameId;

			string player1Id = "ServePlayer1";				// create and add players to game
			var player1 = CreatePlayer(player1Id);
			game.Players.Add(player1);

			string player2Id = "ServePlayer2";
			var player2 = CreatePlayer(player2Id);
			game.Players.Add(player2);

			int handSize = 10;								// setting hand size to test with
			game.StartingHand = handSize;

			game.GameStatus = 1;
			Shuffle(game);

			// Act
			var serve = _gameService.ServeStartingHands(gameId);
			
			// Assert
			Assert.True(serve);								// serve method returns true when successfull
			Assert.Equal(handSize, player1.Hand.Count);		// both hands should hold defined number of cards
			Assert.Equal(handSize, player2.Hand.Count); 
			Assert.NotEqual(player1.Hand, player2.Hand);    // hands should not be equal

			foreach (Card card in player1.Hand)				// no two players can hold the same card
				Assert.DoesNotContain(card, player2.Hand);
		}

		[Fact]
		public void SetPlayerReady_Test()
		{
			string player1Id = "ReadyPlayer1";			// create and add players to game
			var player1 = CreatePlayer(player1Id);

			string player2Id = "ReadyPlayer2";
			var player2 = CreatePlayer(player2Id);

			string player3Id = "ReadyPlayer3";
			var player3 = CreatePlayer(player3Id);

			string player4Id = "ReadyPlayer4";
			var player4 = CreatePlayer(player4Id);
						
			var game = CreateGame();
			var gameId = game.GameId;
			game.MinPlayers = 2;						// set up game for testing
			game.MaxPlayers = 4;

			// Act
			var ready0 = _gameService.SetPlayerReady("idonotexist");    // non joined player 

			_gameService.JoinGame(player1Id, gameId);			
			var ready1 = _gameService.SetPlayerReady(player1Id);		// 1 player, 1 ready

			_gameService.JoinGame(player2Id, gameId);
			_gameService.JoinGame(player3Id, gameId);
			var ready2 = _gameService.SetPlayerReady(player2Id);		// 3 players, 2 ready

			var ready3 = _gameService.SetPlayerReady(player3Id);        // 3 players, 3 ready
			var ready3a = _gameService.SetPlayerReady(player3Id);		// try again

			_gameService.JoinGame(player4Id, gameId);
			var ready4 = _gameService.SetPlayerReady(player4Id);		// 4 players, 4 ready
			
			// Assert 
			Assert.Equal(0, ready0);					// 0, there should be an error

			Assert.Equal(1, ready1);					// 1, still waiting for others to join
			Assert.Contains(player1, game.PlayersReady);

			Assert.Equal(2, ready2);                    // 2, enough players joined but still waiting for others to get ready 
			Assert.Contains(player2, game.PlayersReady);

			Assert.Equal(3, ready3);                    // 3, enough players joined and all ready, but max count not yet reached
			Assert.Contains(player3, game.PlayersReady);
			Assert.Equal(0, ready3a);					// should not have worked, player already ready

			Assert.Equal(4, ready4);                    // 4, max number of players joined and all ready
			Assert.Contains(player4, game.PlayersReady);
		}

		[Fact]
		public void Shuffle_Test()
		{
			var game = CreateGame();
			var gameId = game.GameId;

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
		public void StartGame_Test()
		{
			var game = CreateGame();							// set up test game
			var gameId = game.GameId;
			game.MinPlayers = 1;
			game.MaxPlayers = 1;
			
			string player1Id = "StartPlayer1";					// create test players
			var player1 = CreatePlayer(player1Id);

			string player2Id = "StartPlayer2";
			var player2 = CreatePlayer(player2Id);

			List<Player> players = new List<Player>();

			var cards = game.CardsRemaining;
			var beforeCount = cards.Count;

			// Act
			var firstStart = _gameService.StartGame(gameId);    // start game without players assigned
			var firstStatus = game.GameStatus.ToString();		
			
			game.Players.Add(player1);
			var secondStart = _gameService.StartGame(gameId);   // start game with player1 assigned
			var secondStatus = game.GameStatus.ToString();  

			game.GameStatus = 0;                           // reset status
			game.Players.Add(player2);
			var thirdStart = _gameService.StartGame(gameId);    // start game with both players assigned
			var thirdStatus = game.GameStatus.ToString();      

			// Assert	
			Assert.Equal(0, beforeCount);						// CardsRemaining should have been empty before start
			Assert.NotEmpty(cards);                             // starting game should fill CardsRemaining deck

			Assert.False(firstStart);                           // should be flase, too few players to start
			Assert.Equal("0", firstStatus);

			Assert.True(secondStart);                           // should be true, only game with one player should start
			Assert.Equal("1", secondStatus);

			Assert.False(thirdStart);                           // should be false, too many players
			Assert.Equal("0", thirdStatus);
		}

		#endregion Tests
	}
}
