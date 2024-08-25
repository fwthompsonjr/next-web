#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update -y \
    && apt-get install --no-install-recommends --no-install-suggests -y tzdata ca-certificates bzip2 curl wget libc-dev libxt6 \
    && apt-get install --no-install-recommends --no-install-suggests -y `apt-cache depends firefox-esr | awk '/Depends:/{print$2}'` \
    && update-ca-certificates \
    # Cleanup unnecessary stuff
    && apt-get purge -y --auto-remove \
                  -o APT::AutoRemove::RecommendsImportant=false \
    && rm -rf /var/lib/apt/lists/* /tmp/*

# install geckodriver

    RUN wget https://github.com/mozilla/geckodriver/releases/download/v0.31.0/geckodriver-v0.31.0-linux64.tar.gz && \
        tar -zxf geckodriver-v0.31.0-linux64.tar.gz -C /usr/local/bin && \
        chmod +x /usr/local/bin/geckodriver && \
        rm geckodriver-v0.31.0-linux64.tar.gz

# install firefox

    RUN FIREFOX_SETUP=firefox-setup.tar.bz2 && \
        wget -O $FIREFOX_SETUP "https://download.mozilla.org/?product=firefox-95.0.1&os=linux64" && \
        tar xjf $FIREFOX_SETUP -C /opt/ && \
        ln -s /opt/firefox/firefox /usr/bin/firefox && \
        rm $FIREFOX_SETUP


USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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
## Copy the published output to the final image
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "next.processor.api.dll"]