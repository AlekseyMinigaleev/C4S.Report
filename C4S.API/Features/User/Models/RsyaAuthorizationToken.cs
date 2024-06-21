using FluentValidation;
namespace C4S.API.Features.User.Models
{
    public class RsyaAuthorizationToken
    {
        public RsyaAuthorizationToken(string? token)
        {
            Token = token;
        }

        /// <summary>
        /// Токен авторизации для апи /partner2.yandex.ru/api
        /// </summary>
        public string? Token { get; set; }
    }

    public class RsyaAuthorizationTokenValidator : AbstractValidator<RsyaAuthorizationToken>
    {
        public RsyaAuthorizationTokenValidator(IHttpClientFactory httpClientFactory)
        {
            RuleFor(x => x.Token)
                .MustAsync(async (authorizationToken, cancellationToken) =>
                    {
                        if (authorizationToken is null)
                            return true;

                        if (!authorizationToken.StartsWith("y0_"))
                            return false;

                        var testUrl = "https://partner2.yandex.ru/api/statistics2/tree.json?lang=ru";

                        var httpRequestMethod = new HttpRequestMessage(HttpMethod.Get, testUrl);
                        httpRequestMethod.Headers.Add("Authorization", authorizationToken);

                        using var httpClient = httpClientFactory.CreateClient();
                        var response = await httpClient.SendAsync(httpRequestMethod, cancellationToken);

                        return response.IsSuccessStatusCode;
                    })
                .WithErrorCode("ErrorMessages")
                .WithMessage("Указан неверный токен авторизации РСЯ");
        }
    }   
}