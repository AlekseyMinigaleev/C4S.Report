using System.ComponentModel.DataAnnotations;

namespace C4S.DB.Models.Hangfire
{
    /// <summary>
    /// Перечисление всех возможных типов джоб
    /// </summary>
    public enum HangfireJobType
    {
        [Display(Name = "Получение всех данных по играм")]
        SyncGameJob,
    }
}