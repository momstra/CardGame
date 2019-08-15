using Microsoft.EntityFrameworkCore;

namespace CardGame.Entities
{
	public class CardsContext : DbContext
	{
		public CardsContext(DbContextOptions<CardsContext> options)
			: base(options) { }

		public DbSet<Card> Cards { get; set; }
		public DbSet<Deck> Decks { get; set; }
		public DbSet<Hand> Hands { get; set; }
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
			builder.Entity<Player>()
				.HasKey(k => k.UserId);
			builder.Entity<Player>()
				.HasOne(p => p.Game)
				.WithMany(g => g.Players)
				.HasForeignKey(p => p.GameId)
				.OnDelete(DeleteBehavior.SetNull);
			/*builder.Entity<Game>()
				.HasMany(g => g.Players)
				.WithOne(p => p.Game);*/
		}
	}

}
