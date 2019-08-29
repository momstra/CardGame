using CardGame.Entities;
using CardGame.Services.Rules.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;


namespace CardGame.Services.Rules
{
	public class MauMauRules : IGameRules
	{
		public bool CheckMoveLegal(Card lastCard, Card newCard)
		{
			if (lastCard.Color == newCard.Color)	
				return true;

			if (lastCard.Rank == newCard.Rank)
				return true;

			return false;
		}

		public bool MoveOnEmptyTable()
		{
			return false;
		}
	}
}
