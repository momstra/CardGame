using System;
using System.Collections.Generic;
using CardGame.Entities;


namespace CardGame.Tests.FakeRepositories
{
	public class FakeServicesRepository
	{
		private readonly List<Game> _games;
		private readonly List<Player> _players;
		private readonly List<Card> _cards;
		private readonly List<Hand> _hands;

		public FakeServicesRepository()
		{
			_games = new List<Game>();
			_players = new List<Player>();
			_cards = new List<Card>();
		}

		public List<Game> Games { get => _games; }

		public List<Player> Players { get => _players; }

		public List<Card> Cards { get => _cards; }

		public List<Hand> Hands { get => _hands; }
	}
}
