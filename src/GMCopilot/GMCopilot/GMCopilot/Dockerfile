#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GMCopilot/GMCopilot/GMCopilot.csproj", "GMCopilot/GMCopilot/"]
COPY ["GMCopilot/GMCopilot.Client/GMCopilot.Client.csproj", "GMCopilot/GMCopilot.Client/"]
COPY ["GMCopilot.Data/GMCopilot.Data.csproj", "GMCopilot.Data/"]
COPY ["GMCopilot.Entities/GMCopilot.Entities.csproj", "GMCopilot.Entities/"]
COPY ["GMCopilot.ServiceDefaults/GMCopilot.ServiceDefaults.csproj", "GMCopilot.ServiceDefaults/"]
RUN dotnet restore "./GMCopilot/GMCopilot/GMCopilot.csproj"
COPY . .
WORKDIR "/src/GMCopilot/GMCopilot"
RUN dotnet build "./GMCopilot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GMCopilot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GMCopilot.dll"]