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
			builder.Entity<Card>()
				.Property(p => p.CardId)
				.ValueGeneratedOnAdd();

			builder.Entity<Card>()
				.HasOne(c => c.Owner)
				.WithMany(p => p.Hand)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Card>()
				.HasOne(c => c.CardsPlayed)
				.WithMany(p => p.CardsPlayed)
				.HasForeignKey(c => c.PlayedId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Card>()
				.HasOne(c => c.CardsRemaining)
				.WithMany(r => r.CardsRemaining)
				.HasForeignKey(c => c.RemainingId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Player>()
				.HasKey(k => k.PlayerId);

			builder.Entity<Player>()
				.HasOne(p => p.Game)
				.WithMany(g => g.Players)
				.HasForeignKey(p => p.GameId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Player>()
				.HasOne(p => p.PlayersReady)
				.WithMany(g => g.PlayersReady)
				.HasForeignKey(p => p.ReadyId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Set>()
				.Property(p => p.SetId)
				.ValueGeneratedOnAdd();
		}
	}

}
