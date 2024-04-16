using Newtonsoft.Json.Linq;

namespace C4S.Shared.Extensions
{
    public static class JTokenExtensions
    {
        /// <summary>
        /// Получает значение типа <typeparamref name="T"/> из указанного <paramref name="jToken"/> по <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Тип значения, которое необходимо извлечь.</typeparam>
        /// <param name="keys">Ключи, по которым следует извлечь значение из <see cref="JToken"/>.</param>
        /// <param name="jToken"><see cref="JToken"/>, из которого нужно извлечь значение.</param>
        /// <returns>
        /// Извлеченное значение типа <typeparamref name="T"/>.
        /// </returns>
        public static T? GetValue<T>(this JToken jToken, params string[] keys)
        {
            T? value = default;

            foreach (var key in keys)
            {
                jToken = jToken[key];

                if (jToken is null)
                    return value;
            }

            if (typeof(T).IsArray)
                value = jToken.ToObject<T>();
            else
                value = jToken.Value<T>();

            return value;
        }
    }
}