using AutoMapper;
using C4S.DB.Models;
using System.Linq.Expressions;

namespace C4S.Services.Services.GetGamesDataService.Models
{
    /// <summary>
    /// Информация об игре
    /// </summary>
    /// <remarks>
    /// Включает в себя все необходимые поля для <see cref="GameModel"/> и <see cref="GameStatisticModel"/>
    /// </remarks>
    public class PublicGameData
    {
        #region GameModelInfo

        /// <inheritdoc cref="GameModel.Name"/>
        public string Title { get; set; }

        /// <inheritdoc cref="GameModel.AppId"/>
        public int AppId { get; set; }

        /// <inheritdoc cref="GameModel.PreviewURL"/>
        public string PreviewURL { get; set; }

        /// <inheritdoc cref="GameModel.PublicationDate"/>
        public int FirstPublished { get; set; }

        /// <summary>
        /// Имена всех категорий, к которым относится игра
        /// </summary>
        public string[] CategoriesNames { get; set; }

        #endregion GameModelInfo

        #region GameStatisticModel

        /// <inheritdoc cref="GameStatisticModel.Evaluation"/>
        public double Evaluation { get; set; }

        /// <inheritdoc cref="GameStatisticModel.Rating"/>
        public int? Rating { get; set; }

        # endregion GameStatisticModel

        public PublicGameData(
            int appId,
            string title,
            int firstPublished,
            double evaluation,
            string[] categoriesNames,
            string previewURL,
            int? rating = default)
        {
            AppId = appId;
            Title = title;
            FirstPublished = firstPublished;
            Evaluation = evaluation;
            CategoriesNames = categoriesNames;
            PreviewURL = previewURL;
            Rating = rating;
        }

        private PublicGameData()
        { }
    }

    /// <summary>
    /// Справочник <see cref="Expression"/> для <see cref="PublicGameData"/>
    /// </summary>
    public static class GameInfoExpression
    {
        /// <summary>
        /// <see cref="Expression"/>, указывающий имеет ли игра статус "новая"
        /// </summary>
        public static Expression<Func<PublicGameData, bool>> IsNew =
            (gameInfo) => gameInfo.CategoriesNames.Contains("new");

        /// <summary>
        ///  <see cref="Expression"/>, указывающий имеет ли игра статус "продвигаемая"
        /// </summary>
        public static Expression<Func<PublicGameData, bool>> IsPromoted =
            (gameInfo) => gameInfo.CategoriesNames.Contains("new");
    }

    /// <summary>
    /// Профиль маппинга в <see cref="GameModel"/> из <see cref="PublicGameData"/>
    /// </summary>
    public class GameModelProfiler : Profile
    {
        public GameModelProfiler()
        {
            CreateMap<PublicGameData, GameModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CategoryGameModels, opt => opt.MapFrom(src => new HashSet<CategoryGameModel>()))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.FirstPublished).DateTime))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PageId, opt => opt.Ignore())
                .ForMember(dest => dest.URL, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.GameStatistics, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<PublicGameData, GameStatisticModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.LastSynchroDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.GameId, opt => opt.Ignore())
                .ForMember(dest => dest.CashIncome, opt => opt.Ignore());
        }
    }
}