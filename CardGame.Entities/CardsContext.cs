using Microsoft.EntityFrameworkCore;
using System;

namespace CardGame.Entities
{
	public class CardsContext : DbContext
	{
		public CardsContext(DbContextOptions<CardsContext> options)
			: base(options) { }

		public DbSet<Card> Cards { get; set; }
		public DbSet<Deck> Decks { get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<Player> Players { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Card>()
				.Property(p => p.CardId)
				.ValueGeneratedOnAdd();

			builder.Entity<Deck>()
				.Property(p => p.DeckId)
				.ValueGeneratedOnAdd();

			builder.Entity<Game>()
				.Property(e => e.PlayersReady)
				.HasConversion(
					v => string.Join(',', v),
					v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

			builder.Entity<Player>()
				.HasKey(k => k.UserId);

			builder.Entity<Player>()				// setting delete behaviour for Player >-| Game relationship
				.HasOne(p => p.Game)
				.WithMany(g => g.Players)
				.HasForeignKey(p => p.GameId)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}

}
