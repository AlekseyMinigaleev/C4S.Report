FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update
RUN apt-get install -y xvfb
RUN echo "deb http://deb.debian.org/debian/ unstable main contrib non-free" >> /etc/apt/sources.list.d/debian.list
RUN apt-get update
RUN apt-get install -y libcrypt1
RUN apt-get install -y firefox
WORKDIR /app
EXPOSE 5041

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