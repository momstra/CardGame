using Microsoft.EntityFrameworkCore;

namespace CardGame.Entities
{
	public class CardsContext : DbContext
	{
		public CardsContext(DbContextOptions<CardsContext> options)
			: base(options) { }

		public DbSet<Card> Cards { get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Set> Sets { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			// Primary Keys
			builder.Entity<Card>()
				.Property(p => p.CardId)
				.ValueGeneratedOnAdd();

			builder.Entity<Game>()
				.Property(p => p.GameId)
				.ValueGeneratedNever();

			builder.Entity<Player>()
				.HasKey(k => k.PlayerId);

			builder.Entity<Set>()
				.Property(p => p.SetId)
				.ValueGeneratedOnAdd();

			// Foreign Keys
			builder.Entity<Card>()
				.HasOne(c => c.Player)
				.WithMany(p => p.Hand)
				.HasForeignKey(c => c.PlayerId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Card>()
				.HasOne(c => c.CardsPlayed)
				.WithMany(p => p.CardsPlayed)
				.HasForeignKey(c => c.PlayedId);

			builder.Entity<Card>()
				.HasOne(c => c.CardsRemaining)
				.WithMany(r => r.CardsRemaining)
				.HasForeignKey(c => c.RemainingId);

			builder.Entity<Card>()
				.HasOne(c => c.Set)
				.WithMany(s => s.Cards)
				.HasForeignKey(c => c.SetId);

			builder.Entity<Player>()
				.HasOne(p => p.Game)
				.WithMany(g => g.Players)
				.HasForeignKey(p => p.GameId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Player>()
				.HasOne(p => p.PlayersReady)
				.WithMany(g => g.PlayersReady)
				.HasForeignKey(p => p.PlayersReadyId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}

}
