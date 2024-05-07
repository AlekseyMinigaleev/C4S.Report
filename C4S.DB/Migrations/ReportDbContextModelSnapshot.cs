﻿// <auto-generated />
using System;
using C4S.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace C4S.DB.Migrations
{
    [DbContext(typeof(ReportDbContext))]
    partial class ReportDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("C4S.DB.Models.CategoryGameModel", b =>
                {
                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("GameId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("CategoryGame", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.CategoryModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AppId")
                        .HasColumnType("int");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PageId")
                        .HasColumnType("bigint");

                    b.Property<string>("PreviewURL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PublicationDate")
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

                    b.Property<double>("Evaluation")
                        .HasColumnType("float");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("LastSynchroDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameStatistic", (string)null);
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

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RsyaAuthorizationToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.UserSettingsModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsConfidentialMod")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSettings", (string)null);
                });

            modelBuilder.Entity("C4S.DB.Models.CategoryGameModel", b =>
                {
                    b.HasOne("C4S.DB.Models.CategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("C4S.DB.Models.GameModel", "Game")
                        .WithMany("CategoryGameModels")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Game");
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

                    b.OwnsOne("C4S.DB.ValueObjects.ValueWithProgress<double>", "CashIncome", b1 =>
                        {
                            b1.Property<Guid>("GameStatisticModelId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("ActualValue")
                                .HasColumnType("float")
                                .HasColumnName("CashIncomeActual");

                            b1.Property<double>("ProgressValue")
                                .HasColumnType("float")
                                .HasColumnName("CashIncomeProgress");

                            b1.HasKey("GameStatisticModelId");

                            b1.ToTable("GameStatistic");

                            b1.WithOwner()
                                .HasForeignKey("GameStatisticModelId");
                        });

                    b.OwnsOne("C4S.DB.ValueObjects.ValueWithProgress<int>", "Rating", b1 =>
                        {
                            b1.Property<Guid>("GameStatisticModelId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("ActualValue")
                                .HasColumnType("int")
                                .HasColumnName("RatingActual");

                            b1.Property<int>("ProgressValue")
                                .HasColumnType("int")
                                .HasColumnName("RatingProgress");

                            b1.HasKey("GameStatisticModelId");

                            b1.ToTable("GameStatistic");

                            b1.WithOwner()
                                .HasForeignKey("GameStatisticModelId");
                        });

                    b.Navigation("CashIncome");

                    b.Navigation("Game");

                    b.Navigation("Rating");
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

            modelBuilder.Entity("C4S.DB.Models.UserSettingsModel", b =>
                {
                    b.HasOne("C4S.DB.Models.UserModel", "User")
                        .WithOne("Settings")
                        .HasForeignKey("C4S.DB.Models.UserSettingsModel", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("C4S.DB.Models.GameModel", b =>
                {
                    b.Navigation("CategoryGameModels");

                    b.Navigation("GameStatistics");
                });

            modelBuilder.Entity("C4S.DB.Models.UserModel", b =>
                {
                    b.Navigation("Games");

                    b.Navigation("HangfireJobConfigurationModels");

                    b.Navigation("Settings")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
