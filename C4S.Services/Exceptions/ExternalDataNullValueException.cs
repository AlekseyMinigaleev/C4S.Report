namespace C4S.Services.Exceptions
{
    /// <summary>
    /// Базовое исключение для ситуаций, когда из внешнего источника данных получено значение null.
    /// </summary>
    public class ExternalDataNullValueException : Exception
    {
        /// <summary>
        /// Ключ или идентификатор поля, содержащего null-значение.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Источник данных, из которого получено null-значение.
        /// </summary>
        public object? Source { get; }

        /// <summary>
        /// Дополнительное сообщение об ошибке.
        /// </summary>
        public string? Message { get; }

        public ExternalDataNullValueException(
            string key,
            object? source = null,
            string? message = null)
            : base(message ?? $"Получено значение null по ключу {key} ")
        {
            Key = key;
            Source = source;
            Message = message;
        }
    }

    /// <summary>
    /// Исключение для случаев, когда при парсинге HTML получено значение null.
    /// </summary>
    public class HtmlParsingNullValueException : ExternalDataNullValueException
    {
        public HtmlParsingNullValueException(string key, object? source)
            : base(key, source)
        { }
    }

    /// <summary>
    /// Исключение для случаев, когда из JSON-объекта получено значение null.
    /// </summary>
    public class JsonPropertyNullValueException : ExternalDataNullValueException
    {
        public JsonPropertyNullValueException(string key, object? source = null, string? message = null)
            : base(key, source, message)
        { }
    }
}