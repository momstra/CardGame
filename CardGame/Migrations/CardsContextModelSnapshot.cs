﻿// <auto-generated />
using System;
using CardGame.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CardGame.API.Migrations
{
    [DbContext(typeof(CardsContext))]
    partial class CardsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CardGame.Entities.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Color");

                    b.Property<int?>("PlayedId");

                    b.Property<string>("PlayerId");

                    b.Property<string>("Rank");

                    b.Property<int?>("RemainingId");

                    b.Property<int>("SetId");

                    b.HasKey("CardId");

                    b.HasIndex("PlayedId");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RemainingId");

                    b.HasIndex("SetId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("CardGame.Entities.Game", b =>
                {
                    b.Property<int>("GameId");

                    b.Property<string>("ActivePlayer");

                    b.Property<byte>("GameStatus");

                    b.Property<int>("MaxPlayers");

                    b.Property<int>("MinPlayers");

                    b.Property<int>("SetId");

                    b.Property<int>("StartingHand");

                    b.Property<bool>("TurnCompleted");

                    b.HasKey("GameId");

                    b.HasIndex("SetId")
                        .IsUnique();

                    b.ToTable("Games");
                });

            modelBuilder.Entity("CardGame.Entities.Player", b =>
                {
                    b.Property<string>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("GameId");

                    b.Property<string>("HubId");

                    b.Property<int?>("PlayersReadyId");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayersReadyId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("CardGame.Entities.Set", b =>
                {
                    b.Property<int>("SetId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Size");

                    b.HasKey("SetId");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("CardGame.Entities.Card", b =>
                {
                    b.HasOne("CardGame.Entities.Game", "CardsPlayed")
                        .WithMany("CardsPlayed")
                        .HasForeignKey("PlayedId");

                    b.HasOne("CardGame.Entities.Player", "Player")
                        .WithMany("Hand")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CardGame.Entities.Game", "CardsRemaining")
                        .WithMany("CardsRemaining")
                        .HasForeignKey("RemainingId");

                    b.HasOne("CardGame.Entities.Set", "Set")
                        .WithMany("Cards")
                        .HasForeignKey("SetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CardGame.Entities.Game", b =>
                {
                    b.HasOne("CardGame.Entities.Set", "Set")
                        .WithOne("Game")
                        .HasForeignKey("CardGame.Entities.Game", "SetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CardGame.Entities.Player", b =>
                {
                    b.HasOne("CardGame.Entities.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CardGame.Entities.Game", "PlayersReady")
                        .WithMany("PlayersReady")
                        .HasForeignKey("PlayersReadyId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
