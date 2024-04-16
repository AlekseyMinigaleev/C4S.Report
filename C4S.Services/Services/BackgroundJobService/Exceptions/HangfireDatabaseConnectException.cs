namespace C4S.Services.Services.BackgroundJobService.Exceptions
{
    public class HangfireDatabaseConnectException : Exception
    {
        public string? ConnectionString { get; set; }

        public static string ErrorMessage = "Не удалось подключиться к базе данных hangfire.";

        public HangfireDatabaseConnectException(string? connectionString)
            : base($"{ErrorMessage}. {connectionString}")
        {
            ConnectionString = connectionString;
        }

        public HangfireDatabaseConnectException()
            : base($"{ErrorMessage}")
        { }
    }
}
