FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build
COPY ProductProvisioner.csproj ProductProvisioner.csproj
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app /p:UseAppHost=false /p:PublishReadyToRun=true

FROM mcr.microsoft.com/dotnet/runtime:10.0-noble-chiseled-extra
WORKDIR /app

COPY --from=build --chown=$APP_UID:$APP_UID /app .

ENV DOTNET_EnableDiagnostics=0 \
    COMPlus_EnableDiagnostics=0 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    HOME=/tmp \
    TMPDIR=/tmp

USER $APP_UID

ENTRYPOINT ["dotnet", "ProductProvisioner.dll"]
