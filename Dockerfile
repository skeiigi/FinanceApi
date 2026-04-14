# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Копируем csproj и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем всё остальное и собираем
COPY . ./
RUN dotnet publish -c Release -o out

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Railway подставит порт автоматически через переменную PORT
ENTRYPOINT ["dotnet", "FinanceApi.dll"]