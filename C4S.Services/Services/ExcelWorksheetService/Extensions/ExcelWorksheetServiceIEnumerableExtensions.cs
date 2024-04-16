namespace C4S.Services.Services.ExcelWorksheetService.Extensions
{
    public static class ExcelWorksheetServiceIEnumerableExtensions
    {
        /// <summary>
        /// Get instance of type <typeparamref name="T"/> from <paramref name="services"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns>
        /// <typeparamref name="T"/>
        /// </returns>
        /// <exception cref="InvalidCastException"></exception>
        public static T Resolve<T>(this IEnumerable<IExcelWorksheetService> services)
            where T : IExcelWorksheetService
        {
            var service = services
               .Single(x => x.GetType().Name == typeof(T).Name);

            var instance = (T)service;

            return instance is null
                ? throw new InvalidCastException()
                : instance;
        }
    }
}
