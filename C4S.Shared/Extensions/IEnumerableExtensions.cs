namespace C4S.Shared.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Получает элементы, которые отсутствуют во второй коллекции.
        /// </summary>
        /// <typeparam name="T">Тип элементов в коллекциях.</typeparam>
        /// <param name="firstCollection">Первая коллекция элементов.</param>
        /// <param name="secondCollection">Вторая коллекция элементов.</param>
        /// <returns>Новая коллекция элементов, которые не содержатся во второй коллекции.</returns>
        public static IEnumerable<T> GetItemsNotInSecondCollection<T>(this IEnumerable<T> firstCollection,
            IEnumerable<T> secondCollection)
        {
            var itemsNotInSecondCollection = firstCollection
                .Where(firstItem => !secondCollection
                    .Any(secondItem => secondItem.Equals(firstItem)));
            return itemsNotInSecondCollection;
        }
    }
}