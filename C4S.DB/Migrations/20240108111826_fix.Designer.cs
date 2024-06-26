﻿// <auto-generated />
using System;
using C4S.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace C4S.DB.Migrations
{
    [DbContext(typeof(ReportDbContext))]
    [Migration("20240108111826_fix")]
    partial class fix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("C4S.DB.Models.GameGameStatusModel", b =>
                {
                    b.Property<Guid>("GameStatisticId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameStatisticId", "GameStatusId");

                    b.HasIndex("GameStatusId");

                    b.ToTable("GameGameStatus", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AppId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PageId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublicationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Game", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GameStatisticModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("CashIncome")
                        .HasColumnType("float");

                    b.Property<double>("Evaluation")
                        .HasColumnType("float");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastSynchroDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PlayersCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameStatistic", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GameStatusModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("GameStatus", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.Hangfire.HangfireJobConfigurationModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CronExpression")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnable")
                        .HasColumnType("bit");

                    b.Property<int>("JobType")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("HangfireJobConfiguration", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.UserModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DeveloperPageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RsyaAuthorizationToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GameGameStatusModel", b =>
                {
                    b.HasOne("C4S.DB.Models.GameStatisticModel", "GameStatistic")
                        .WithMany("GameGameStatus")
                        .HasForeignKey("GameStatisticId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("C4S.DB.Models.GameStatusModel", "GameStatus")
                        .WithMany()
                        .HasForeignKey("GameStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameStatistic");

                    b.Navigation("GameStatus");
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.HasOne("C4S.DB.Models.UserModel", "User")
                        .WithMany("Games")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("C4S.DB.Models.GameStatisticModel", b =>
                {
                    b.HasOne("C4S.DB.Models.GameModel", "Game")
                        .WithMany("GameStatistics")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("C4S.DB.Models.Hangfire.HangfireJobConfigurationModel", b =>
                {
                    b.HasOne("C4S.DB.Models.UserModel", "User")
                        .WithMany("HangfireJobConfigurationModels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Navigation("GameStatistics");
                });

            modelBuilder.Entity("C4S.DB.Models.GameStatisticModel", b =>
                {
                    b.Navigation("GameGameStatus");
                });

            modelBuilder.Entity("C4S.DB.Models.UserModel", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("HangfireJobConfigurationModels");
                });
#pragma warning restore 612, 618
        }
    }
}
