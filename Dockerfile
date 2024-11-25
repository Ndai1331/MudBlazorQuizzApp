FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QuizAppBlazor/Client/QuizAppBlazor.Client.csproj", "QuizAppBlazor/Client/"]
RUN dotnet restore "QuizAppBlazor/Client/QuizAppBlazor.Client.csproj"
COPY . .
WORKDIR "/src/QuizAppBlazor/Client"
RUN dotnet build "QuizAppBlazor.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuizAppBlazor.Client.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine as final
WORKDIR /var/www/web
COPY --from=publish /app/publish/wwwroot .
COPY QuizAppBlazor/Client/nginx.conf /etc/nginx/nginx.conf

