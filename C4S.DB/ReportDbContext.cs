using C4S.DB.Models;
using C4S.DB.Models.Hangfire;
using Microsoft.EntityFrameworkCore;

namespace C4S.DB
{
    public class ReportDbContext : DbContext
    {
        public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
        { }

        /// <summary>
        /// Список C4S пользователей
        /// </summary>
        public DbSet<UserModel> Users { get; set; }

        /// <summary>
        /// Список игр
        /// </summary>
        public DbSet<GameModel> Games { get; set; }

        /// <summary>
        /// Список игровых статистик
        /// </summary>
        public DbSet<GameStatisticModel> GamesStatistics { get; set; }

        /// <summary>
        /// Список джоб.
        /// </summary>
        public DbSet<HangfireJobConfigurationModel> HangfireConfigurations { get; set; }

        /// <summary>
        /// Список статусов игр
        /// </summary>
        public DbSet<CategoryModel> Categories { get; set; }

        /// <summary>
        /// Список связей <see cref="CategoryModel"/> - <see cref="GameModel"/>
        /// </summary>
        public DbSet<CategoryGameModel> CategoryGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
        }
    }
}