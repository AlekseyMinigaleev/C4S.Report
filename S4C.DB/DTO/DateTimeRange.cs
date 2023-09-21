﻿using FluentValidation;
using FluentValidation.Results;

namespace C4S.DB.DTO
{
    /// <summary>
    /// Временной интервал
    /// </summary>
    public class DateTimeRange
    {
        /// <summary>
        /// Начальная дата
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конечная дата
        /// </summary>
        public DateTime FinishDate { get; set; }

        public DateTimeRange(
            DateTime startDate,
            DateTime finishDate)
        {
            StartDate = startDate;
            FinishDate = finishDate;
        }

        /// <summary>
        /// Выполняет валидацию модели
        /// </summary>
        /// <returns>
        /// <see cref="ValidationResult"/>
        /// </returns>
        public async Task<ValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
        {
            var validator = new DateTimeRangeValidator();
            var validationResult = await validator.ValidateAsync(this, cancellationToken);

            return validationResult;
        }
    }

    public class DateTimeRangeValidator : AbstractValidator<DateTimeRange>
    {
        public DateTimeRangeValidator()
        {
            RuleFor(x => x.StartDate.Date)
                .LessThan(x => x.FinishDate.Date)
                .WithMessage(x => $"значение StartDate:{x.StartDate} должно иметь значение, меньше чем FinishDate: {x.FinishDate}");
        }
    }
}