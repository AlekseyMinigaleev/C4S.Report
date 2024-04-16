namespace C4S.Services.Services.ExcelWorksheetService.Models
{
    /// <summary>
    /// Excel ячейка
    /// </summary>
    public struct ExcelCell
    {
        /// <summary>
        /// Индекс строки
        /// </summary>
        public int Row
        {
            readonly get => _row;

            set => _row = value < MinIndex
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : value;
        }

        /// <summary>
        /// Индекс колонки
        /// </summary>
        public int Column
        {
            readonly get => _column;

            set => _column = _column < MinIndex
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : value;
        }

        /// <summary>
        /// Минимальный индекс
        /// </summary>
        public const int MinIndex = 0;

        private int _row;
        private int _column;

        /// <param name="row"><inheritdoc cref="Row"/></param>
        /// <param name="col"><inheritdoc cref="Column"/></param>
        public ExcelCell(int row, int col)
        {
            Row = row;
            Column = col;
        }
    }
}
