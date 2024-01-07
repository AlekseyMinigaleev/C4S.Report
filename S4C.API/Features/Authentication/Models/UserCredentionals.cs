﻿using FluentValidation;

namespace С4S.API.Features.Authentication.ViewModels
{
    /// <summary>
    /// Представляет учетные данные пользователя
    /// </summary>
    public class UserCredentionals
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
    }

    public class UserCredentionalsValidator : AbstractValidator<UserCredentionals>
    {
        public UserCredentionalsValidator()
        {
            RuleFor(x => x.Login)
                .EmailAddress()
                    .WithMessage("Некорректный формат электронной почты");

            RuleFor(x => x.Password)
                .MinimumLength(8)
                    .WithMessage("Минимальная длина пароля - 8 символов")
                .Matches("[A-Z]")
                    .WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
                .Matches("[a-z]")
                    .WithMessage("Пароль должен содержать хотя бы одну строчную букву")
                .Matches("[0-9]")
                    .WithMessage("Пароль должен содержать хотя бы одну цифру");
                //.Matches("[!@#$%^&*]")
                //    .WithMessage("Пароль должен содержать хотя бы один специальный символ: !@#$%^&*");
        }
    }
}
