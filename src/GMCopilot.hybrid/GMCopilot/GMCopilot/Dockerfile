#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GMCopilot/GMCopilot/GMCopilot/GMCopilot.csproj", "GMCopilot/GMCopilot/"]
COPY ["src/GMCopilot/GMCopilot/GMCopilot.Client/GMCopilot.Client.csproj", "GMCopilot/GMCopilot.Client/"]
COPY ["src/GMCopilot/GMCopilot.Core/GMCopilot.Core.csproj", "GMCopilot.Core/"]
COPY ["src/GMCopilot/GMCopilot.ServiceDefaults/GMCopilot.ServiceDefaults.csproj", "GMCopilot.ServiceDefaults/"]
RUN dotnet restore "./GMCopilot/GMCopilot/GMCopilot.csproj"
COPY src/GMCopilot/ .
RUN dotnet build "./GMCopilot/GMCopilot/GMCopilot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GMCopilot/GMCopilot/GMCopilot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GMCopilot.dll"]