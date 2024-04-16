using AngleSharp;
using C4S.DB;
using C4S.DB.Models;
using C4S.Services.Exceptions;
using C4S.Shared.Extensions;
using C4S.Shared.Logger;
using Microsoft.EntityFrameworkCore;

namespace C4S.Services.Services.CategoriesSyncService
{
    /// <inheritdoc cref="ICategoriesSyncService"/>
    public class CategoriesSyncService : ICategoriesSyncService
    {
        private readonly ReportDbContext _dbContext;
        private readonly IBrowsingContext _browsingContext;
        private readonly string _categoriesURL;
        private BaseLogger _logger;

        public CategoriesSyncService(
            ReportDbContext dbContext,
            IBrowsingContext browsingContext,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _dbContext = dbContext;
            _browsingContext = browsingContext;
            _categoriesURL = configuration["CategoriesURL"]!;
            ArgumentException.ThrowIfNullOrEmpty(
               _categoriesURL,
               "в файле appsetting.json не указана или указана неверно ссылка на категории");
        }

        /// <inheritdoc/>
        public async Task SyncCategoriesAsync(BaseLogger logger, CancellationToken cancellationToken)
        {
            _logger = logger;

            _logger.LogInformation("Запущен процесс синхронизации статусов игр");

            _logger.LogInformation("Запущен процесс получения статусов");
            var incomingCategories = await GetIncomingCategories(cancellationToken);
            _logger.LogSuccess($"процесс получения статусов успешно завершен. Получено {incomingCategories.Count} статусов");

            _logger.LogInformation("Запущен процесс обработки полученных статусов");
            await ProcessIncomingCategories(incomingCategories, cancellationToken);
            _logger.LogSuccess("процесс обработки полученных статусов успешно завершен");

            _logger.LogSuccess("процесс синхронизации статусов игр успешно завершен");
        }

        private async Task<List<CategoryModel>> GetIncomingCategories(CancellationToken cancellationToken)
        {
            var incomingCategoryModels = new List<CategoryModel>();

            var document = await _browsingContext
                .OpenAsync(_categoriesURL, cancellationToken)
                ?? throw new Exception(); /*TODO: сделать общую ошибку когда не удается получить доступ к ресурсу.*/

            var categoriesColumnsSelector = ".categories-page__columns";
            var categoriesColumnListArray = document
                .QuerySelector(categoriesColumnsSelector)
                ?.Children 
                ?? throw new HtmlParsingNullValueException(categoriesColumnsSelector, document);

            foreach (var categoryColumn in categoriesColumnListArray)
            {
                var categoryListElement = categoryColumn.Children;

                foreach (var categoryListItemElement in categoryListElement)
                {
                    var categoryElementSelector = "span.category-wrapper";
                    var categoryElement = categoryListItemElement
                        .QuerySelector(categoryElementSelector)
                        ?? throw new HtmlParsingNullValueException(categoryElementSelector, categoryListItemElement);

                    var dataNameAttributeName = "data-name";
                    var dataName = categoryElement.GetAttribute(dataNameAttributeName) 
                        ?? throw new HtmlParsingNullValueException(dataNameAttributeName, categoryElement);

                    var titleAttributeName = "title";
                    var title = categoryElement.GetAttribute(titleAttributeName)
                        ?? throw new HtmlParsingNullValueException(titleAttributeName, categoryElement);

                    var categoryModel = new CategoryModel(dataName, title);
                    incomingCategoryModels.Add(categoryModel);
                }
            }

            return incomingCategoryModels;
        }

        private async Task ProcessIncomingCategories(
            IEnumerable<CategoryModel> incomingCategories,
            CancellationToken cancellationToken)
        {
            var existCategories = await _dbContext.Categories.ToListAsync(cancellationToken);

            var countOfDeletedCategories = RemoveFromDb(existCategories, incomingCategories);
            _logger.LogInformation($"Добавлено на удаление {countOfDeletedCategories} категорий ");

            var countOfAddedCategories = AddToDb(existCategories, incomingCategories);
            _logger.LogInformation($"Добавлено на добавление {countOfAddedCategories} категорий");

            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogSuccess($"База данных обновлена");
        }

        private int RemoveFromDb(
            IEnumerable<CategoryModel> existCategories,
            IEnumerable<CategoryModel> incomingCategories)
        {
            var categoriesToDelete = existCategories
                .GetItemsNotInSecondCollection(incomingCategories);

            _dbContext.Categories.RemoveRange(categoriesToDelete);

            return categoriesToDelete.Count();
        }

        private int AddToDb(
            IEnumerable<CategoryModel> existCategories,
            IEnumerable<CategoryModel> incomingCategories)
        {
            var categoriesToAdd = incomingCategories
                .GetItemsNotInSecondCollection(existCategories);

            _dbContext.Categories.AddRange(categoriesToAdd);

            return categoriesToAdd.Count();
        }
    }
}