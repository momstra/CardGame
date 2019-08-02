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
	}
}
