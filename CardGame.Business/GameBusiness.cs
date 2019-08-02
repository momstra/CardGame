using System;
using System.Collections.Generic;
using System.Linq;

using CardGame.Entities;
using CardGame.Repositories.Interfaces;

namespace CardGame.Business
{
	public class GameBusiness
	{
		private readonly ICardsRepository _repository;

		public GameBusiness(ICardsRepository repository)
		{
			_repository = repository;
		}

		public int CreateNewGame()
		{
			Random random = new Random();
			int id = random.Next(1000, 9999);
			while (_repository.GetGame(id) != null)
				id = random.Next(1000, 9999);

			if (_repository.AddGame(id) != null)
				return id;

			return 0;
		}

		public void StartGame(int gameId)
		{
			Shuffle(gameId);
			_repository.GetGame(gameId).GameStarted = true;
		}


		public Card DrawCard(int gameId)
		{
			if (_repository.GetCardsRemaining(gameId).Count > 0)
				return _repository.GetCardsRemaining(gameId).Dequeue();

			return null;
		}

		public void Shuffle(int gameId)
		{
			_repository.GetCardsRemaining(gameId).Clear();
			List<Card> cards = _repository.Deck.ToList();
			Shuffle(cards, gameId);
		}

		public void Shuffle(List<Card> cards, int gameId)
		{
			if (cards.Count < 1) return;
			Random rand = new Random();
			Card randomCard;
			while (cards.Count() > 0)
			{
				var position = rand.Next(0, cards.Count());
				randomCard = cards[position];
				_repository.GetCardsRemaining(gameId).Enqueue(randomCard);
				cards.RemoveAt(position);
			}
		}


	}
}
