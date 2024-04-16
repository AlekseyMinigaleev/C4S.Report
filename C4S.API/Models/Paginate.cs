using FluentValidation;

namespace С4S.API.Models
{
    /// <summary>
    /// Параметры для пагинации
    /// </summary>
    public class Paginate
    {
        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// номер страницы
        /// </summary>
        public int PageNumber { get; set; }
    }

    public class PaginateValidator : AbstractValidator<Paginate>
    {
        public PaginateValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Минимальное значение для поля 'ItemsPerPage' - 1");

            RuleFor(x => x.ItemsPerPage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Минимальное значение для поля 'PageNumber' - 0");
            ;
        }
    }

}