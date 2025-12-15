FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Riff.NotificationService/Riff.NotificationService.csproj", "Riff.NotificationService/"]
COPY ["Riff.Api.Contracts/Riff.Api.Contracts.csproj", "Riff.Api.Contracts/"]
RUN dotnet restore "Riff.NotificationService/Riff.NotificationService.csproj"
COPY . .
WORKDIR "/src/Riff.NotificationService"
RUN dotnet publish "./Riff.NotificationService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Riff.NotificationService.dll"]