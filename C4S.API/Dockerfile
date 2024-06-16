FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Установка необходимых инструментов и зависимостей
RUN apt-get update && apt-get install -y wget gnupg2 unzip lsb-release libappindicator3-1

# Установка Google Chrome
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable

RUN wget -q -O /tmp/chromedriver_linux64.zip https://chromedriver.storage.googleapis.com/114.0.5735.90/chromedriver_linux64.zip \
    && unzip /tmp/chromedriver_linux64.zip -d /usr/local/bin/ \
    && rm /tmp/chromedriver_linux64.zip \
    && chmod +x /usr/local/bin/chromedriver

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["C4S.API/C4S.API.csproj", "C4S.API/"]
COPY ["C4S.DB/C4S.DB.csproj", "C4S.DB/"]
COPY ["C4S.Shared/C4S.Shared.csproj", "C4S.Shared/"]
COPY ["C4S.Services/C4S.Services.csproj", "C4S.Services/"]
RUN dotnet restore "./C4S.API/./C4S.API.csproj"
COPY . .
WORKDIR "/src/C4S.API"
RUN dotnet build "./C4S.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./C4S.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "C4S.API.dll"]
