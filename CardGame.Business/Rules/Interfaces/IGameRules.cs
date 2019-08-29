using CardGame.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardGame.Services.Rules.Interfaces
{
	public interface IGameRules
	{
		bool CheckMoveLegal(Card lastCard, Card newCard);
		bool MoveOnEmptyTable();
	}
}
