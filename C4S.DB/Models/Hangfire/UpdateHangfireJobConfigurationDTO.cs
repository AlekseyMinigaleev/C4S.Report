namespace C4S.DB.Models.Hangfire
{
    public class UpdateHangfireJobConfigurationDTO
    {
        public Guid Id { get; set; }

        public string CronExpression { get; set; }

        public bool IsEnable { get; set; }
    }
}
