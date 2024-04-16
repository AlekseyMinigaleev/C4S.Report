namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица статуса игры
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Название статуса
        /// </summary>
        public string Title { get; private set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var category = (CategoryModel)obj;

            return Name == category.Name;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Id, Name);

        public CategoryModel(
            string name,
            string title)
        {
            Id = Guid.NewGuid();
            Name = name;
            Title = title;
        }

        private CategoryModel()
        { }
    }
}