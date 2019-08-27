using Microsoft.EntityFrameworkCore;
using System;

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

			builder.Entity<Game>()
				.Property(e => e.PlayersReady)
				.HasConversion(
					v => string.Join(',', v),
					v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));

			builder.Entity<Player>()
				.HasKey(k => k.UserId);

			builder.Entity<Player>()
				.HasOne(p => p.Game)
				.WithMany(g => g.Players)
				.HasForeignKey(p => p.GameId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Set>()
				.Property(p => p.SetId)
				.ValueGeneratedOnAdd();

			/*builder.Entity<Set>()
				.HasOne(s => s.Game)
				.WithOne(g => g.Set)
				.HasForeignKey<Game>(s => s.GameId)
				.OnDelete(DeleteBehavior.Cascade);*/
		}
	}

}
