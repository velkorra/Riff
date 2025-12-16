FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

COPY backend/Riff.sln .
COPY backend/Riff.Api/Riff.Api.csproj ./Riff.Api/
COPY backend/Riff.Api.Contracts/Riff.Api.Contracts.csproj ./Riff.Api.Contracts/
COPY backend/Riff.Infrastructure/Riff.Infrastructure.csproj ./Riff.Infrastructure/
COPY backend/Riff.Tests/Riff.Tests.csproj ./Riff.Tests/
COPY backend/Riff.PlaylistService/Riff.PlaylistService.csproj ./Riff.PlaylistService/
COPY backend/Riff.NotificationService/Riff.NotificationService.csproj ./Riff.NotificationService/

RUN dotnet restore Riff.sln

COPY backend/. .

WORKDIR /src/Riff.Tests

ENTRYPOINT ["dotnet", "test"]