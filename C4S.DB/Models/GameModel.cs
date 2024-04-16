using C4S.Shared.Extensions;

namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица игры
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; private set; }

        /// <remarks>
        /// appId со страницы разработчика
        /// </remarks>
        public int AppId { get; private set; }

        /// <summary>
        /// Id страницы игры.
        /// </summary>
        /// <remarks>
        /// Поле необходимое для  получения данных с РСЯ
        /// </remarks>
        public long? PageId { get; private set; }

        /// <summary>
        /// Название игры
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime PublicationDate { get; private set; }

        /// <summary>
        /// Ссылка на картинку игры
        /// </summary>
        public string PreviewURL { get; set; }

        /// <summary>
        /// Ссылка на игру
        /// </summary>
        public string URL => $"{User.DeveloperPageUrl}#app={AppId}";

        /// <summary>
        /// Флаг указывающий, архивирована ли игра
        /// </summary>
        public bool IsArchived { get; private set; }

        /// <summary>
        /// Пользователь, которому принадлежит игра
        /// </summary>
        public UserModel User { get; private set; }

        /// <summary>
        /// FK <see cref="UserModel"/>
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Список записей статистики
        /// </summary>
        public ISet<GameStatisticModel> GameStatistics { get; set; }

        /// <summary>
        /// Список категорий игры
        /// </summary>
        public ISet<CategoryModel> Categories => CategoryGameModels
            .Select(x => x.Category)
            .ToHashSet();

        /// <summary>
        /// Список связей <see cref="GameModel"/> - <see cref="CategoryModel"/>
        /// </summary>
        public ISet<CategoryGameModel> CategoryGameModels { get; private set; }

        public GameModel(
            int appId,
            UserModel user,
            ISet<CategoryModel> categories,
            string name,
            DateTime publicationDate,
            ISet<GameStatisticModel> gameStatistics)
        {
            Id = Guid.NewGuid();
            AppId = appId;
            User = user;
            UserId = user.Id;
            Name = name;
            AddCategories(categories);
            PublicationDate = publicationDate;
            GameStatistics = gameStatistics;
        }

        /// <summary>
        /// Выполняет обновление сущности
        /// </summary>
        /// <param name="name">Название игры</param>
        /// <param name="publicationDate">дата публикации</param>
        /// <param name="previewURL">Ссылка на превью</param>
        /// <param name="categories">категории игры</param>
        public void Update(
            string name,
            DateTime publicationDate,
            string previewURL,
            IEnumerable<CategoryModel> categories)
        {
            Name = name;
            PublicationDate = publicationDate;
            PreviewURL = previewURL;
            UpdateCategories(categories);
        }

        /// <summary>
        /// Изменяет список статусов игры на <paramref name="categories"/>
        /// </summary>
        /// <param name="categories">Новый список статусов игры</param>
        public void AddCategories(IEnumerable<CategoryModel> categories)
        {
            foreach (var category in categories)
                AddCategory(category);
        }

        /// <summary>
        /// Добавляет <paramref name="category"/> к списку статусов игры
        /// </summary>
        /// <param name="category">Добавляемый статус</param>
        public void AddCategory(CategoryModel category) =>
            CategoryGameModels.Add(new CategoryGameModel(this, category));

        /// <summary>
        /// Удаляет указанные категории из коллекции.
        /// </summary>
        /// <param name="categories">Набор категорий для удаления.</param>
        public void RemoveCategories(IEnumerable<CategoryModel> categories)
        {
            foreach (var category in categories)
                RemoveCategory(category);
        }

        /// <summary>
        /// Удаляет указанную категорию из коллекции категорий игр.
        /// </summary>
        /// <param name="category">Категория для удаления.</param>
        public void RemoveCategory(CategoryModel category)
        {
            var categoryGameModelToRemove = CategoryGameModels
                   .First(x => x.CategoryId == category.Id);

            CategoryGameModels.Remove(categoryGameModelToRemove);
        }

        /// <summary>
        /// Устанавливает указанный <paramref name="pageId"/>
        /// </summary>
        /// <param name="pageId">Id страницы, необходимо для РСЯ</param>
        public void SetPageId(long? pageId) => PageId = pageId;

        /// <summary>
        /// Устанавливает указанный флаг <paramref name="isArchived"/>
        /// </summary>
        /// <param name="isArchived">Флаг указывающий, архивирована ли игра</param>
        public void SetIsArchived(bool isArchived) => IsArchived = isArchived;

        /// <summary>
        /// Устанавливает поля пользователя, которому принадлежит игра
        /// </summary>
        /// <param name="user">пользователь, которому принадлежит игра</param>
        public void SetUser(UserModel user)
        {
            User = user;
            UserId = user.Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var gameModel = (GameModel)obj;

            return AppId == gameModel.AppId;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Id, AppId);

        private void UpdateCategories(IEnumerable<CategoryModel> categories)
        {
            var categoriesToRemove = Categories
                .GetItemsNotInSecondCollection(categories);
            RemoveCategories(categoriesToRemove);

            var categoriesToAdd = categories
                .GetItemsNotInSecondCollection(Categories);
            AddCategories(categoriesToAdd);
        }

        private GameModel()
        { }
    }
}