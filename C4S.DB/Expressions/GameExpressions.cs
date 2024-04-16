using C4S.DB.Extensions;
using C4S.DB.Models;
using C4S.DB.ValueObjects;
using System.Linq.Expressions;

namespace C4S.DB.Expressions
{
    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="GameModel"/>
    /// </summary>
    public static class GameExpressions
    {
        /// <summary>
        /// Выражение для получения актуальной на данный момент оценки игры.
        /// </summary>
        public static Expression<Func<GameModel, double>> ActualEvaluationExpression => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic()!.Evaluation;

        /// <summary>
        /// </summary>
        public static Expression<Func<GameModel, ValueWithProgress<int>?>> ActualRatingExpression => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic().Rating;

        /// <summary>
        /// </summary>
        public static Expression<Func<GameModel, ValueWithProgress<double>?>> ActualCashIncomeExpression => (GameModel game) =>
            game.GameStatistics.GetLastSynchronizationStatistic()!.CashIncome;
    }
}