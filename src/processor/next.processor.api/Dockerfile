# See https://aka.ms/customizecontainer 
# to learn how to customize your debug container and 
# how Visual Studio uses this Dockerfile to build your images 
# for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["next.processor.api/nuget.config", "next.processor.api/"]
COPY ["nuget.config", "."]
COPY ["next.processor.api/next.processor.api.csproj", "next.processor.api/"]
RUN dotnet restore "./next.processor.api/next.processor.api.csproj"
COPY . .
WORKDIR "/src/next.processor.api"
RUN dotnet build "./next.processor.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./next.processor.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "next.processor.api.dll"]