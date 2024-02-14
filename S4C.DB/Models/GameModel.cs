﻿namespace C4S.DB.Models
{
    /// <summary>
    /// Таблица игры
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// PK
        /// </summary>
        public Guid Id { get; set; }

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
        public int? PageId { get; private set; }

        /// <summary>
        /// Название игры
        /// </summary>
        public string? Name { get; private set; }

        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? PublicationDate { get; private set; }

        /// <summary>
        /// Пользователь, которому принадлежит игра
        /// </summary>
        public UserModel User { get; private set; }

        /// <summary>
        /// FK <see cref="UserModel"/>
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Ссылка на картинку игры
        /// </summary>
        public string? PreviewURL { get; set; }

        /// <summary>
        /// Ссылка на игру
        /// </summary>
        public string? URL => $"{User.DeveloperPageUrl}#app={AppId}";

        /// <summary>
        /// Список записей статистики
        /// </summary>
        public ISet<GameStatisticModel> GameStatistics { get; private set; }

        private GameModel()
        { }

        public GameModel(
            int appId,
            UserModel user,
            string? name = default,
            DateTime? publicationDate = default,
            ISet<GameStatisticModel>? gameStatistics = default)
        {
            Id = Guid.NewGuid();
            AppId = appId;
            User = user;
            UserId = user.Id;
            Name = name;
            PublicationDate = publicationDate;
            GameStatistics = gameStatistics;
        }

        /// <summary>
        /// Выполняет обновление сущности
        /// </summary>
        /// <param name="name">Название игры</param>
        /// <param name="publicationDate">дата публикации</param>
        public void Update(string name, DateTime publicationDate, string previewURL)
        {
            Name = name;
            PublicationDate = publicationDate;
            PreviewURL = previewURL;
        }

        /// <summary>
        /// Устанавливает указанный <paramref name="pageId"/>
        /// </summary>
        /// <param name="pageId">Id страницы, необходимо для РСЯ</param>
        public void SetPageId(int pageId) => PageId = pageId;

        /*TODO: Очень жесткое дублирование*/

        /// <inheritdoc cref="HasChanges(GameModel)"/>
        /// <param name="incomingFields"> Поля с которым происходит сравнение</param>
        public bool HasChanges(GameModifiableFields incomingFields)
        {
            var hasChanges = Name == incomingFields.Name
                && PublicationDate == incomingFields.PublicationDate
                && incomingFields.PreviewURL == PreviewURL;

            return !hasChanges;
        }

        /*TODO: Очень жесткое дублирование*/

        /// <summary>
        /// Проверяет есть ли изменения у модели по сравнению с <paramref name="incomingGameModel"/>
        /// </summary>
        /// <param name="incomingGameModel">Игра с которой происходит сравнение</param>
        /// <returns>
        /// <see langword="true"/> если в модели есть изменения, иначе <see langword="false"/>
        /// </returns>
        public bool HasChanges(GameModel incomingGameModel)
        {
            var hasChanges = Name == incomingGameModel.Name
                && PublicationDate == incomingGameModel.PublicationDate
                && incomingGameModel.PreviewURL == PreviewURL;

            return hasChanges;
        }
    }

    /*TOOD: очень сомнительная штука, добавляет много гемора, нужно пересмотреть*/

    /// <summary>
    /// Изменяемые поля модели <see cref="GameModel"/>
    /// </summary>
    public class GameModifiableFields
    {
        /// <inheritdoc cref="GameModel.Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="GameModel.PublicationDate"/>
        public DateTime PublicationDate { get; set; }

        /// <inheritdoc cref="GameModel.PreviewURL"/>
        public string PreviewURL { get; set; }

        public GameModifiableFields(
            string name,
            DateTime publicationDate,
            string previewURL)
        {
            Name = name;
            PublicationDate = publicationDate;
            PreviewURL = previewURL;
        }

        private GameModifiableFields()
        { }
    }
}