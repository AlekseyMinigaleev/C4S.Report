namespace С4S.API.Models
{
    /// <summary>
    /// Параметры для сортировки
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Имя поля по которому будет применять сортировка
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// тип сортировки
        /// </summary>
        public SortType SortType { get; set; }

        /// <summary>
        /// Возвращает строку для применения сортировки
        /// </summary>
        /// <returns></returns>
        public string GetSortExpression()
        {
            var sortExpression = $"{FieldName} {(SortType == SortType.Ascending ? "ascending" : "descending")}";
            return sortExpression;
        }
    }

    /// <summary>
    /// тип сортировки
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// По возрастанию
        /// </summary>
        Ascending,

        /// <summary>
        /// По убыванию
        /// </summary>
        Descending
    }
}