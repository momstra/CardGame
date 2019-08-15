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
			//builder.Entity<Hand>()
			//	.HasKey(k => k.HandId);
			builder.Entity<Player>()
				.HasKey(k => k.UserId);
		}
	}

}
