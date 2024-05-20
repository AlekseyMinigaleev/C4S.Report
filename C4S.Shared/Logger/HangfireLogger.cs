using Hangfire.Console;
using Hangfire.Server;

namespace C4S.Shared.Logger
{
    /// <summary>
    /// Логгер в консоль Hangfire
    /// </summary>
    public class HangfireLogger : BaseLogger
    {
        public HangfireLogger(string? prefix = default)
            : base(prefix) { }

        private readonly PerformContext _hangfireContext;

        public HangfireLogger(PerformContext hangfireContext)
        {
            _hangfireContext = hangfireContext;
        }

        ///<inheritdoc/>
        public override void Log(string message, LogLevel logLevel)
        {
            var color = logLevel switch
            {
                LogLevel.Success => ConsoleTextColor.Green,
                LogLevel.Warning => ConsoleTextColor.Yellow,
                LogLevel.Error => ConsoleTextColor.Red,
                LogLevel.Information => ConsoleTextColor.White,
                _ => ConsoleTextColor.White,
            };

            if (Prefix != default)
                message = $"{Prefix} {message}";

            _hangfireContext.SetTextColor(color);
            _hangfireContext.WriteLine(message);
            _hangfireContext.ResetTextColor();
        }

        ///<inheritdoc/>
        public override void LogError(string message) => Log(message, LogLevel.Error);

        ///<inheritdoc/>
        public override void LogInformation(string message) => Log(message, LogLevel.Information);

        ///<inheritdoc/>
        public override void LogSuccess(string message) => Log(message, LogLevel.Success);

        ///<inheritdoc/>
        public override void LogWarning(string message) => Log(message, LogLevel.Warning);
    }
}