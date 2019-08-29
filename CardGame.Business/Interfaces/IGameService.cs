﻿using System;
using System.Collections.Generic;
using System.Linq;

using CardGame.Entities;
using CardGame.Repositories.Interfaces;

namespace CardGame.Services.Interfaces
{
	public interface IGameService
	{
		int CreateGame(string playerId);
		Card DrawCard(int gameId);
		Game GetGame(int gameId);
		Game GetGame(string userId);
		List<Game> GetGames();
		List<int> GetGameIdsList();
		List<Player> GetPlayers(int gameId);
		List<string> GetPlayersIds(int gameId);
		string GetTurnPlayer(int gameId);
		int JoinGame(string playerId, int gameId);
		bool LeaveGame(string playerId);
		bool LeaveRunningGame(string PlayerId);
		bool LeaveRunningGame(Player player);
		Player MoveToNextPlayer(int gameId);
		object PlayCard(int gameId, Card card);
		bool RemoveGame(int gameId);
		bool ServeStartingHands(int gameId);
		Byte SetPlayerReady(string playerId);
		void Shuffle(int gameId);
		void Shuffle(List<Card> cards, int gameId);
		bool StartGame(int gameId);
	}
}
