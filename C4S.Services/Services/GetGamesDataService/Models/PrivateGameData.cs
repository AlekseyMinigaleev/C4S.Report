namespace C4S.Services.Services.GetGamesDataService.Models
{
    /// <summary>
    /// Класс, представляющий конфиденциальные данные игры.
    /// </summary>
    public class PrivateGameData
    {
        /// <summary>
        /// Получает или устанавливает значение денежного дохода без учета НДС.
        /// </summary>
        public double? CashIncome { get; set; }
    }

}
