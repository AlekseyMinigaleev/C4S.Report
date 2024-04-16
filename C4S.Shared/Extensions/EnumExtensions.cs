using System.ComponentModel.DataAnnotations;

namespace C4S.Shared.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Возвращает имя <paramref name="value"/>, указанное в атрибуте <see cref="DisplayAttribute"/>
        /// </summary>
        /// <remarks>
        /// Если атрибут <see cref="DisplayAttribute"/> не указан, возвращает результат работы метода <see cref="Enum.GetName(Type, object)"/>
        /// </remarks>
        /// <param name="value">Значение <see cref="Enum"/>, имя которого нужно получить</param>
        /// <returns></returns>
        public static string GetName(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name != null)
            {
                var fieldInfo = type.GetField(name);
                var result = (DisplayAttribute?)Attribute
                    .GetCustomAttribute(fieldInfo!, typeof(DisplayAttribute));

                if (result is not null)
                    name = result.Name;
            }

            return name;
        }
    }
}
