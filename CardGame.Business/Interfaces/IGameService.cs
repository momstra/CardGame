using System;
using System.Collections.Generic;

using CardGame.Entities;

namespace CardGame.Services.Interfaces
{
	public interface IGameService
	{
		int CreateGame(string playerId);			// create game for player
		Card DrawCard(int gameId);					// draw card from CardsRemaining deck

		Game GetGame(int gameId);					// get game by GameId
		Game GetGame(string userId);				// ... by PlayerId of a joined player
		List<Game> GetGames();						// get list of all games
		List<int> GetGameIdsList();					// get list of Ids of all games

		List<Player> GetPlayers(int gameId);		// get list of players assigned to game
		List<string> GetPlayersIds(int gameId);		// get list of Ids of players assigned to game

		string GetTurnPlayer(int gameId);			// get player whose turn it is
		int JoinGame(string playerId, int gameId);	// assign player to game

		bool LeaveGame(string playerId);			// remove player from game by id
		bool LeaveGame(Player player);				// remove player from game by object
		bool LeaveRunningGame(string PlayerId);		// ... from running game by id
		bool LeaveRunningGame(Player player);		// ... from running game by object

		Player MoveToNextPlayer(int gameId);		// move turn to next player
		object PlayCard(int gameId, Card card);		// play card
		bool RemoveGame(int gameId);				// remove game
		bool ServeStartingHands(int gameId);		// serve initial hands at beginning
		Byte SetPlayerReady(string playerId);		// set player ready to start

		void ReShuffleCardsRemaining(Game game);	// reshuffle the existing CardsRemaining
		void Shuffle(int gameId);					// shuffle set into CardsRemaining
		void Shuffle(List<Card> cards, int gameId);	// shuffle cards into game by id
		void Shuffle(List<Card> cards, Game game);	// shuffle cards into game by object

		bool StartGame(int gameId);					// start game
	}
}
