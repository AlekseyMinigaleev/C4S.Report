namespace C4S.Db.Exceptions
{
    /// <summary>
    /// Ошибка, обозначающая некорректное cron выражение.
    /// </summary>
    public class InvalidCronExpressionException : Exception
    {
        /// <summary>
        /// Cron выражение, которое вызвало ошибку.
        /// </summary>
        public readonly string? CronExpression;

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public const string ErrorMessage = $"Указанное выражение cron не является валидным";

        /// <param name="cronExpression"> Cron выражение, которое вызвало ошибку.</param>
        public InvalidCronExpressionException(string? cronExpression) : base(ErrorMessage)
        {
            CronExpression = cronExpression;
        }
    }
}