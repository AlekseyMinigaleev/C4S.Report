using C4S.DB.ValueObjects;
using System.Linq.Expressions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица игровой статистики
    /// </summary>
    public class GameStatisticModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Оценка игры
        /// </summary>
        public double Evaluation { get; private set; }

        /// <summary>
        /// Рейтинг игры
        /// </summary>
        public ValueWithProgress<int>? Rating { get; set; }

        /// <summary>
        /// Доход игры
        /// </summary>
        public ValueWithProgress<double>? CashIncome { get; set; }

        /// <summary>
        /// Дата последней синхронизации с Яндексом
        /// </summary>
        public DateTime LastSynchroDate { get; private set; }

        /// <summary>
        /// Связь с <see cref="GameModel"/>
        /// </summary>
        public GameModel Game { get; private set; }

        /// <summary>
        /// FK <see cref="GameModel"/>
        /// </summary>
        public Guid GameId { get; set; }

        public GameStatisticModel(
            GameModel game,
            DateTime lastSynchroDate,
            double evaluation)
        {
            Id = Guid.NewGuid();
            GameId = game.Id;
            Game = game;
            LastSynchroDate = lastSynchroDate;
            Evaluation = evaluation;
        }

        private GameStatisticModel()
        { }
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameStatisticModel"/>
    /// </summary>
    public static class GameStatisticExpression
    { }
}