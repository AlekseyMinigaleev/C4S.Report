namespace C4S.Shared.Logger
{
    /// <summary>
    /// Базовый логгер в формате C4S
    /// </summary>
    public abstract class BaseLogger
    {
        public string? Prefix { get; set; }

        public BaseLogger(string? prefix = default)
        {
            Prefix = prefix;
        }

        /// <summary>
        /// Выполняет логирование на уровне <see cref="LogLevel.Information"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public abstract void LogInformation(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="LogLevel.Success"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public abstract void LogSuccess(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="LogLevel.Warning"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public abstract void LogWarning(string message);

        /// <summary>
        /// Выполняет логирование на уровне <see cref="LogLevel.Error"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        public abstract void LogError(string message);

        /// <summary>
        /// Выполняет логирование на уровне <paramref name="logLevel"/>
        /// </summary>
        /// <param name="message">Сообщение лога.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public abstract void Log(string message, LogLevel logLevel);
    }
}