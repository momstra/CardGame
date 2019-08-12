using System;
using System.Collections.Generic;
using System.Linq;

using CardGame.Entities;
using CardGame.Repositories.Interfaces;

namespace CardGame.Business.Interfaces
{
	public interface IGameBusiness
	{
		int CreateNewGame();
		void StartGame(int gameId);
		Card DrawCard(int gameId);
		void Shuffle(int gameId);
		void Shuffle(List<Card> cards, int gameId);

	}
}
