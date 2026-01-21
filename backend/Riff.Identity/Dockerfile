FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Riff.Identity/Riff.Identity.csproj", "Riff.Identity/"]
COPY ["Riff.Infrastructure/Riff.Infrastructure.csproj", "Riff.Infrastructure/"]
COPY ["Riff.Api.Contracts/Riff.Api.Contracts.csproj", "Riff.Api.Contracts/"]
RUN dotnet restore "Riff.Identity/Riff.Identity.csproj"
COPY . .
WORKDIR "/src/Riff.Identity"
RUN dotnet publish "./Riff.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Riff.Identity.dll"]
