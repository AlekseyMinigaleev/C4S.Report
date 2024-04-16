using C4S.API.Models;

namespace C4S.API.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Выполняет пагинацию для <typeparamref name="IQueryable"/> коллекции.
        /// </summary>
        /// <param name="paginate">параметры пагинации</param>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, Paginate paginate) =>
            source
                .Skip((paginate.PageNumber - 1) * paginate.ItemsPerPage)
                .Take(paginate.ItemsPerPage);
    }
}