# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base stage: Set the base image to be used for the container
# In this case, we're using the official ASP.NET Core runtime image from Microsoft's container registry
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Set the working directory to "/app"
WORKDIR /app

# Expose ports 80 and 443, which are commonly used for HTTP and HTTPS traffic
EXPOSE 80
EXPOSE 443

# Build stage: Set the second stage to be the .NET Core SDK image from Microsoft's container registry
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory to "/src"
WORKDIR /src

# Copy the project file into the container
COPY ["game-master-copilot-server/game-master-copilot-server.csproj", "game-master-copilot-server/"]

# Restore any dependencies required by the project
RUN dotnet restore "game-master-copilot-server/game-master-copilot-server.csproj"

# Copy the entire project into the container and set the working directory to the project directory
COPY . .

# Set the working directory to the project directory
WORKDIR "/src/game-master-copilot-server"

# Build the project in release mode and output the build artifacts to the "/app/build" directory
RUN dotnet build "game-master-copilot-server.csproj" -c Release -o /app/build

# Publish stage: Set the third stage to be the "build" stage
FROM build AS publish

# Publish the project in release mode and output the publish artifacts to the "/app/publish" directory
RUN dotnet publish "game-master-copilot-server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: Set the fourth and final stage to be the "base" stage
FROM base AS final

# Set the working directory to "/app"
WORKDIR /app

# Copy the publish artifacts from the "publish" stage into the container
COPY --from=publish /app/publish .

# Set the entry point to be the "dotnet" command with the name of the DLL file to run
ENTRYPOINT ["dotnet", "game-master-copilot-server.dll"]
