using System.ComponentModel.DataAnnotations;

namespace C4S.Services.Services.GetGamesDataService.Enums
{
    /// <summary>
    /// Словарь всех допустимых форматов для запроса на Яндекс игры
    /// </summary>
    public enum RequestFormat
    {
        /// <summary>
        /// Полный формат
        /// </summary>
        [Display(Name = "long")]
        Long,

        /// <summary>
        /// Короткий формат
        /// </summary>
        [Display(Name = "short")]
        Short
    }
}