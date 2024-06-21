# C4S.Report

## Описание проекта

C4S.Report - это проект для автоматизации сбора статистики по играм на платформе Яндекс Игры. Платформа предоставляет два различных ресурса для отслеживания статистики: один для метрик, связанных с доходом игры, и другой для метрик, связанных с самой игрой (оценка, рейтинг, дата публикации и прочие). Наш продукт решает проблему консолидации данных и автоматического сбора информации из разных источников.

## Текущий статус

Проект находится в стадии разработки. Возможны ошибки и баги. Актуальная версия проекта находится в ветке `dev`.

## Основные функции

- **Консолидация данных**: объединение данных из двух ресурсов Яндекс Игры в один интерфейс.
- **Автоматический сбор данных**: автоматический сбор данных из двух ресурсов Яндекс Игры в запланированное время.
- **Настройка частоты синхронизации данных**: возможность настройки частоты синхронизации данных.

## Архитектура проекта

Проект состоит из двух частей:

- **Backend**: репозиторий с исходным кодом [C4S.Report Backend](https://github.com/AlekseyMinigaleev/C4S.Report/tree/dev).
- **Frontend**: репозиторий с исходным кодом фронтенда [C4S.Report Frontend](https://github.com/AlekseyMinigaleev/s4c-report-front/tree/dev).

## Технологии

Проект использует следующие технологии:

- **Язык программирования**: C#
- **Фреймворк**: ASP.NET Core Web API
- **Орагнизация модуля API**: 
  - Vertical Slice Architecture
  - CQRS (Command Query Responsibility Segregation)
  - MediatR
- **Маппинг данных**: Automapper
- **Валидация**: FluentValidation
- **Фоновые задачи**: Hangfire
- **Парсинг HTML**: Selenium и веб-драйвер FireFox
- **База данных**: SQL Server
- **ORM**: Entity Framework Core (Code First подход)

## Установка и запуск

### Установка с помощью Git

1. Склонируйте репозиторий с GitHub:
    ```sh
    git clone -b dev https://github.com/AlekseyMinigaleev/C4S.Report.git
    cd C4S.Report
    ```

2. Установите зависимости:
    ```sh
    dotnet restore
    ```

3. Запустите проект:
    ```sh
    dotnet run
    ```

### Установка с помощью Docker Compose

Если вы предпочитаете использовать Docker для развертывания проекта, следуйте этим шагам:

1. Убедитесь, что у вас установлены Docker и Docker Compose.

2. Создайте файл `docker-compose.yml` с указанным ниже содержимым:

    ```yaml
    version: '3.4'

    networks:
      c4s.report:

    services:
      db:
        container_name: db
        image: mcr.microsoft.com/mssql/server:2022-latest
        ports:
          - 8001:1433
        environment:
          - ACCEPT_EULA=Y
          - MSSQL_SA_PASSWORD=your_password
        networks:
          - c4s.report
      api:
        container_name: api
        image: alekseyminigaleev/c4sapi:3.1
        ports:
          - 8080:8080
        depends_on:
          - db
        networks:
          - c4s.report
      front:
        container_name: front
        image: alekseyminigaleev/c4s-front:3.1
        ports:
          - 3000:3000
        networks:
          - c4s.report
    ```

3. Запустите Docker Compose:
    ```sh
    docker-compose up
    ```

Эта конфигурация позволяет легко и быстро развернуть все необходимые компоненты проекта C4S.Report, используя Docker Compose. Все сервисы будут доступны на соответствующих портах на вашем локальном компьютере.

## Регистрация нового аккаунта

Для регистрации нового аккаунта выполните следующие шаги:

1. **Запустите проект**: Проект использует Swagger для удобства тестирования API, поэтому можно воспользоваться Swagger UI или использовать curl.

2. **Выполните POST запрос** на эндпоинт `http://localhost:5041/api/Authentication/createAccount`:

    ```sh
    curl -X 'POST' \
      'http://localhost:5041/api/Authentication/createAccount' \
      -H 'accept: */*' \
      -H 'Content-Type: application/json' \
      -d '{
        "credentionals": {
          "login": "your_email",
          "password": "your_password"
        },
        "developerPageUrl": "your_developerPageURL",
        "rsyaAuthorizationToken": "your_rsya_authorization_token"
      }'
    ```

    - `your_email`: ваша почта для регистрации.
    - `your_password`: пароль для нового аккаунта.
    - `your_developerPageURL`: ссылка на страницу разработчика игры в Яндекс Играх (например, `"https://yandex.ru/games/developer/42543"`.
    - `your_rsya_authorization_token`: токен авторизации РСЯ, который можно получить на сайте [Яндекс.Разработчик](https://yandex.ru/dev/partner/) после нажатия на кнопку "Получить OAuth-токен".
    
![image](https://github.com/AlekseyMinigaleev/C4S.Report/assets/113462817/0bf27557-d73d-4db8-8377-c8d6be346ce0)

3. **Подтвердите почту**: После создания аккаунта на указанную почту будет отправлено письмо с подтверждением. Для подтверждения почты выполните POST запрос на эндпоинт `http://localhost:5041/api/Email/verify-email-verification-code` с кодом подтверждения, который пришел на почту:

    ```sh
    curl -X 'POST' \
      'http://localhost:5041/api/Email/verify-email-verification-code' \
      -H 'accept: */*' \
      -H 'Authorization: Bearer your_access_token_here' \
      -H 'Content-Type: application/json' \
      -d '{
        "verificationCode": "your_verification_code_here"
      }'
    ```
    
    - `your_access_token_here`: ваш access токен для аутентификации запросов.
    - `your_verification_code_here`: код подтверждения, полученный на почту после регистрации.

 После успешного подтверждения почты  аккаунт считается активным. Если не подтвердить почту, аккаунт будет считаться неактивным. Вы сможете повторно зарегистрироваться с той же почтой, удалив текущий аккаунт.
 
4. **Установите pageId**: Для получения данных о доходе необходимо установить `pageId` для каждой игры, по которой вы хотите получать статистику. 

Для того чтобы получить `pageId` для каждой игры на платформе Яндекс Игры, необходимо выполнить следующие шаги:

1. **Войдите в аккаунт на Яндекс.Партнер**: [Перейдите на сайт Яндекс.Партнер](https://partner.yandex.ru/) и войдите в свой аккаунт.

2. Найдите и откройте раздел отчетов по сайтам.

![photo_2024-06-21_19-12-50](https://github.com/AlekseyMinigaleev/C4S.Report/assets/113462817/0cce9a15-e169-416e-98a9-1d7cf4b436db)

4. **Выберите нужную игру**: Найдите в списке игру, для которой необходимо установить `pageId`.

![image](https://github.com/AlekseyMinigaleev/C4S.Report/assets/113462817/6d408822-2dc2-478c-8084-1a555b6bdcc0)

    ```sh
    # Установка pageId
    curl -X 'PUT' \
      'http://localhost:5041/api/Game/set-pageId' \
      -H 'accept: */*' \
      -H 'Authorization: Bearer your_access_token_here' \
      -H 'Content-Type: application/json' \
      -d '{
        "body": [
          {
            "gameId": "your_gameId_here",
            "pageId": your_pageId_here
          }
        ]
      }'
    ```
    
  - `your_pageId_here`: идентификатор страницы, который можно получить из отчетов на сайте [Яндекс.Партнер](https://partner.yandex.ru/).
  - `your_gameId_here`: идентификатор игры, который вы получили из предыдущего запроса.
  
Получите `gameId` с помощью запроса на эндпоинт `http://localhost:5041/api/Game/get-games`

    ```sh
    # Получение gameId
    curl -X 'GET' \
      'http://localhost:5041/api/Game/get-games?Paginate.ItemsPerPage=100&Paginate.PageNumber=1&Sort.FieldName=evaluation&Sort.SortType=0' \
      -H 'accept: */*' \
      -H 'Authorization: Bearer your_access_token_here'
    ```
    
  - `your_access_token_here`: ваш access токен для аутентификации запросов.    
  
## Контакты
- email: aleksey.mingialeev@gmail.com
- tg: https://t.me/AlekseyMinigaleev
- WhatsApp: +7-919-606-68-20
