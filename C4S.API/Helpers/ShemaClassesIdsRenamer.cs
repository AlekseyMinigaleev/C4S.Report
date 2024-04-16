namespace C4S.API.Helpers
{
    /*TODO: разобраться + рефакторинг*/

    /// <summary>
    /// Переименовыватель id схемык класса
    /// </summary>
    public static class ShemaClassesIdsRenamer
    {
        private static readonly string[] _specialWords = new string[4] { "ViewModel", "Body", "Query", "Command" };

        /// <summary>
        /// хз че это
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Selector(Type t)
        {
            if (_specialWords.Contains(t.Name) && (object)t.DeclaringType != null)
                return t.DeclaringType!.Name + "." + t.Name;

            var declaringType = t.DeclaringType;
            var text = string.Empty;
            while (declaringType != null)
            {
                text = declaringType.Name + "." + text;
                declaringType = declaringType.DeclaringType;
            }

            if (t.IsGenericType)
            {
                var genericArguments = t.GetGenericArguments();
                var text2 = string.Join(string.Empty, genericArguments.Select((x) => x.Name));
                var trimChars = $"`{genericArguments.Length}".ToCharArray();
                return (text + t.Name).TrimEnd(trimChars) + text2;
            }

            return text + t.Name;
        }
    }
}